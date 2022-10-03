using Library.SharedKernel.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models.Security
{
    public class User : IdentityUser<Guid>
    {
        #region Base
        public Guid Creater { get; set; }
        public Guid? Modifer { get; set; }
        public Guid? Deleter { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateDeleted { get; set; }
        #endregion

        public string FullName { get; set; }
        public UserType UserType { get; set; }
        public string GenerationStamp { get; set; }
    }
}
