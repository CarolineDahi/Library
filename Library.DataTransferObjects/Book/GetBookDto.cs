using Library.DataTransferObjects.Author;
using Library.DataTransferObjects.Category;
using Library.DataTransferObjects.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataTransferObjects.Book
{
    public class GetBookDto : BaseBookDto
    {
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Guid PublishingHouseId { get; set; }
        public string PublishingHouseName { get; set; }
        public double Quantity { get; set; }
        public List<BaseAuthorDto> Authors { get; set; }
        public List<GetCategoryDto> Categories { get; set; }
        public List<GetDocumentDto> Documents { get; set; }
    }
}
