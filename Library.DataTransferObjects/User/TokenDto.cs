using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataTransferObjects.User
{
    public class TokenDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
