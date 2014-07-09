using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hoist.Api.Model;
using Hoist.Api.Http;
using System.Collections.Generic;


namespace Hoist.Api.Test
{
    [TestClass]
    public class ProxyTests
    {
        MockHttpLayer httpLayer = null;
        HoistClient client = null;

        [TestInitialize]
        public void TestInitalize()
        {
            httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse() { Code = 500, Description = "UNKNOWN" };
            client = new HoistClient("MYAPI", httpLayer);
        }

        public void ConfirmCall(MockHttpLayer.HttpCall expectedCall, MockHttpLayer.HttpCall actual)
        {
            Assert.AreEqual(expectedCall.endpoint, actual.endpoint, "EndPoint Different");
            Assert.AreEqual(expectedCall.apiKey, actual.apiKey, "ApiKey Different");
            Assert.AreEqual(expectedCall.data, actual.data, "Data Different");
            Assert.AreEqual(expectedCall.session, actual.session, "Session Different");
            Assert.AreEqual(expectedCall.method, actual.method, "Method Different");
            Assert.AreEqual(expectedCall.oauth, actual.oauth, "OAuth Different");
            Assert.AreEqual(expectedCall.environment, actual.environment, "Enviroment Different");
        }

        [TestMethod]
        public void CanGetProxyObject()
        {
            HoistProxy proxy = client.GetProxy("xero");
            Assert.IsNotNull(proxy, "Should create proxy object");
            Assert.IsFalse(proxy.HasToken, "Should not have token");
            Assert.AreEqual("xero", proxy.Name);            
        }

        [TestMethod]
        public void CanGetProxyObjectWithToken()
        {
            HoistProxy proxy = client.GetProxy("xero", "someimportanttoken");
            Assert.IsNotNull(proxy, "Should create proxy object");
            Assert.IsTrue(proxy.HasToken, "Should have token");
            Assert.AreEqual("xero", proxy.Name);
        }

        [TestMethod]
        public void CanConnectWithProxyObject()
        {
            httpLayer.Response = new ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"redirect\":\"https://api.xero.com/oauth/Authorize?oauth_token=undefined\"}"
            };

            HoistProxy proxy = client.GetProxy("xero");
            var response  = proxy.Connect();
            Assert.AreEqual("https://api.xero.com/oauth/Authorize?oauth_token=undefined", response.redirect);

            Assert.IsNotNull(response);
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://proxy.hoi.io/xero/connect", "MYAPI", null, null, null), httpLayer.Calls[0]);
        }


        [TestMethod]
        public void CanConnectWithProxyObjectReturnsToken()
        {
            httpLayer.Response = new ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"token\":\"IAMACOOLTOKEN\"}"
            };

            HoistProxy proxy = client.GetProxy("xero");
            var response = proxy.Connect();
            Assert.AreEqual("IAMACOOLTOKEN", response.token);
            
