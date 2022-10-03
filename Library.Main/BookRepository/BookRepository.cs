using Library.Base;
using Library.DataTransferObjects.Author;
using Library.DataTransferObjects.Book;
using Library.DataTransferObjects.Category;
using Library.DataTransferObjects.Document;
using Library.Models.Main;
using Library.Models.Shared;
using Library.Shared.DocumentRepository;
using Library.SharedKernel.Enums;
using Library.SharedKernel.OperationResult;
using Library.SQL.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Main.BookRepository
{
    public class BookRepository : LibraryRepository, IBookRepository
    {
        private readonly IDocumentRepository documentRepository;

        public BookRepository(LibraryDBContext context, IDocumentRepository documentRepository) : base(context)
        {
            this.documentRepository = documentRepository;
        }

        public async Task<OperationResult<IEnumerable<GetBookDto>>> GetAll()
        {
            var operation = new OperationResult<IEnumerable<GetBookDto>>();
            try
            {
                var books = await Context.Books.Select(book => new GetBookDto
                {
                    Id = book.Id,
                    Description = book.Description,
                    Title = book.Title,
                    Price = book.Price,
                    ReleaseDate = book.ReleaseDate,
                    PublishingHouseId = book.PublishingHouseId.Value,
                    PublishingHouseName = book.PublishingHouse.Name,
                }).ToListAsync();

                operation.SetSuccess(books);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetBookDto>> GetById(Guid id)
        {
            var operation = new OperationResult<GetBookDto>();
            try
            {
                var book = await Context.Books.Where(book => book.Id.Equals(id))
                                                  .Select(book => new GetBookDto
                                                  {
                                                      Id = book.Id,
                                                      Description = book.Description,
                                                      Title = book.Title,
                                                      Price = book.Price,
                                                      ReleaseDate = book.ReleaseDate,
                                                      CoverPath = book.BookDocuments.Where(bookDoc => !bookDoc.DateDeleted.HasValue
                                                                                                   && bookDoc.Kind == DocumentKind.CoverBook)
                                                                                    .Select(bookDoc => bookDoc.Document.Path)
                                                                                    .FirstOrDefault(),
                                                      PublishingHouseId = book.PublishingHouseId.Value,
                                                      PublishingHouseName = book.PublishingHouse.Name,
                                                      Authors = book.AuthorBooks.Where(authorBook => !authorBook.DateDeleted.HasValue)
                                                                                .Select(autherBook => new BaseAuthorDto
                                                                                {
                                                                                    Id = autherBook.BookId,
                                                                                    Name = autherBook.Book.Title
                                                                                }).ToList(),
                                                      Categories = book.BookCategories.Where(bookCat => !bookCat.DateDeleted.HasValue)
                                                                                      .Select(bookCat => new GetCategoryDto
                                                                                      {
                                                                                          Id = bookCat.CategoryId,
                                                                                          Name = bookCat.Category.Name,
                                                                                      }).ToList(),
                                                      Documents = book.BookDocuments.Where(bookDoc => !bookDoc.DateDeleted.HasValue
                                                                                                   && bookDoc.Kind != DocumentKind.CoverBook)
                                                                                    .Select(bookDoc => new GetDocumentDto
                                                                                    {
                                                                                        Id = bookDoc.DocumentId,
                                                                                        Path = bookDoc.Document.Path,
                                                                                        Name = bookDoc.Document.Name,
                                                                                        Type = bookDoc.Document.Type,
                                                                                    }).ToList(),
                                                      Quantity = book.BillBooks.Where(billbook => !billbook.DateDeleted.HasValue)
                                                                               .Sum(billbook => Math.Sign((int)billbook.Bill.Type) * billbook.Quantity),
                                                  }).SingleOrDefaultAsync();

                operation.SetSuccess(book);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetBookDto>> Create(SetBookDto bookDto)
        {
            var operation = new OperationResult<GetBookDto>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var book = new Book
                    {
                        Title = bookDto.Title,
                        Description = bookDto.Description,
                        Price = bookDto.Price,
                        PublishingHouseId = bookDto.PublishingHouseId,
                        ReleaseDate = bookDto.ReleaseDate,
                        AuthorBooks = bookDto.AuthorIds.Select(id => new AuthorBook()
                        {
                            AuthorId = id
                        }).ToList(),
                        BookCategories = bookDto.CategoryIds.Select(id => new BookCategory()
                        {
                            CategoryId = id
                        }).ToList(),
                    };

                    Context.Add(book);
                    await Context.SaveChangesAsync();

                    List<BookDocument> bookDocs = new();
                    if(bookDto.Cover is not null)
                    {
                        var cover = await documentRepository.Add("Books", new List<IFormFile> { bookDto.Cover });
                        bookDocs.Add(new()
                        {
                            BookId = book.Id,
                            DocumentId = cover.Result.FirstOrDefault().Id,
                            Kind = DocumentKind.CoverBook,
                        });
                    }

                    if(bookDto.Documents is not null)
                    {
                        var documents = await documentRepository.Add("Books", bookDto.Documents.ToList());
                        documents.Result.ToList().ForEach(document => bookDocs.Add(new()
                        {
                            BookId = book.Id,
                            DocumentId = document.Id,
                            Kind = DocumentKind.Book
                        }));
                    }

                    Context.AddRange(bookDocs);
                    await Context.SaveChangesAsync();

                    var bookRes = await GetById(book.Id);
                    operation.SetSuccess(bookRes.Result);

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
        public async Task<OperationResult<GetBookDto>> Update(UpdateBookDto bookDto)
        {
            var operation = new OperationResult<GetBookDto>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var book = await Context.Books.Where(book => bookDto.Id.Equals(book.Id)).SingleOrDefaultAsync();

                    if (book is null)
                    {
                        operation.SetFailed($"this book with {book.Id} id not found.");
                    }

                    book.Title = bookDto.Title;
                    book.Description = bookDto.Description;
                    book.ReleaseDate = bookDto.ReleaseDate;
                    book.Price = bookDto.Price;
                    book.PublishingHouseId = bookDto.PublishingHouseId;

                    //ToDo
                    
                    Context.Update(book);
                    await Context.SaveChangesAsync();

                    var authorRes = await GetById(book.Id);
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
                    var books = await Context.Books.Include(book => book.BookCategories)
                                                   .Include(book => book.BookDocuments)
                                                   .ThenInclude(bookDoc => bookDoc.Document)
                                                   .Include(book => book.AuthorBooks)
                                                   .Where(book => ids.Contains(book.Id))
                                                   .ToListAsync();
                    books.ForEach(async book =>
                    {
                        book.DateDeleted = DateTime.UtcNow;
                        await documentRepository.Delete(book.BookDocuments.Select(b => b.DocumentId).ToList());
                        book.BookDocuments.ToList().ForEach(d => d.DateDeleted = DateTime.UtcNow); 
                        book.AuthorBooks.ToList().ForEach(a => a.DateDeleted = DateTime.UtcNow);
                        book.BookCategories.ToList().ForEach(c => c.DateDeleted = DateTime.UtcNow);
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
