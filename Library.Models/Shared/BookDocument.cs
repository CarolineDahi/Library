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
    public class BookDocument : EntityBase
    {
        public Guid BookId { get; set; }
        public Book Book { get; set; }

        public Guid DocumentId { get; set; }
        public Document Document { get; set; }
        public DocumentKind Kind { get; set; }
    }
}
