using Library.Base;
using Library.DataTransferObjects.Bill;
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

namespace Library.Main.BillRepository
{
    public class BillRepository : LibraryRepository, IBillRepository
    {
        public BillRepository(LibraryDBContext context) : base(context)
        {
        }

        public async Task<OperationResult<IEnumerable<GetBillDto>>> GetAll(BillFilterDto? filterDto)
        {
            var operation = new OperationResult<IEnumerable<GetBillDto>>();
            try
            {
                var bills = await Context.Bills.Where(b => filterDto != null ?
                                                        (  (filterDto.StartDate.HasValue ? b.DateCreated >= filterDto.StartDate.Value
                                                                                         : true)
                                                        && (filterDto.EndDate.HasValue ? b.DateCreated <= filterDto.EndDate.Value
                                                                                         : true)
                                                        && (filterDto.BillType.HasValue ? b.Type == filterDto.BillType
                                                                                        : true)
                                                        && (filterDto.CustomerId.HasValue ? b.CustomerId == filterDto.CustomerId
                                                                                           : true)
                                                        && (filterDto.BookId.HasValue ? b.BillBooks.Select(b => b.BookId).ToList().Contains(filterDto.BookId.Value)
                                                                                      : true)
                                                        )
                                                        : true)
                                               .Select(b => new GetBillDto
                                               {
                                                   Id = b.Id,
                                                   Number = b.Number,
                                                   Type = b.Type,
                                                   BillDate = b.DateCreated,
                                                   Quantity = b.BillBooks.Sum(bb => bb.Quantity),
                                                   CustomerId = b.CustomerId,
                                                   CustomerName = b.Customer.FirstName + " " + b.Customer.LastName,
                                               }).ToListAsync();

                operation.SetSuccess(bills);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetBillDetailsDto>> GetById(Guid id)
        {
            var operation = new OperationResult<GetBillDetailsDto>();
            try
            {
                var bill = await Context.Bills.Where(b => b.Id.Equals(id))
                                              .Select(b => new GetBillDetailsDto
                                              {
                                                  Id = b.Id,
                                                  Number = b.Number,
                                                  Type = b.Type,
                                                  BillDate = b.DateCreated,
                                                  Quantity = b.BillBooks.Sum(bb => bb.Quantity),
                                                  CustomerId = b.CustomerId,
                                                  CustomerName = b.Customer.FirstName + " " + b.Customer.LastName,
                                                  Books = b.BillBooks.Where(billBook => !billBook.DateDeleted.HasValue)
                                                                     .Select(billBook => new GetBookInBillDto
                                                                     {
                                                                         Id = billBook.BookId,
                                                                         Title = billBook.Book.Title,
                                                                         CoverPath = billBook.Book.BookDocuments
                                                                                                  .Where(doc => !doc.DateDeleted.HasValue
                                                                                                             && doc.Kind == DocumentKind.CoverBook)
                                                                                                  .Select(doc => doc.Document.Path)
                                                                                                  .FirstOrDefault(),
                                                                         Quantity = billBook.Quantity
                                                                     }).ToList()
                                              }).SingleOrDefaultAsync();

                operation.SetSuccess(bill);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetBillDto>> Create(SetBillDto billDto)
        {
            var operation = new OperationResult<GetBillDto>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var bill = new Bill
                    {
                        Type = billDto.Type,
                        Number = await _generateNumber(billDto.Type),
                        CustomerId = billDto.CustomerId,
                    };
                    Context.Add(bill);

                    billDto.Books.ToList().ForEach(book =>
                    Context.Add(new BillBook
                    {
                        BillId = bill.Id,
                        BookId = book.Id,
                        Quantity = book.Quantity,
                    }));

                    await Context.SaveChangesAsync();
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
                    var bills = await Context.Bills.Where(bill => ids.Contains(bill.Id)).ToListAsync();

                    bills.ForEach(bill =>
                    {
                        bill.DateDeleted = DateTime.UtcNow;
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

        #region - Helper Methods -
        private async Task<string> _generateNumber(BillType type)
        {
            string num = "";
            num += (type == BillType.Import ? "I" : "O");
            num += "-";
            var lastBill = await Context.Bills.Where(b => b.Type == type)
                                                  .OrderByDescending(r => r.DateCreated)
                                                  .FirstOrDefaultAsync();
            if (lastBill == null)
                num += 1.ToString("D5");
            else
            {
                var newNum = Int32.Parse(lastBill.Number.Skip(2).ToString()) + 1;
                num += newNum.ToString("D5");
            }
            return num;
        }
        #endregion
    }
}
