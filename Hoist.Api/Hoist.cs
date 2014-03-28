using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Hoist.Api.Exceptions;
using Hoist.Api.Http;
using Hoist.Api.Model;

namespace Hoist.Api
{
    public class Hoist
    {
        private string _apiKey;
        private string _session;

        private IHttpLayer _httpLayer;
        private JavaScriptSerializer _serialiser;

        private class EndPoints
        {
            public const string Base = "https://auth.hoi.io/";
            public const string CreateUser = Base + "user";
            public const string Login = Base + "login";
            public const string Status = Base + "status";
            public const string Logout = Base + "logout";
        }

        public Hoist(string apiKey) : this(apiKey, new HoistHttpLayer()) { } 

        public Hoist(string apiKey, IHttpLayer httpLayer)
        {
            _apiKey = apiKey;
            _httpLayer = httpLayer;
            _serialiser = new JavaScriptSerializer();
        }

        public HoistUser Login(string email, string password)
        {
            var response = _httpLayer.Post(EndPoints.Login, _apiKey, null, _serialiser.Serialize(new LoginPayload(email, password)));
            _session = response.HoistSession;
            return ProcessHoistUser(response);
        }

        public HoistUser Status()
        {
            var response = _httpLayer.Get(EndPoints.Status, _apiKey, _session);
            return ProcessHoistUser(response);
        }

        private HoistUser ProcessHoistUser(ApiResponse response)
        {
            if (response.Code == 200)
            {
                return _serialiser.Deserialize<HoistUser>(response.Payload);
            }
            else if (response.Code == 401 && response.WithWWWAuthenticate)
            {
                throw new BadApiKeyException();
            }
            else if (response.Code == 401)
            {
                return null; //Failed to login
            }
            else
            {
                throw new UnexpectedResponseException(response);
            }
        }

    }
}
