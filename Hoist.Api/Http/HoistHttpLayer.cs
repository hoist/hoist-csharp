using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

using Hoist.Api.Exceptions;
using Hoist.Api.Logging;

namespace Hoist.Api.Http
{
    class HoistHttpLayer : IHttpLayer
    {
        private static ILogger logger = LogManager.GetLogger(typeof(HoistHttpLayer));
        
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            logger.Error("Certificate error: {0}", sslPolicyErrors.ToString());

            // Do not allow this client to communicate with unauthenticated servers. 
            return false;
        }

        public ApiResponse Post(string endpoint, string apiKey, string session, string data)
        {
            logger.Debug("{0} {1} {2} {3}", endpoint, apiKey, session, data);  //TODO: Security Leak!!!
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

            if (data != null)
            {
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] byte1 = encoding.GetBytes(data);
                wr.ContentLength = byte1.Length;
                var newStream = wr.GetRequestStream();
                newStream.Write(byte1, 0, byte1.Length);
                newStream.Close();
            }

            var retval = GetResponse(wr);
            return retval;

        }

        public ApiResponse Get(string endpoint, string apiKey, string session)
        {
            logger.Debug("{0} {1} {2}", endpoint, apiKey, session);
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
                
                response = (HttpWebResponse)ex.Response;
                if (response == null)
                {
                    logger.Error("No response was returned: {0}", ex.ToString());
                    //Something really bad has happend here!
                    throw new NoResponseException(ex); 
                }
            }
            var retval = new ApiResponse();
            retval.Code = (int)response.StatusCode;
            retval.Description = response.StatusDescription;
            retval.WithWWWAuthenticate = response.Headers["WWW-Authenticate"] != null;

            if (response.Cookies.Count > 0)
            {
                foreach (Cookie cookie in response.Cookies)
                {
                    logger.Debug("Auth Cookie: {0}", cookie.Name + "=" + cookie.Value);
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
        
        public ApiResponse Delete(string endpoint, string apiKey, string session)
        {
            logger.Debug("{0} {1} {2}", endpoint, apiKey, session);
            var wr = WebRequest.CreateHttp(endpoint);
            wr.ServerCertificateValidationCallback = ValidateServerCertificate;
            wr.Headers.Add("Authorization", "Hoist " + apiKey);
            if (session != null)
            {
                wr.Headers.Add("Cookie", session);
            }
            wr.Method = "DELETE";
            wr.ContentType = "application/json";

            var retval = GetResponse(wr);
            return retval;
        }
    }
}
