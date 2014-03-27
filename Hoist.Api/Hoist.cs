using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hoist.Api.Exceptions;

namespace Hoist.Api
{
    public class Hoist
    {
        private string _apiKey;
        private IHttpLayer _httpLayer;

        private class EndPoints
        {
            public const string Base = "https://auth.hoi.io/";
            public const string CreateUser = Base + "user";
            public const string Login = Base + "login";
            public const string Status = Base + "status";
            public const string Logout = Base + "logout";
        }

        public Hoist(string apiKey) 
        {
            _apiKey = apiKey;

        }

        public Hoist(string apiKey, IHttpLayer httpLayer)
        {
            _apiKey = apiKey;
            _httpLayer = httpLayer;
        }

        public IJsonObject Login(string email, string password)
        {
            var response = _httpLayer.Post(EndPoints.Login, _apiKey, new LoginPayLoad(email, password));
            if (response.Code == 200) {
                return response.Payload;
            }
            else if (response.Code == 401 && response.WithWWWAuthenticate) {
                throw new BadApiKeyException();
            }
            else if (response.Code == 401) {
                return null; //Failed to login
            }
            else {
                throw new UnexpectedResponseException(response);
            }           
        }

        private class LoginPayLoad : IJsonObject 
        {
            private string _email = null;
            private string _password = null;

            public LoginPayLoad(string email, string password)
            {
                _email = email;
                _password = password;
            }

            public string Get(string key)
            {
                switch (key)
                {
                    case "email":
                        return _email;
                    case "password":
                        return _password;
                    default:
                        return null;
                }
            }
        }

    }
}
