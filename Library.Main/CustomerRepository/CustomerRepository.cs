using Library.Base;
using Library.DataTransferObjects.Customer;
using Library.Models.Main;
using Library.SharedKernel.OperationResult;
using Library.SQL.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Main.CustomerRepository
{
    public class CustomerRepository : LibraryRepository, ICustomerRepository
    {
        public CustomerRepository(LibraryDBContext context) : base(context)
        {
        }

        public async Task<OperationResult<IEnumerable<GetCustomerDto>>> GetAll()
        {
            var operation = new OperationResult<IEnumerable<GetCustomerDto>>();
            try
            {
                var customers = await Context.Customers.Select(customer => new GetCustomerDto
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Phone = customer.Phone,
                }).ToListAsync();

                operation.SetSuccess(customers);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetCustomerDto>> GetById(Guid id)
        {
            var operation = new OperationResult<GetCustomerDto>();
            try
            {
                var customer = await Context.Customers.Where(customer => customer.Id.Equals(id))
                                                      .Select(customer => new GetCustomerDto
                                                      {
                                                          Id = customer.Id,
                                                          FirstName = customer.FirstName,
                                                          LastName = customer.LastName,
                                                          Phone = customer.Phone,
                                                      }).SingleOrDefaultAsync();

                operation.SetSuccess(customer);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetCustomerDto>> Create(SetCustomerDto customerDto)
        {
            var operation = new OperationResult<GetCustomerDto>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var customer = new Customer()
                    {
                        FirstName = customerDto.FirstName,
                        LastName = customerDto.LastName,
                        Phone = customerDto.Phone,
                    };
                    Context.Add(customer);
                    await Context.SaveChangesAsync();

                    var customerRes = await GetById(customer.Id);
                    operation.SetSuccess(customerRes.Result);

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

        public async Task<OperationResult<GetCustomerDto>> Update(UpdateCustomerDto customerDto)
        {
            var operation = new OperationResult<GetCustomerDto>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var customer = await Context.Customers.Where(customer => customerDto.Id.Equals(customer.Id)).SingleOrDefaultAsync();

                    if (customer is null)
                    {
                        operation.SetFailed($"this customer with {customer.Id} id not found.");
                    }

                    customer.FirstName = customerDto.FirstName;
                    customer.LastName = customerDto.LastName;
                    customer.Phone = customerDto.Phone;

                    Context.Update(customer);
                    await Context.SaveChangesAsync();

                    var customerRes = await GetById(customer.Id);
                    operation.SetSuccess(customerRes.Result);

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
                    var customers = await Context.Customers.Where(book => ids.Contains(book.Id)).ToListAsync();
                    customers.ForEach(customer =>
                    {
                        customer.DateDeleted = DateTime.UtcNow;
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