            Assert.IsNotNull(response);
            Assert.IsTrue(proxy.HasToken, "Should have token");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://proxy.hoi.io/xero/connect", "MYAPI", null, null, null), httpLayer.Calls[0]);
        }

        [TestMethod]
        public void CanDisconnect()
        {
            httpLayer.Response = new ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"status\":\"ok\"}"
            };

            HoistProxy proxy  = client.GetProxy("xero", "iamacooltoken");
            Assert.IsTrue(proxy.HasToken, "Should have token have disconnect");
            var response = proxy.Disconnect();            
            Assert.IsTrue(response, "Disconnect should return true");
            Assert.IsFalse(proxy.HasToken, "Should not have token after disconnect");
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://proxy.hoi.io/xero/disconnect", "MYAPI", null, null), httpLayer.Calls[0]);
        }

        [TestMethod]
        public void CanGETfromProxy()
        {
            httpLayer.Response = new ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"status\":\"ok\"}"
            };

            HoistProxy proxy = client.GetProxy("xero", "iamacooltoken");
            HoistModel model = proxy.GET("Invoices");

            Assert.IsNotNull(model);
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://proxy.hoi.io/xero/Invoices", "MYAPI", null, "iamacooltoken"), httpLayer.Calls[0]);
            
        }


        [TestMethod]
        public void CanDELETEfromProxy()
        {
            httpLayer.Response = new ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"status\":\"ok\"}"
            };

            HoistProxy proxy = client.GetProxy("xero", "iamacooltoken");
            HoistModel model = proxy.DELETE("Invoices");

            Assert.IsNotNull(model);
            ConfirmCall(MockHttpLayer.HttpCall.DELETE("https://proxy.hoi.io/xero/Invoices", "MYAPI",  null, "iamacooltoken"), httpLayer.Calls[0]);
        }

        [TestMethod]
        public void CanPOSTfromProxy()
        {
            httpLayer.Response = new ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"status\":\"ok\"}"
            };

            HoistProxy proxy = client.GetProxy("xero", "iamacooltoken");
            HoistModel model = proxy.POST("Invoices", new HoistModel(new Dictionary<string, string>() { {"a","b"} }));

            Assert.IsNotNull(model);
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://proxy.hoi.io/xero/Invoices", "MYAPI", null, "{\"a\":\"b\"}", "iamacooltoken"), httpLayer.Calls[0]);
        }

        [TestMethod]
        public void CanPUTfromProxy()
        {
            httpLayer.Response = new ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"status\":\"ok\"}"
            };

            HoistProxy proxy = client.GetProxy("xero", "iamacooltoken");
            HoistModel model = proxy.PUT("Invoices", new HoistModel(new Dictionary<string, string>() { {"a","b"} }));

            Assert.IsNotNull(model);
            ConfirmCall(MockHttpLayer.HttpCall.PUT("https://proxy.hoi.io/xero/Invoices", "MYAPI", null, "{\"a\":\"b\"}", "iamacooltoken"), httpLayer.Calls[0]);
        }

        public class InvoicesJson
        {
            public class InvoiceResponse {
                public Guid Id;
            }
            public InvoiceResponse Response;
        }

        public class InvoiceJson
        {
            public class InnerJson
            {
                public string Type;
                public string Date;
                public string DueDate;
            }

            public InnerJson Invoice;

        }


        [TestMethod]
        public void CanTypeGetRequest()
        {
            httpLayer.Response = new ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"Response\":{ \"Id\": \"1a6f7749-b96a-48ec-9612-807efe894e99\"}}"
            };

            HoistProxy proxy = client.GetProxy("xero", "iamacooltoken");
            InvoicesJson model = proxy.GET<InvoicesJson>("Invoices");

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Response);
            Assert.IsNotNull(model.Response.Id);
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://proxy.hoi.io/xero/Invoices", "MYAPI", null, "iamacooltoken"), httpLayer.Calls[0]);
        }

        [TestMethod]
        public void CanTypeDeleteRequest()
        {
            httpLayer.Response = new ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"Response\":{ \"Id\": \"1a6f7749-b96a-48ec-9612-807efe894e99\"}}"
            };

            HoistProxy proxy = client.GetProxy("xero", "iamacooltoken");
            InvoicesJson model = proxy.DELETE<InvoicesJson>("Invoices");

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Response);
            Assert.IsNotNull(model.Response.Id);
            ConfirmCall(MockHttpLayer.HttpCall.DELETE("https://proxy.hoi.io/xero/Invoices", "MYAPI", null, "iamacooltoken"), httpLayer.Calls[0]);
        }

        [TestMethod]
        public void CanTypePostRequest()
        {
            httpLayer.Response = new ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"Response\":{ \"Id\": \"1a6f7749-b96a-48ec-9612-807efe894e99\"}}"
            };

            HoistProxy proxy = client.GetProxy("xero", "iamacooltoken");
            InvoicesJson model = proxy.POST<InvoicesJson>("Invoices", new InvoiceJson() { Invoice = new InvoiceJson.InnerJson() { Type = "ABS", Date = "2009-01-01", DueDate = "2009-01-19" } });

            Assert.IsNotNull(model);
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://proxy.hoi.io/xero/Invoices", "MYAPI", null, "{\"Invoice\":{\"Type\":\"ABS\",\"Date\":\"2009-01-01\",\"DueDate\":\"2009-01-19\"}}", "iamacooltoken"), httpLayer.Calls[0]);
        }

        [TestMethod]
        public void CanTypePutRequest()
        {
            httpLayer.Response = new ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"Response\":{ \"Id\": \"1a6f7749-b96a-48ec-9612-807efe894e99\"}}"
            };

            HoistProxy proxy = client.GetProxy("xero", "iamacooltoken");
            InvoicesJson model = proxy.PUT<InvoicesJson>("Invoices", new InvoiceJson() { Invoice = new InvoiceJson.InnerJson() { Type = "ABS", Date = "2009-01-01", DueDate = "2009-01-19" } });

            Assert.IsNotNull(model);
            ConfirmCall(MockHttpLayer.HttpCall.PUT("https://proxy.hoi.io/xero/Invoices", "MYAPI", null, "{\"Invoice\":{\"Type\":\"ABS\",\"Date\":\"2009-01-01\",\"DueDate\":\"2009-01-19\"}}", "iamacooltoken"), httpLayer.Calls[0]);
        }



        /*Test Name:	CanTypePostRequest
Test FullName:	Hoist.Api.Test.ProxyTests.CanTypePostRequest
Test Source:	c:\Workspaces-acox\Hoist\hoist-csharp\Hoist.Api.Test\ProxyTests.cs : line 235
Test Outcome:	Failed
Test Duration:	0:00:00.016438

Result Message:	Assert.AreEqual failed. Expected:<{}>. Actual:<{"Invoice":{"Type":"ABS","Date":"2009-01-01","DueDate":"2009-01-19"}}>. Data Different
Result StackTrace:	
at Hoist.Api.Test.ProxyTests.ConfirmCall(HttpCall expectedCall, HttpCall actual) in c:\Workspaces-acox\Hoist\hoist-csharp\Hoist.Api.Test\ProxyTests.cs:line 28
   at Hoist.Api.Test.ProxyTests.CanTypePostRequest() in c:\Workspaces-acox\Hoist\hoist-csharp\Hoist.Api.Test\ProxyTests.cs:line 252

*/


        /*
         * 
            var token = proxy.Connect();
            

            var josnObject = proxy.Get("/Invoices");
            proxy.Post("", somjson);
            proxy.Delete();
            proxy.Put();

            proxy.Disconnect(); */
    }
}
