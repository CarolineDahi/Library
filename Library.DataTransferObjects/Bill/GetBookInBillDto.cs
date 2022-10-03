using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataTransferObjects.Bill
{
    public class GetBookInBillDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public double Quantity { get; set; }
        public string CoverPath { get; set; }
    }
}
