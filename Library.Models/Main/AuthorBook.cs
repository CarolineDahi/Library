using Library.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.Main
{
    public class AuthorBook : EntityBase
    {
        public Guid AuthorId { get; set; }
        public Author Author { get; set; }

        public Guid BookId { get; set; }
        public Book Book { get; set; }
    }
}
