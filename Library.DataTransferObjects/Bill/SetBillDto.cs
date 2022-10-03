using Library.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataTransferObjects.Bill
{
    public class SetBillDto
    {
        public BillType Type { get; set; }
        public Guid? CustomerId { get; set; }

        public IEnumerable<SetBookInBillDto> Books { get; set; }
    }
}
