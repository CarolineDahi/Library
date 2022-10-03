using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataTransferObjects.Bill
{
    public class GetBillDetailsDto : GetBillDto
    {
        public IEnumerable<GetBookInBillDto> Books { get; set; }
    }
}
