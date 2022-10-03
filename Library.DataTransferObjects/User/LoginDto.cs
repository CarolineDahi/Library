using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataTransferObjects.User
{
    public class LoginDto
    {
        [DefaultValue("caro")]
        public string UserName { get; set; }

        [DefaultValue("1234")]
        public string Password { get; set; }

        [DefaultValue("true")]
        public bool RememberMe { get; set; } = true;

    }
}
