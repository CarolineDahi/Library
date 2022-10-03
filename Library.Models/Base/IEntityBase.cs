using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.Base
{
    public interface IEntityBase
    {
        public Guid Id { get; set; }
        public Guid CreatedId { get; set; }
        public Guid? UpdatedId { get; set; }
        public Guid? DeletedId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateDeleted{ get; set; }
    }
}
