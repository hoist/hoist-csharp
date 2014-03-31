using System;
using Hoist.Api.Exceptions;
using Hoist.Api.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hoist.Api.Test
{
    [TestClass]
    public class NotificationTests
    {
        [TestMethod]
        public void SendNotificationCallsApi()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"status\":\"ok\"}"
            };

            var client = new Hoist("MYAPI", httpLayer);
            var sent = client.SendNotification("MyTestEmail", new { name = "Owen", Date = "25th December 2013", Message = "Merry Christmas" });
            Assert.IsTrue(sent);
            Assert.AreEqual(1, httpLayer.Calls.Count, "Calls API");
            Assert.AreEqual("https://notify.hoi.io/notification/MyTestEmail", httpLayer.Calls[0].Item1);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].Item2);
            Assert.AreEqual("{\"name\":\"Owen\",\"Date\":\"25th December 2013\",\"Message\":\"Merry Christmas\"}", httpLayer.Calls[0].Item3);
            Assert.AreEqual(null, httpLayer.Calls[0].Item4);
            Assert.AreEqual("POST", httpLayer.Calls[0].Item5);
        }

        [TestMethod]
        public void SendNotificationHandles401()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 401,
                WithWWWAuthenticate = true,
                Payload = ""
            };
            var client = new Hoist("MYAPI", httpLayer);

            var caughtException = false;
            try
            {
                var sent = client.SendNotification("MyTestEmail", new { name = "Owen", Date = "25th December 2013", Message = "Merry Christmas" });
            }
            catch (BadApiKeyException)
            {
                caughtException = true;
            }

            Assert.IsTrue(caughtException);
            Assert.AreEqual(1, httpLayer.Calls.Count, "Calls API");
            Assert.AreEqual("https://notify.hoi.io/notification/MyTestEmail", httpLayer.Calls[0].Item1);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].Item2);
            Assert.AreEqual("{\"name\":\"Owen\",\"Date\":\"25th December 2013\",\"Message\":\"Merry Christmas\"}", httpLayer.Calls[0].Item3);
            Assert.AreEqual(null, httpLayer.Calls[0].Item4);
            Assert.AreEqual("POST", httpLayer.Calls[0].Item5);
        }

        [TestMethod]
        public void SendNotificationHandles404()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 404,
                WithWWWAuthenticate = false,
                Payload = ""
            };
            var client = new Hoist("MYAPI", httpLayer);

            var caughtException = false;
            try
            {
                var sent = client.SendNotification("MyTestEmail", new { name = "Owen", Date = "25th December 2013", Message = "Merry Christmas" });
            }
            catch (TemplateNotFoundException)
            {
                caughtException = true;
            }

            Assert.IsTrue(caughtException);
            Assert.AreEqual(1, httpLayer.Calls.Count, "Calls API");
            Assert.AreEqual("https://notify.hoi.io/notification/MyTestEmail", httpLayer.Calls[0].Item1);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].Item2);
            Assert.AreEqual("{\"name\":\"Owen\",\"Date\":\"25th December 2013\",\"Message\":\"Merry Christmas\"}", httpLayer.Calls[0].Item3);
            Assert.AreEqual(null, httpLayer.Calls[0].Item4);
            Assert.AreEqual("POST", httpLayer.Calls[0].Item5);
        }


    }
}
