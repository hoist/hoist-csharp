using System;
using System.Collections.Generic;
using Hoist.Api.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hoist.Api.Test
{
    [TestClass]
    public class BucketTests
    {
        public void ConfirmCall(MockHttpLayer.HttpCall expectedCall, MockHttpLayer.HttpCall actual) {
            Assert.AreEqual(expectedCall.endpoint,actual.endpoint, "EndPoint Different");
            Assert.AreEqual(expectedCall.apiKey, actual.apiKey, "ApiKey Different");
            Assert.AreEqual(expectedCall.data, actual.data, "Data Different");
            Assert.AreEqual(expectedCall.session, actual.session, "Session Different");
            Assert.AreEqual(expectedCall.method, actual.method, "Method Different");
        }

        [TestMethod]
        public void CanGetEmptyBuckets()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "[]"
            };
            
            var client = new Hoist("MYAPI", httplayer);
            var buckets = client.ListBuckets();
            Assert.IsTrue(buckets.Count == 0, "Buckets should be empty");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/buckets", "MYAPI", null), httplayer.Calls[0]);

        }

        [TestMethod]
        public void CanGet1Buckets()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "[{\"name\":\"myfirstbucket\",\"owner\":\"530c623381aee28c0600000c\",\"_id\":\"533b27f14e7521fa0d00000b\",\"key\":\"wthpw\",\"members\":[\"530c623381aee28c0600000c\"]}]"
            };

            var client = new Hoist("MYAPI", httplayer);
            var buckets = client.ListBuckets();
            Assert.IsTrue(buckets.Count == 1, "Buckets should contain 1");
            Assert.AreEqual("myfirstbucket", buckets[0].name);
            Assert.AreEqual("530c623381aee28c0600000c", buckets[0].owner);
            Assert.AreEqual("533b27f14e7521fa0d00000b", buckets[0]._id);
            Assert.AreEqual("wthpw", buckets[0].key);
            Assert.AreEqual(1, buckets[0].members.Count);
            Assert.AreEqual("530c623381aee28c0600000c", buckets[0].members[0]);
            
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/buckets", "MYAPI", null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CanGetMulitpleBuckets()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "[{\"name\":\"myfirstbucket\",\"owner\":\"530c623381aee28c0600000c\",\"_id\":\"533b27f14e7521fa0d00000b\",\"key\":\"wthpw\",\"members\":[\"530c623381aee28c0600000c\"]}, {\"name\":\"mysecondbucket\",\"owner\":\"530c623381aee28c0600000c\",\"_id\":\"533b27f14e7521fa0d00000c\",\"key\":\"pvpmv\",\"members\":[\"530c623381aee28c0600000c\"]}]"
            };

            var client = new Hoist("MYAPI", httplayer);
            var buckets = client.ListBuckets();
            Assert.IsTrue(buckets.Count == 2, "Buckets should contain 2");
            Assert.AreEqual("myfirstbucket", buckets[0].name);
            Assert.AreEqual("530c623381aee28c0600000c", buckets[0].owner);
            Assert.AreEqual("533b27f14e7521fa0d00000b", buckets[0]._id);
            Assert.AreEqual("wthpw", buckets[0].key);
            Assert.AreEqual(1, buckets[0].members.Count);
            Assert.AreEqual("530c623381aee28c0600000c", buckets[0].members[0]);

            Assert.AreEqual("mysecondbucket", buckets[1].name);
            Assert.AreEqual("530c623381aee28c0600000c", buckets[1].owner);
            Assert.AreEqual("533b27f14e7521fa0d00000c", buckets[1]._id);
            Assert.AreEqual("pvpmv", buckets[1].key);
            Assert.AreEqual(1, buckets[1].members.Count);
            Assert.AreEqual("530c623381aee28c0600000c", buckets[1].members[0]);

            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/buckets", "MYAPI", null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void GetBucketsCall401ReturnsNull()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 401,
                WithWWWAuthenticate = false,
                Payload = "{}"
            };
            var client = new Hoist("MYAPI", httplayer);
            var buckets = client.ListBuckets();
            Assert.IsNull(buckets);
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/buckets", "MYAPI", null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void GetBucketsCall401WithAuthReturnsException()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 401,
                WithWWWAuthenticate = true,
                Payload = "Unauthorized"
            };
            var client = new Hoist("MYAPI", httplayer);
            var caughtEx = false;
            try
            {
                var buckets = client.ListBuckets();
            }
            catch (Exceptions.BadApiKeyException)
            {
                caughtEx = true;
            }
            Assert.IsTrue(caughtEx);
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/buckets", "MYAPI", null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CanCreateBucket()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{}"
            };

            var client = new Hoist("MYAPI", httplayer);
            bool worked = client.CreateBucket("MyBucket");
            Assert.IsTrue(worked, "Should return true if it worked");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/MyBucket", "MYAPI", null, "{}"), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CreateBucket409RaisesException()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 409,
                WithWWWAuthenticate = false,
                Payload = "{\"message\":\"A bucket with that name already exists\"}"
            };

            var client = new Hoist("MYAPI", httplayer);
            var caughtEx = false;
            try
            {
                bool worked = client.CreateBucket("MyBucket");
            }
            catch (Exceptions.DataConflictException)
            {
                caughtEx = true;
            }
            Assert.IsTrue(caughtEx, "Should return DataConflictException");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/MyBucket", "MYAPI", null, "{}"), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CreateBucket401ReturnsFalse()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 401,
                WithWWWAuthenticate = false,
                Payload = "{}"
            };

            var client = new Hoist("MYAPI", httplayer);
            bool worked = client.CreateBucket("MyBucket");
            Assert.IsFalse(worked, "Should return false on 401");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/MyBucket", "MYAPI", null, "{}"), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CreateBucket401WithAuthRaisesException()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 401,
                WithWWWAuthenticate = true,
                Payload = "Unauthorized"
            };

            var client = new Hoist("MYAPI", httplayer);
            var caughtEx = false;
            try
            {
                bool worked = client.CreateBucket("MyBucket");
            }
            catch (Exceptions.BadApiKeyException)
            {
                caughtEx = true;
            }
            Assert.IsTrue(caughtEx, "Should return BadApiKeyException");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/MyBucket", "MYAPI", null, "{}"), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CanSetCurrentBucket()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = ""
            };

            var client = new Hoist("MYAPI", httplayer);

            var bucket = new HoistBucket() { 
                name = "myfirstbucket", 
                _id = "533b27f14e7521fa0d00000b", 
                key = "withpw", 
                owner = "530c623381aee28c0600000c" ,
                members = new List<string>() { "530c623381aee28c0600000c" }
            };

            var success = client.SetCurrentBucket(bucket);
            Assert.IsTrue(success, "Should return true when setting bucket");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/current/withpw", "MYAPI", null, "{}"), httplayer.Calls[0]);


        }




    }
}
