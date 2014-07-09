using System;
using Hoist.Api.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hoist.Api.Test
{
    [TestClass]
    public class EnvironmentTest
    {
        MockHttpLayer httpLayer = null;

        public void ConfirmCall(MockHttpLayer.HttpCall expectedCall, MockHttpLayer.HttpCall actual)
        {
            
            Assert.AreEqual(expectedCall.endpoint, actual.endpoint, "EndPoint Different");
            Assert.AreEqual(expectedCall.apiKey, actual.apiKey, "ApiKey Different");
            Assert.AreEqual(expectedCall.data, actual.data, "Data Different");
            Assert.AreEqual(expectedCall.session, actual.session, "Session Different");
            Assert.AreEqual(expectedCall.method, actual.method, "Method Different");
            Assert.AreEqual(expectedCall.oauth, actual.oauth, "OAuth Different");
            
        }

        [TestInitialize]
        public void TestInitalize()
        {
            httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse() { Code = 500, Description = "UNKNOWN" };
        }

        public HoistClient CreateHoist(string apiKey, string enviroment)
        {
            return new HoistClient(apiKey, httpLayer, enviroment);
        }

        [TestMethod]
        public void LoginReturnsUserWithEnviroment()
        {
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{'role':'Member', 'id': '52b75440c69c80630a00000c'}"
            };
            var client = CreateHoist("MYAPIKEY", "test");
            var usr = client.Login("Username", "Password");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/login?overrideEnviroment=test", "MYAPIKEY", null, "{\"email\":\"Username\",\"password\":\"Password\"}", null), httpLayer.Calls[0]);
        }

        [TestMethod]
        public void LoginReturnsUserWithoutEnviroment()
        {
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{'role':'Member', 'id': '52b75440c69c80630a00000c'}"
            };
            var client = CreateHoist("MYAPIKEY", null);
            var usr = client.Login("Username", "Password");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/login", "MYAPIKEY", null, "{\"email\":\"Username\",\"password\":\"Password\"}", null), httpLayer.Calls[0]);
        }

        [TestMethod]
        public void StatusReturnsUserWithEnviroment()
        {
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{'role':'Member', 'id': '52b75440c69c80630a00000c'}"
            };
            var client = CreateHoist("MYAPIKEY", "test");
            var usr = client.Status();
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/status?overrideEnviroment=test", "MYAPIKEY", null, null), httpLayer.Calls[0]);
        }

        [TestMethod]
        public void StatusReturnsUserWithoutEnviroment()
        {
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{'role':'Member', 'id': '52b75440c69c80630a00000c'}"
            };
            var client = CreateHoist("MYAPIKEY", null);
            var usr = client.Status();
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/status", "MYAPIKEY", null, null), httpLayer.Calls[0]);
        }

        [TestMethod]
        public void ProxyReturnsWithoutEnviroment()
        {
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{'role':'Member', 'id': '52b75440c69c80630a00000c'}"
            };
            var client = CreateHoist("MYAPIKEY", null);
            var proxy = client.GetProxy("myproxy");
            var usr = proxy.DELETE("user");
            ConfirmCall(MockHttpLayer.HttpCall.DELETE("https://proxy.hoi.io/myproxy/user", "MYAPIKEY", null, null), httpLayer.Calls[0]);

            usr = proxy.GET("user");
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://proxy.hoi.io/myproxy/user", "MYAPIKEY", null, null), httpLayer.Calls[1]);

            usr = proxy.POST("user", new {});
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://proxy.hoi.io/myproxy/user", "MYAPIKEY",  null, "{}"), httpLayer.Calls[2]);

            usr = proxy.PUT("user", new { });
            ConfirmCall(MockHttpLayer.HttpCall.PUT("https://proxy.hoi.io/myproxy/user", "MYAPIKEY", null, "{}"), httpLayer.Calls[3]);


        }

        [TestMethod]
        public void ProxyReturnsWithEnviroment()
        {
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{'role':'Member', 'id': '52b75440c69c80630a00000c'}"
            };
            var client = CreateHoist("MYAPIKEY", "test");
            var proxy = client.GetProxy("myproxy");
            var usr = proxy.DELETE("user");
            ConfirmCall(MockHttpLayer.HttpCall.DELETE("https://proxy.hoi.io/myproxy/user?overrideEnviroment=test", "MYAPIKEY", null, null), httpLayer.Calls[0]);

            usr = proxy.GET("user");
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://proxy.hoi.io/myproxy/user?overrideEnviroment=test", "MYAPIKEY", null, null), httpLayer.Calls[1]);

            usr = proxy.POST("user", new { });
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://proxy.hoi.io/myproxy/user?overrideEnviroment=test", "MYAPIKEY", null, "{}"), httpLayer.Calls[2]);

            usr = proxy.PUT("user", new { });
            ConfirmCall(MockHttpLayer.HttpCall.PUT("https://proxy.hoi.io/myproxy/user?overrideEnviroment=test", "MYAPIKEY", null, "{}"), httpLayer.Calls[3]);
        }
    }
}
