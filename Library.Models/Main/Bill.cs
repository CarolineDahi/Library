using Library.Models.Base;
using Library.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.Main
{
    public class Bill : EntityBase
    {
        public string Number { get; set; }
        public BillType Type { get; set; }

        public Guid? CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<BillBook> BillBooks { get; set; }
    }
}
