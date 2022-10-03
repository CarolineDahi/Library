using Library.Base;
using Library.DataTransferObjects.Author;
using Library.DataTransferObjects.Book;
using Library.Models.Main;
using Library.Shared.DocumentRepository;
using Library.SharedKernel.OperationResult;
using Library.SQL.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Main.AuthorRepository
{
    public class AuthorRepository : LibraryRepository, IAuthorRepository
    {
        private readonly IDocumentRepository documentRepository;

        public AuthorRepository(LibraryDBContext context, IDocumentRepository documentRepository) : base(context)
        {
            this.documentRepository = documentRepository;
        }

        public async Task<OperationResult<IEnumerable<GetAuthorDto>>> GetAll()
        {
            var operation = new OperationResult<IEnumerable<GetAuthorDto>>();
            try
            {
                var authors = await Context.Authors.Select(author => new GetAuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName, 
                    Address = author.Address,
                    Age = author.Age,
                    Phone = author.Phone,
                    ImageUrl = author.ImagePath,
                    Books = author.AuthorBooks.Select(autherBook  => new BaseBookDto
                    {
                        Id = autherBook.BookId,
                        Title = autherBook.Book.Title
                    }).ToList(),
                }).ToListAsync();

                operation.SetSuccess(authors);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetAuthorDto>> GetById(Guid id)
        {
            var operation = new OperationResult<GetAuthorDto>();
            try
            {
                var author = await Context.Authors.Where(author => author.Id.Equals(id))
                                                  .Select(author => new GetAuthorDto
                                                  {
                                                      Id = author.Id,
                                                      FirstName = author.FirstName,
                                                      LastName = author.LastName,
                                                      Address = author.Address,
                                                      Age = author.Age,
                                                      Phone = author.Phone,
                                                      ImageUrl = author.ImagePath,
                                                      Books = author.AuthorBooks.Select(autherBook => new BaseBookDto
                                                      {
                                                          Id = autherBook.BookId,
                                                          Title = autherBook.Book.Title
                                                      }).ToList(),
                                                  }).SingleOrDefaultAsync();

                operation.SetSuccess(author);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetAuthorDto>> Create(SetAuthorDto authorDto)
        {
            var operation = new OperationResult<GetAuthorDto>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var image = await documentRepository.Upload("Authors", authorDto.Image );
                    
                    var author = new Author
                    {
                        FirstName = authorDto.FirstName,
                        LastName = authorDto.LastName,
                        Address = authorDto.Address,
                        Age = authorDto.Age,
                        Phone = authorDto.Phone,
                        ImagePath = image.Result
                    };

                    Context.Add(author);
                    await Context.SaveChangesAsync();

                    var authorRes = await GetById(author.Id);
                    operation.SetSuccess(authorRes.Result);

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

        public async Task<OperationResult<GetAuthorDto>> Update(UpdateAuthorDto authorDto)
        {
            var operation = new OperationResult<GetAuthorDto>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var author = await Context.Authors.Where(author => authorDto.Id.Equals(author.Id)).SingleOrDefaultAsync();

                    if(author is null)
                    {
                        operation.SetFailed($"this author with {author.Id} id not found.");
                    }

                    author.FirstName = authorDto.FirstName;
                    author.LastName = authorDto.LastName;
                    author.Address = authorDto.Address;
                    author.Age = authorDto.Age;
                    author.Phone = authorDto.Phone;
                    
                    if(authorDto.ImageForDelete)
                    {
                        await documentRepository.Remove(author.ImagePath);
                        var newImage = await documentRepository.Upload("Authors", authorDto.Image);
                    }

                    Context.Update(author);
                    await Context.SaveChangesAsync();

                    var authorRes = await GetById(author.Id);
                    operation.SetSuccess(authorRes.Result);

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
                    var authors = await Context.Authors.Include(author => author.AuthorBooks)
                                                       .Where(author => ids.Contains(author.Id))
                                                       .ToListAsync();
                    authors.ForEach(async author =>
                    {
                        author.DateDeleted = DateTime.UtcNow;
                        await documentRepository.Remove(author.ImagePath);
                        author.AuthorBooks.ToList().ForEach(book => book.DateDeleted = DateTime.UtcNow);
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
