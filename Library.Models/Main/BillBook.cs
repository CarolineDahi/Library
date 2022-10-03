using Library.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.Main
{
    public class BillBook : EntityBase
    {
        public Guid BookId { get; set; }
        public Book Book { get; set; }

        public Guid BillId { get; set; }
        public Bill Bill { get; set; }

        public double Quantity { get; set; }
    }
}
