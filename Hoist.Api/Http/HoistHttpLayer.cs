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

        private HttpWebRequest CreateRequest(string endpoint, string method, string apiKey, string session, string oauthToken, string data)
        {
            logger.Debug("{3} {0} {1} {2}", endpoint, apiKey, session, method);
            var wr = WebRequest.CreateHttp(endpoint);
            wr.CookieContainer = new CookieContainer();
            wr.ServerCertificateValidationCallback = ValidateServerCertificate;
            wr.Headers.Add("Authorization", "Hoist " + apiKey);

            if (session != null)
            {
                var cookiebits = session.Split('=');
                logger.Debug("Adding Session Cookie {0} {1}", cookiebits);
                wr.CookieContainer.Add(new Cookie(cookiebits[0], cookiebits[1], "/", ".hoi.io"));
            }          
           
            if (oauthToken != null)
            {
                logger.Debug("Adding Oauth Cookie");
                wr.Headers.Add("oauth", "TOKEN " + oauthToken);
            }
           
            wr.Method = method;

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
            return wr;
        }

        public ApiResponse Post(string endpoint, string apiKey, string session, string oauthToken,string data)
        {
            return GetResponse(CreateRequest(endpoint, "POST", apiKey, session, oauthToken, data));
        }

        public ApiResponse Put(string endpoint, string apiKey, string session, string oauthToken,string data)
        {
            return GetResponse(CreateRequest(endpoint, "PUT", apiKey, session, oauthToken, data));
        }

        public ApiResponse Get(string endpoint, string apiKey, string session, string oauthToken)
        {
            
            return GetResponse(CreateRequest(endpoint, "GET", apiKey, session, oauthToken, null ));
        }

        public ApiResponse Delete(string endpoint, string apiKey, string session, string oauthToken)
        {
            return GetResponse(CreateRequest(endpoint, "DELETE", apiKey, session, oauthToken, null));
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
        
        
    }
}
