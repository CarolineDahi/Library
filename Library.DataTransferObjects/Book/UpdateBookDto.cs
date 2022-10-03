using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataTransferObjects.Book
{
    public class UpdateBookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public IFormFile? CoverPath { get; set; }
        public bool CoverForDelete { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Guid PublishingHouseId { get; set; }
        
    }
}
