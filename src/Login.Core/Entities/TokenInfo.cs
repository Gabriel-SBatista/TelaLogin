using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Core.Entities
{
    public class TokenInfo
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}
