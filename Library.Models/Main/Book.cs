using Library.Models.Base;
using Library.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.Main
{
    public class Book : EntityBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime ReleaseDate {get; set; }

        public Guid? PublishingHouseId { get; set; }
        public PublishingHouse PublishingHouse { get; set; }

        public ICollection<AuthorBook> AuthorBooks { get; set; }
        public ICollection<BookCategory> BookCategories { get; set; }
        public ICollection<BillBook> BillBooks { get; set; }
        public ICollection<BookDocument> BookDocuments { get; set; }
    }
}
