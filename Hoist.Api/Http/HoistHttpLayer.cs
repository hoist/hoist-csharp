using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Hoist.Api.Http
{
    class HoistHttpLayer : IHttpLayer
    {
        
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                Console.WriteLine("Certificate Passed");
                return true;
            }

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers. 
            return false;
        }

        public ApiResponse Post(string endpoint, string apiKey, string session, string data)
        {
            Console.WriteLine("{0} {1} {2} {3}", endpoint, apiKey, session, data);
            var wr = WebRequest.CreateHttp(endpoint);
            wr.CookieContainer = new CookieContainer();
            wr.ServerCertificateValidationCallback = ValidateServerCertificate;
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
            wr.ServerCertificateValidationCallback = ValidateServerCertificate;
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
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)wr.GetResponse();
            }
            catch (WebException ex)
            {
                //Console.WriteLine(ex);
                response = (HttpWebResponse)ex.Response;
            }
            var retval = new ApiResponse();
            retval.Code = (int)response.StatusCode;
            retval.Description = response.StatusDescription;
            retval.WithWWWAuthenticate = response.Headers["WWW-Authenticate"] != null;

            //Console.WriteLine(response.Headers["Set-Cookie"]);

            if (response.Cookies.Count > 0)
            {
                foreach (Cookie cookie in response.Cookies)
                {
                    Console.WriteLine(cookie.Name + "=" + cookie.Value);
                    if (cookie.Name.StartsWith("hoist-session"))
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
