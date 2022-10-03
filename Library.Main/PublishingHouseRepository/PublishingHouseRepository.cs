using Library.Base;
using Library.DataTransferObjects.Book;
using Library.DataTransferObjects.PublishingHouse;
using Library.Models.Main;
using Library.SharedKernel.Enums;
using Library.SharedKernel.OperationResult;
using Library.SQL.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Main.PublishingHouseRepository
{
    public class PublishingHouseRepository : LibraryRepository, IPublishingHouserepository
    {
        public PublishingHouseRepository(LibraryDBContext context) : base(context)
        {
        }

        public async Task<OperationResult<IEnumerable<GetPublishingHouseDto>>> GetAll()
        {
            var operation = new OperationResult<IEnumerable<GetPublishingHouseDto>>();
            try
            {
                var publishHouses = await Context.PublishingHouses.Where(house => !house.DateDeleted.HasValue)
                                                                  .Select(house => new GetPublishingHouseDto
                                                                  {
                                                                      Id = house.Id,
                                                                      Name = house.Name,
                                                                      Phone = house.Phone,
                                                                      Address = house.Address,
                                                                  }).ToListAsync();
                operation.SetSuccess(publishHouses);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetPublishingHouseDto>> GetById(Guid id)
        {
            var operation = new OperationResult<GetPublishingHouseDto>();
            try
            {
                var publishHouse = await Context.PublishingHouses.Where(house => !house.DateDeleted.HasValue
                                                                               && house.Id == id)
                                                                 .Select(house => new GetPublishingHouseDto
                                                                 {
                                                                     Id = house.Id,
                                                                     Name = house.Name,
                                                                     Phone = house.Phone,
                                                                     Address = house.Address,
                                                                     Books = house.Books.Where(book => !book.DateDeleted.HasValue)
                                                                                        .Select(book => new BaseBookDto
                                                                                        {
                                                                                            Id = book.Id,
                                                                                            Title = book.Title,
                                                                                            CoverPath = book.BookDocuments.Where(bookDoc => !bookDoc.DateDeleted.HasValue
                                                                                                                                         && bookDoc.Kind == DocumentKind.CoverBook)
                                                                                                                          .Select(bookDoc => bookDoc.Document.Path)
                                                                                                                          .FirstOrDefault(),
                                                                                        }).ToList()
                                                                 }).SingleOrDefaultAsync();
                operation.SetSuccess(publishHouse);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetPublishingHouseDto>> Create(SetPublishingHouseDto categoryDto)
        {
            var operation = new OperationResult<GetPublishingHouseDto>();
            using(var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var publishingHouse = new PublishingHouse
                    {
                        Name = categoryDto.Name,
                        Address = categoryDto.Address,
                        Phone = categoryDto.Phone,
                    };
                    Context.Add(publishingHouse);
                    await Context.SaveChangesAsync();

                    var publishingHouseDto = await GetById(publishingHouse.Id);
                    operation.SetSuccess(publishingHouseDto.Result);

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

        public async Task<OperationResult<GetPublishingHouseDto>> Update(UpdatePublishingHouseDto publishingHouseDto)
        {
            var operation = new OperationResult<GetPublishingHouseDto>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var publishingHouse = await Context.PublishingHouses.Where(house => !house.DateDeleted.HasValue
                                                                                     && house.Id == publishingHouseDto.Id)
                                                                        .SingleOrDefaultAsync();
                    publishingHouse.Name = publishingHouseDto.Name;
                    publishingHouse.Address = publishingHouseDto.Address;
                    publishingHouse.Phone = publishingHouseDto.Phone;

                    Context.Update(publishingHouse);
                    await Context.SaveChangesAsync();
                    transaction.Commit();

                    var publishingDto = await GetById(publishingHouseDto.Id);
                    operation.SetSuccess(publishingDto.Result);
                }
                catch (Exception ex)
                {
                    operation.SetException(ex);
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
                    var publishHouses = await Context.PublishingHouses.Include(house => house.Books)
                                                                      .Where(house => ids.Contains(house.Id))
                                                                      .ToListAsync();
                    if(publishHouses.Any(p => p.Books.Any(b => !b.DateDeleted.HasValue)))
                    {
                        return operation.SetFailed("There are books in this PublishingHouse");
                    }
                    publishHouses.ForEach(house =>
                    {
                        house.DateDeleted = DateTime.UtcNow;
                    });
                    await Context.SaveChangesAsync();
                    operation.SetSuccess(true);
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

    }
}
