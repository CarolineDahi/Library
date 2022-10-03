using Library.Models.Base;
using Library.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.Main
{
    public class BookCategory : EntityBase
    {
        public Guid BookId { get; set; }
        public Book Book { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
