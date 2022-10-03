using Library.Models.Base;
using Library.Models.Main;
using Library.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.Shared
{
    public class Document : EntityBase
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public DocumentType Type { get; set; }

        public ICollection<BookDocument> BookDocuments { get; set; }
    }
}
