using Library.DataTransferObjects.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataTransferObjects.PublishingHouse
{
    public class GetPublishingHouseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public List<BaseBookDto> Books { get; set; }
    }
}
