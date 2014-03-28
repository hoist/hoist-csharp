using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Model
{
    public class HoistUser
    {
        public string Role { get; set; }
        public string Id { get; set; }

        public override string ToString()
        {
            return String.Format("{{'Role':{0}, 'Id'{1}}}", Role, Id);
        }
    }
}
