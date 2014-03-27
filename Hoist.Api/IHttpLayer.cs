using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoist.Api
{
    public interface IHttpLayer
    {
        ApiResponse Post(string endpoint, string apiKey, IJsonObject data);
    }

    public interface IJsonObject {
        string Get(string key);
        //string Set(string key, string value);
        //string ToPayload();
    }
}
