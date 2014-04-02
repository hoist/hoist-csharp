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
        public class HttpCall
        {
            public string endpoint = null;
            public string apiKey = null;
            public string session = null;
            public string data = null;
            public string method = null;

            public static HttpCall GET(string endpoint, string apiKey, string session)
            {
                return new HttpCall() { endpoint = endpoint, apiKey = apiKey, session = session, method = "GET" };
            }

            public static HttpCall POST(string endpoint, string apiKey, string session, string data)
            {
                return new HttpCall() { endpoint = endpoint, apiKey = apiKey, session = session, method = "POST", data = data };
            }

            public static HttpCall DELETE(string endpoint, string apiKey, string session)
            {
                return new HttpCall() { endpoint = endpoint, apiKey = apiKey, session = session, method = "DELETE" };
            }
        }

        public List<HttpCall> Calls = new List<HttpCall>();

        public ApiResponse Response = null;
        public Exception ErrorToThrow = null;

        public ApiResponse Post(string endpoint, string apiKey, string session, string data)
        {
            Calls.Add(HttpCall.POST(endpoint, apiKey, session, data));
            
            if (ErrorToThrow != null)
            {
                throw ErrorToThrow;
            }
            return Response;
        }

        public ApiResponse Get(string endpoint, string apiKey, string session)
        {
            Calls.Add(HttpCall.GET(endpoint, apiKey, session));
            if (ErrorToThrow != null)
            {
                throw ErrorToThrow;
            }
            return Response;
        }

        public ApiResponse Delete(string endpoint, string apiKey, string session)
        {
            Calls.Add(HttpCall.DELETE(endpoint, apiKey, session));
            if (ErrorToThrow != null)
            {
                throw ErrorToThrow;
            }
            return Response;
        }

    }
}
