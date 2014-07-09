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
            public string oauth = null;
            public string environment = null;

            public static HttpCall GET(string endpoint, string apiKey, string session)
            {
                return new HttpCall() { endpoint = endpoint, apiKey = apiKey, session = session, method = "GET" };
            }

            public static HttpCall GET(string endpoint, string apiKey, string session, string oauth)
            {
                return new HttpCall() { endpoint = endpoint, apiKey = apiKey, session = session, method = "GET", oauth = oauth };
            }

            public static HttpCall POST(string endpoint, string apiKey, string session, string data)
            {
                return new HttpCall() { endpoint = endpoint, apiKey = apiKey, session = session, method = "POST", data = data };
            }

            public static HttpCall POST(string endpoint, string apiKey, string session, string data, string oauth)
            {
                return new HttpCall() { endpoint = endpoint, apiKey = apiKey, session = session, method = "POST", data = data, oauth = oauth };
            }

            public static HttpCall PUT(string endpoint, string apiKey, string session, string data)
            {
                return new HttpCall() { endpoint = endpoint, apiKey = apiKey, session = session, method = "PUT", data = data};
            }

            public static HttpCall PUT(string endpoint, string apiKey, string session, string data, string oauth)
            {
                return new HttpCall() { endpoint = endpoint, apiKey = apiKey, session = session, method = "PUT", data = data, oauth = oauth };
            }

            public static HttpCall DELETE(string endpoint, string apiKey, string session)
            {
                return new HttpCall() { endpoint = endpoint, apiKey = apiKey, session = session, method = "DELETE"};
            }

            public static HttpCall DELETE(string endpoint, string apiKey, string session, string oauth)
            {
                return new HttpCall() { endpoint = endpoint, apiKey = apiKey, session = session, method = "DELETE", oauth = oauth };
            }
        }

        public List<HttpCall> Calls = new List<HttpCall>();

        public ApiResponse Response = null;
        public Exception ErrorToThrow = null;

        public ApiResponse Post(string endpoint, string apiKey, string session, string oauth, string data)
        {
            Calls.Add(HttpCall.POST(endpoint, apiKey, session, data, oauth));
            
            if (ErrorToThrow != null)
            {
                throw ErrorToThrow;
            }
            return Response;
        }

        public ApiResponse Put(string endpoint, string apiKey, string session, string oauth, string data)
        {
            Calls.Add(HttpCall.PUT(endpoint, apiKey, session, data, oauth));

            if (ErrorToThrow != null)
            {
                throw ErrorToThrow;
            }
            return Response;
        }

        public ApiResponse Get(string endpoint, string apiKey, string session, string oauth)
        {
            Calls.Add(HttpCall.GET(endpoint, apiKey, session, oauth));
            if (ErrorToThrow != null)
            {
                throw ErrorToThrow;
            }
            return Response;
        }

        public ApiResponse Delete(string endpoint, string apiKey, string session, string oauth)
        {
            Calls.Add(HttpCall.DELETE(endpoint, apiKey, session, oauth));
            if (ErrorToThrow != null)
            {
                throw ErrorToThrow;
            }
            return Response;
        }

        

    }
}
