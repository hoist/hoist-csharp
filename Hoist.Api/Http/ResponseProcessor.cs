using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Hoist.Api.Exceptions;
using Hoist.Api.Model;

namespace Hoist.Api.Http
{
    public class ResponseProcessor
    {
        private JavaScriptSerializer Serialiser { get; set; }

        public ResponseProcessor()
        {
            Serialiser = new JavaScriptSerializer();
            RegisterConverter(new HoistModelJavaScriptConverter());
        }

        public void RegisterConverter( JavaScriptConverter convertor ) {
            Serialiser.RegisterConverters(new List<JavaScriptConverter>() { convertor });
        }

        public string ProcessResponse(ApiResponse response, bool ignore404 = false, bool ignore401 = true)
        {
            //Throw Exceptions on bad responses otherwise just return self..
            if (response.Code == 200)
            {
                return response.Payload;
            }
            else if (response.Code == 401 && response.WithWWWAuthenticate)
            {
                throw new BadApiKeyException();
            }
            else if (response.Code == 401)
            {
                if (ignore401) { return null; }
                else
                {
                    throw new UnauthorisedException();
                }
            }
            else if (response.Code == 403)
            {
                throw new UnauthorisedException();
            }
            else if (response.Code == 404)
            {
                if (ignore404) { return null; }
                else
                {
                    throw new NotFoundException();
                }
            }
            else if (response.Code == 409)
            {
                throw new DataConflictException();
            }
            else
            {
                throw new UnexpectedResponseException(response);
            }
        }

        public T ProcessHoistData<T>(ApiResponse response, bool ignore404 = false, bool ignore401 = true) where T : class
        {
            var payload = ProcessResponse(response,ignore404, ignore401);
            return payload != null ? Serialiser.Deserialize<T>(payload) : null;
        }

        internal string ToHoist(object data)
        {
            return data != null ? Serialiser.Serialize(data) : null;
        }
    }
}
