using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataTransferObjects.Author
{
    public class BaseAuthorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
