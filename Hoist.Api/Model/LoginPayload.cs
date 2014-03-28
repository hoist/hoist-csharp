using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Model
{
    class LoginPayload
    {
        public string email { get; set; }
        public string password { get; set; }

        public LoginPayload(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }
}
