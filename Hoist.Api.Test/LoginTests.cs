using System;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hoist.Api;
using System.Collections.Generic;

namespace Hoist.Api.Test
{
    [TestClass]
    public class LoginTests
    {
        MockHttpLayer httpLayer = null;

        [TestInitialize]
        public void TestInitalize()
        {
            httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse() { Code = 500, Description = "UNKNOWN" };
        }
        
        public Hoist CreateHoist(string apiKey)
        {
            return new Hoist(apiKey, httpLayer);
        }

        [TestMethod]
        public void CanAttemptToLogin()
        {
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = new SimpleHoistObject(
                    new Dictionary<string, string>() { { "role", "Member" }, { "id", "52b75440c69c80630a00000c" } })
            };
            var client = CreateHoist("MYAPIKEY");
            var usr = client.Login("Username", "Password");
            Assert.IsNotNull(usr);
            Assert.IsTrue(httpLayer.Calls.Count > 0);
        }

        [TestMethod]      
        public void BadApiKeyReturnExceptionOnLogin()
        {
            bool caughtException = false;
            httpLayer.Response = new ApiResponse { Code = 401, WithWWWAuthenticate = true };

            var client = CreateHoist("BADAPI");
            try
            {
                var usr = client.Login("Username", "Password");
            }
            catch (Exceptions.BadApiKeyException)
            {
                caughtException = true;
            }
            Assert.IsTrue(httpLayer.Calls.Count > 0);
            Assert.IsTrue(caughtException);
        }

        [TestMethod]
        public void FailedLoginReturnsNull()
        {
            httpLayer.Response = new ApiResponse { Code = 401, WithWWWAuthenticate = false };
            var client = CreateHoist("MYAPIKEY");
            var usr = client.Login("Username", "Password1");
            Assert.IsTrue(httpLayer.Calls.Count > 0);
            Assert.IsNull(usr); 
        }
                    
    }

    public class MockHttpLayer : IHttpLayer {

        public List<Tuple<string, string>> Calls = new List<Tuple<string, string>>();
        public ApiResponse Response = null;
        public Exception ErrorToThrow = null;

        public ApiResponse Post(string endpoint, string apiKey, IJsonObject data)
        {
            Calls.Add(new Tuple<string, string>(endpoint, apiKey));
            if (ErrorToThrow != null)
            {
                throw ErrorToThrow;
            }
            return Response;
        }

    }
}
