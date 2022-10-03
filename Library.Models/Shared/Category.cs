using Library.Models.Base;
using Library.Models.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.Shared
{
    public class Category : EntityBase
    {
        public string Name { get; set; }

        public ICollection<BookCategory> BookCategories { get; set; }
    }
}
