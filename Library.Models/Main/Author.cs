using Library.Models.Base;
using Library.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.Main
{
    public class Author : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int Age { get; set; }

        public string ImagePath { get; set; }

        public ICollection<AuthorBook> AuthorBooks { get; set; }
    }
}
