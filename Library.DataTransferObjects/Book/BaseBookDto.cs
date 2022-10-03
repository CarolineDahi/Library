using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataTransferObjects.Book
{
    public class BaseBookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CoverPath { get; set; }
    }
}
