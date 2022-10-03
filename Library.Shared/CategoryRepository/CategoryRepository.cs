using Library.Base;
using Library.DataTransferObjects.Category;
using Library.Models.Shared;
using Library.SharedKernel.OperationResult;
using Library.SQL.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Shared.CategoryRepository
{
    public class CategoryRepository : LibraryRepository, ICategoryRepository
    {
        public CategoryRepository(LibraryDBContext context) : base(context)
        {
        }


        public async Task<OperationResult<IEnumerable<GetCategoryDto>>> GetAll()
        {
            var operation = new OperationResult<IEnumerable<GetCategoryDto>>();
            try
            {
                var categories = await Context.Categories.Select(category => new GetCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                }).ToListAsync();

                operation.SetSuccess(categories);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetCategoryDto>> GetById(Guid id)
        {
            var operation = new OperationResult<GetCategoryDto>();
            try
            {
                var category = await Context.Categories.Where(category => category.Id.Equals(id))
                                                       .Select(category => new GetCategoryDto
                                                       {
                                                           Id = category.Id,
                                                           Name = category.Name,
                                                       }).SingleOrDefaultAsync();

                operation.SetSuccess(category);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetCategoryDto>> Create(SetCategoryDto categoryDto)
        {
            var operation = new OperationResult<GetCategoryDto>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var category = new Category
                    {
                        Name = categoryDto.Name,
                    };

                    Context.Add(category);
                    await Context.SaveChangesAsync();

                    var categoryRes = await GetById(category.Id);
                    operation.SetSuccess(categoryRes.Result);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    operation.SetException(ex);
                    transaction.Rollback();
                }
            }
            return operation;
        }

        public async Task<OperationResult<GetCategoryDto>> Update(UpdateCategoryDto categoryDto)
        {
            var operation = new OperationResult<GetCategoryDto>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var category = await Context.Categories.Where(category => categoryDto.Id.Equals(category.Id)).SingleOrDefaultAsync();

                    if (category is null)
                    {
                        operation.SetFailed($"this category with {category.Id} id not found.");
                    }

                    category.Name = categoryDto.Name;
                    
                    Context.Update(category);
                    await Context.SaveChangesAsync();

                    var categoryRes = await GetById(category.Id);
                    operation.SetSuccess(categoryRes.Result);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    operation.SetException(ex);
                    transaction.Rollback();
                }
            }
            return operation;
        }

        public async Task<OperationResult<bool>> Delete(Guid id)
        {
            var operation = new OperationResult<bool>();
            try
            {
                operation = await DeleteRange(new List<Guid> { id });
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<bool>> DeleteRange(IEnumerable<Guid> ids)
        {
            var operation = new OperationResult<bool>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var categories = await Context.Categories.Include(category => category.BookCategories)
                                                             .Where(category => ids.Contains(category.Id))
                                                             .ToListAsync();
                    if(categories.Any(c => c.BookCategories.Any()))
                    {
                        return operation.SetFailed("There are books in this category");
                    }

                    categories.ForEach(category =>
                    {
                        category.DateDeleted = DateTime.UtcNow;
                    });

                    await Context.SaveChangesAsync();
                    transaction.Commit();
                    operation.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    operation.SetException(ex);
                    transaction.Rollback();
                }
            }
            return operation;
        }
    }
}
