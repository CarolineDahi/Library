using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataTransferObjects.Book
{
    public class SetBookDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public IFormFile Cover { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Guid PublishingHouseId { get; set; }
        public IEnumerable<IFormFile> Documents { get; set; }
        public IEnumerable<Guid> AuthorIds { get; set; }
        public IEnumerable<Guid> CategoryIds { get; set; }
    }
}
