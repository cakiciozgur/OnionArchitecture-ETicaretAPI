using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.DTOs.User
{
    public class LoginUserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Token Token { get; set; }
    }
}
