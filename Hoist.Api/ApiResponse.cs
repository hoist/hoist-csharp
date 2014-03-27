using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api
{
    public class ApiResponse
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public bool WithWWWAuthenticate { get; set; }
        public string HoistSession { get; set; }
        public IJsonObject Payload { get; set; }
    }
}
