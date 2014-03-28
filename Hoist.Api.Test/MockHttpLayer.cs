using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hoist.Api.Http;

namespace Hoist.Api.Test
{
    public class MockHttpLayer : IHttpLayer
    {

        public List<Tuple<string, string, string, string>> Calls = new List<Tuple<string, string, string, string>>();
        public ApiResponse Response = null;
        public Exception ErrorToThrow = null;

        public ApiResponse Post(string endpoint, string apiKey, string session, string data)
        {
            Calls.Add(new Tuple<string, string, string, string>(endpoint, apiKey, data, session));
            if (ErrorToThrow != null)
            {
                throw ErrorToThrow;
            }
            return Response;
        }

        public ApiResponse Get(string endpoint, string apiKey, string session)
        {
            Calls.Add(new Tuple<string, string, string, string>(endpoint, apiKey, null, session));
            if (ErrorToThrow != null)
            {
                throw ErrorToThrow;
            }
            return Response;
        }

    }
}
