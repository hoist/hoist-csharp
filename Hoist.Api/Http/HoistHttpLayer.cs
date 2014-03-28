using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Hoist.Api.Http
{
    class HoistHttpLayer : IHttpLayer
    {
        public ApiResponse Post(string endpoint, string apiKey, string session, string data)
        {
            var wr = WebRequest.CreateHttp(endpoint);
            wr.Headers.Add("Authorization", "Hoist " + apiKey);
            if (session != null)
            {
                wr.Headers.Add("Cookie", session);
            }
            wr.Method = "POST";
            wr.ContentType = "application/json";
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(data);
            wr.ContentLength = byte1.Length;
            var newStream = wr.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);
            newStream.Close();

            var retval = GetResponse(wr);
            return retval;
                       
        }

        public ApiResponse Get(string endpoint, string apiKey, string session)
        {
            var wr = WebRequest.CreateHttp(endpoint);
            wr.Headers.Add("Authorization", "Hoist " + apiKey);
            if (session != null)
            {
                wr.Headers.Add("Cookie", session);
            }
            wr.Method = "GET";
            wr.ContentType = "application/json";

            var retval = GetResponse(wr);
            return retval;
        }

        private static ApiResponse GetResponse(HttpWebRequest wr)
        {
            var response = (HttpWebResponse)wr.GetResponse();
            var retval = new ApiResponse();
            retval.Code = (int)response.StatusCode;
            retval.Description = response.StatusDescription;
            retval.WithWWWAuthenticate = response.Headers["WWW-Authenticate"] != null;
            if (response.Cookies.Count > 0)
            {
                foreach (Cookie cookie in response.Cookies)
                {
                    if (cookie.Name.StartsWith("hoist-session-"))
                    {
                        retval.HoistSession = cookie.Name + "=" + cookie.Value;
                    }
                }
            }
            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            retval.Payload = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();
            return retval;
        }

        
    }
}
