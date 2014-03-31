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
        internal JavaScriptSerializer Serialiser { get; private set;}

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
            Serialiser = new JavaScriptSerializer();
            Serialiser.RegisterConverters(new List<JavaScriptConverter>() { new HoistModelJavaScriptConverter() });
        }

        public HoistUser Login(string email, string password)
        {
            var response = Post(EndPoints.Login, new LoginPayload(email, password)); 
            _session = response.HoistSession;
            return ProcessHoistUser(response);
        }

        public HoistUser Status()
        {
            var response = _httpLayer.Get(EndPoints.Status, _apiKey, _session);
            return ProcessHoistUser(response);
        }

        public HoistCollection<HoistModel> GetCollection(string collectionName)
        {
            return GetCollection<HoistModel>(collectionName);
        }

        public HoistCollection<CollectionType> GetCollection<CollectionType>(string collectionName) where CollectionType : class
        {
            return new HoistCollection<CollectionType>(this, collectionName);
        }

        internal ApiResponse Post(string endPoint, object data)
        {
            return _httpLayer.Post(endPoint, _apiKey, _session, Serialiser.Serialize(data));
        }

        private HoistUser ProcessHoistUser(ApiResponse response)
        {
            if (response.Code == 200)
            {
                return Serialiser.Deserialize<HoistUser>(response.Payload);
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
