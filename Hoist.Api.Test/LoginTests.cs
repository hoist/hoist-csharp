using System;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hoist.Api;
using System.Collections.Generic;
using Hoist.Api.Http;

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
        public void LoginReturnsUser()
        {
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{'role':'Member', 'id': '52b75440c69c80630a00000c'}"
            };
            var client = CreateHoist("MYAPIKEY");
            var usr = client.Login("Username", "Password");
            Assert.IsNotNull(usr);
            Assert.IsTrue(httpLayer.Calls.Count == 1);
            Assert.AreEqual("Member", usr.Role);
            Assert.AreEqual("52b75440c69c80630a00000c", usr.Id);
        }


        [TestMethod]
        public void CallsAfterSuccessfulLoginUseAuth()
        {
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{'role':'Member', 'id': '52b75440c69c80630a00000c'}",
                HoistSession = "hoist-session-bmsucflxdbwkaccaodcs=s%3ACU7ClH2eINsE1QDWHK9uR7AN.eKWi2Q3xQWQW8ClGq7zrGH5eHVpXgQAnBCt5A2TpSrU"
            };
            var client = CreateHoist("MYAPIKEY");
            var usr = client.Login("Username", "Password");
            Assert.IsNotNull(usr);
            Assert.IsTrue(httpLayer.Calls.Count == 1);
            Assert.AreEqual("Member", usr.Role);
            Assert.AreEqual("52b75440c69c80630a00000c", usr.Id);
            //Check the right URLs were called
            Assert.AreEqual("https://auth.hoi.io/login", httpLayer.Calls[0].Item1);
            Assert.AreEqual("MYAPIKEY", httpLayer.Calls[0].Item2);
            Assert.AreEqual("{\"Email\":\"Username\",\"Password\":\"Password\"}", httpLayer.Calls[0].Item3);
            Assert.AreEqual(null, httpLayer.Calls[0].Item4);
                        
            usr = client.Status();
            Assert.IsNotNull(usr);
            Assert.IsTrue(httpLayer.Calls.Count == 2);
            Assert.AreEqual("https://auth.hoi.io/status", httpLayer.Calls[1].Item1);
            Assert.AreEqual("MYAPIKEY", httpLayer.Calls[1].Item2);
            Assert.AreEqual(null, httpLayer.Calls[1].Item3);
            Assert.AreEqual("hoist-session-bmsucflxdbwkaccaodcs=s%3ACU7ClH2eINsE1QDWHK9uR7AN.eKWi2Q3xQWQW8ClGq7zrGH5eHVpXgQAnBCt5A2TpSrU", httpLayer.Calls[1].Item4);
            Assert.AreEqual("Member", usr.Role);
            Assert.AreEqual("52b75440c69c80630a00000c", usr.Id);
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
            Assert.IsTrue(httpLayer.Calls.Count == 1);
            Assert.IsTrue(caughtException);
        }

        [TestMethod]
        public void FailedLoginReturnsNull()
        {
            httpLayer.Response = new ApiResponse { Code = 401, WithWWWAuthenticate = false };
            var client = CreateHoist("MYAPIKEY");
            var usr = client.Login("Username", "Password1");
            Assert.IsTrue(httpLayer.Calls.Count == 1);
            Assert.IsNull(usr);
        }


        [TestMethod]
        public void BadApiReturnsExceptionWithStatus()
        {
            bool caughtException = false;
            httpLayer.Response = new ApiResponse { Code = 401, WithWWWAuthenticate = true };

            var client = CreateHoist("BADAPI");
            try
            {
                var usr = client.Status();
            }
            catch (Exceptions.BadApiKeyException)
            {
                caughtException = true;
            }
            Assert.IsTrue(httpLayer.Calls.Count == 1);
            Assert.IsTrue(caughtException);
        }

        [TestMethod]
        public void NoSessionReturnsExceptionWithStatus()
        {
            httpLayer.Response = new ApiResponse { Code = 401, WithWWWAuthenticate = false };
            var client = CreateHoist("MYAPIKEY");
            var usr = client.Status();
            Assert.IsTrue(httpLayer.Calls.Count == 1);
            Assert.IsNull(usr);
        }

        [TestMethod]
        public void HTTPCode500ReturnsExceptionWithStatus()
        {
            bool caughtException = false;
            httpLayer.Response = new ApiResponse { Code = 500, WithWWWAuthenticate = false };
            var client = CreateHoist("MYAPIKEY");
            try
            {
                var usr = client.Status();
            }
            catch (Exceptions.UnexpectedResponseException)
            {
                caughtException = true;
            }
            Assert.IsTrue(httpLayer.Calls.Count == 1);
            Assert.IsTrue(caughtException);
        }

        [TestMethod]
        public void HTTPCode500ReturnsExceptionWithLogin()
        {
            bool caughtException = false;
            httpLayer.Response = new ApiResponse { Code = 500, WithWWWAuthenticate = false };
            var client = CreateHoist("MYAPIKEY");
            try
            {
                var usr = client.Login("username", "password");
            }
            catch (Exceptions.UnexpectedResponseException)
            {
                caughtException = true;
            }
            Assert.IsTrue(httpLayer.Calls.Count == 1);
            Assert.IsTrue(caughtException);
        }

                    
    }

   
}
