using Library.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.Main
{
    public class Customer : EntityBase
    {
        public string  FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public ICollection<Bill> Bills { get; set; }
    }
}
