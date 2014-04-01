using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Hoist.Api.Exceptions;
using Hoist.Api.Model;

namespace Hoist.Api.Http
{
    class ResponseProcessor
    {
        private JavaScriptSerializer Serialiser { get; set; }

        public ResponseProcessor()
        {
            Serialiser = new JavaScriptSerializer();
            Serialiser.RegisterConverters(new List<JavaScriptConverter>() { new HoistModelJavaScriptConverter() });
        }

        public string ProcessResponse(ApiResponse response)
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
                return null;
            }
            else if (response.Code == 404)
            {
                throw new NotFoundException();
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

        public T ProcessHoistData<T>(ApiResponse response) where T : class
        {
            var payload = ProcessResponse(response);
            return payload != null ? Serialiser.Deserialize<T>(payload) : null;
        }

        internal string ToHoist(object data)
        {
            return data != null ? Serialiser.Serialize(data) : null;
        }
    }
}
