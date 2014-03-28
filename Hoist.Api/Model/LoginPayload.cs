using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Model
{
    class LoginPayload
    {
         public string Email { get; set; }
            public string Password { get; set; }
            
            public LoginPayload(string email, string password)
            {
                Email = email;
                Password = password;
            }
    }
}
