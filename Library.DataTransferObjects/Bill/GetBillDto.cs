using Library.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataTransferObjects.Bill
{
    public class GetBillDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public BillType Type { get; set; }
        public DateTime BillDate { get; set; }
        public double Quantity { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
