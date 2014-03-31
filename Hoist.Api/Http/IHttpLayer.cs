using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api.Http
{
    public interface IHttpLayer
    {
        ApiResponse Post(string endpoint, string apiKey, string session, string data);
        ApiResponse Get(string endpoint, string apiKey, string session);
        ApiResponse Delete(string endpoint, string apiKey, string session);
    }

}
