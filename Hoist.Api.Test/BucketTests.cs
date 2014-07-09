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
            
            var client = new HoistClient("MYAPI", httplayer);
            var buckets = client.ListBuckets();
            Assert.IsTrue(buckets.Count == 0, "Buckets should be empty");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/buckets", "MYAPI", null, null), httplayer.Calls[0]);

        }

        [TestMethod]
        public void CanGet1Buckets()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "[{\"key\":\"ping\"}]"
            };

            var client = new HoistClient("MYAPI", httplayer);
            var buckets = client.ListBuckets();
            Assert.IsTrue(buckets.Count == 1, "Buckets should contain 1");
            Assert.AreEqual("ping", buckets[0].key);
            
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/buckets", "MYAPI", null, null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CanGetMulitpleBuckets()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "[{\"name\":\"myfirstbucket\",\"key\":\"wthpw\"},{\"name\":\"mysecondbucket\",\"key\":\"pvpmv\"},{\"meta\":[{\"a\":\"b\"}],\"key\":\"ping\"},{\"key\":\"pong\"}]"
            };

            var client = new HoistClient("MYAPI", httplayer);
            var buckets = client.ListBuckets();
            Assert.AreEqual(4, buckets.Count, "Buckets should contain 3");
            Assert.AreEqual("wthpw", buckets[0].key);
            Assert.AreEqual(null, buckets[0].meta);   
            Assert.AreEqual("pvpmv", buckets[1].key);
            Assert.AreEqual("ping", buckets[2].key);
            Assert.AreEqual(1, buckets[2].meta[0].Keys.Count);
            Assert.AreEqual("pong", buckets[3].key);            
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/buckets", "MYAPI", null, null), httplayer.Calls[0]);
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
            var client = new HoistClient("MYAPI", httplayer);
            var buckets = client.ListBuckets();
            Assert.IsNull(buckets);
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/buckets", "MYAPI", null, null), httplayer.Calls[0]);
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
            var client = new HoistClient("MYAPI", httplayer);
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
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/buckets", "MYAPI", null, null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CanCreateBucket()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"key\":\"pong\"}"
            };

            var client = new HoistClient("MYAPI", httplayer);
            var bucket = client.CreateBucket("MyBucket");
            Assert.AreEqual("pong", bucket.key, "Should return true if it worked");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/MyBucket", "MYAPI", null, "{}", null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CanCreateBucketWithMeta()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"key\":\"pong\", \"meta\":{\"a\":\"b\"}}"
            };

            var client = new HoistClient("MYAPI", httplayer);
            var bucket = client.CreateBucket("MyBucket", new HoistModel( new Dictionary<string, string>() { {"a","b"} } ) );
            Assert.AreEqual("pong", bucket.key, "Should return true if it worked");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/MyBucket", "MYAPI", null, "{\"a\":\"b\"}", null), httplayer.Calls[0]);
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

            var client = new HoistClient("MYAPI", httplayer);
            var caughtEx = false;
            try
            {
                var worked = client.CreateBucket("MyBucket");
            }
            catch (Exceptions.DataConflictException)
            {
                caughtEx = true;
            }
            Assert.IsTrue(caughtEx, "Should return DataConflictException");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/MyBucket", "MYAPI", null, "{}", null), httplayer.Calls[0]);
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

            var client = new HoistClient("MYAPI", httplayer);
            var worked = client.CreateBucket("MyBucket");
            Assert.IsNull(worked, "Should return null on 401");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/MyBucket", "MYAPI", null, "{}", null), httplayer.Calls[0]);
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

            var client = new HoistClient("MYAPI", httplayer);
            var caughtEx = false;
            try
            {
                var worked = client.CreateBucket("MyBucket");
            }
            catch (Exceptions.BadApiKeyException)
            {
                caughtEx = true;
            }
            Assert.IsTrue(caughtEx, "Should return BadApiKeyException");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/MyBucket", "MYAPI", null, "{}", null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CanEnterBucket()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"key\":\"ping\"}"
            };

            var client = new HoistClient("MYAPI", httplayer);
                        
            var bucket = client.EnterBucket("withpw");
            Assert.AreEqual("ping", bucket.key, "Should return bucket when setting bucket");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/current/withpw", "MYAPI", null, "{}", null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CanEnterBucketReturns403WhenNoAccess()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 403,
                WithWWWAuthenticate = false,
                Payload = ""
            };

            var client = new HoistClient("MYAPI", httplayer);
            var caughtException = false;
            try
            {
                var bucket = client.EnterBucket("withpw");
            }
            catch (Exceptions.UnauthorisedException)
            {
                caughtException = true;
            }

            Assert.IsTrue(caughtException, "Should raise exception");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/current/withpw", "MYAPI", null, "{}", null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CanLeaveBucket()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"status\":\"ok\"}"
            };

            var client = new HoistClient("MYAPI", httplayer);
            var success = client.LeaveBucket();

            Assert.IsTrue(success, "Should return true");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/current/default", "MYAPI", null, "{}", null), httplayer.Calls[0]);
        
        }

        [TestMethod]
        public void LeaveBucket401ReturnsTrue()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 401,
                WithWWWAuthenticate = false,
                Payload = "{\"doNotKill\":true,\"name\":\"That request requires a user to be logged in\",\"logLevel\":6,\"resCode\":401,\"message\":\"Default Error Message\"}"
            };

            var client = new HoistClient("MYAPI", httplayer);
            var success = client.LeaveBucket();

            Assert.IsTrue(success, "Should return true");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/current/default", "MYAPI", null, "{}", null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CanGetCurrentBucket()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"key\":\"pong\", \"meta\":[{\"a\":\"b\"}]}"
            };

            var client = new HoistClient("MYAPI", httplayer);
            HoistBucket bucket = client.CurrentBucket();

            Assert.AreEqual("pong", bucket.key);
            Assert.AreEqual("b", bucket.meta[0].Get("a"));            
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/bucket/current", "MYAPI", null, null), httplayer.Calls[0]);
        
        }

        [TestMethod]
        public void CurrentBucket401ReturnsNull()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 401,
                WithWWWAuthenticate = false,
                Payload = "{\"doNotKill\":true,\"name\":\"That request requires a user to be logged in\",\"logLevel\":6,\"resCode\":401,\"message\":\"Default Error Message\"}"
            };

            var client = new HoistClient("MYAPI", httplayer);
            HoistBucket bucket = client.CurrentBucket();

            Assert.IsNull(bucket);
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/bucket/current", "MYAPI", null, null), httplayer.Calls[0]);

        }

        [TestMethod]
        public void CurrentBucket404ReturnsNull()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 404,
                WithWWWAuthenticate = false,
                Payload = "{}"
            };

            var client = new HoistClient("MYAPI", httplayer);
            HoistBucket bucket = client.CurrentBucket();
            Assert.IsNull(bucket);
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.GET("https://auth.hoi.io/bucket/current", "MYAPI", null, null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void CanUpdateBucket()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"key\":\"pong\", \"meta\":[{\"a\":\"b\"}]}"
            };

            var client = new HoistClient("MYAPI", httplayer);
            HoistBucket bucket = client.UpdateBucket(new HoistBucket()
            {
                key = "pong",
                meta = new List<HoistModel>() { new HoistModel(new Dictionary<string, string>() { {"a","b"} } ) }
            });

            Assert.AreEqual("pong", bucket.key);
            Assert.AreEqual("b", bucket.meta[0].Get("a"));
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/pong/meta", "MYAPI", null, "[{\"a\":\"b\"}]", null), httplayer.Calls[0]);
        }

        [TestMethod]
        public void UpdateBucket404ReturnsException()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 404,
                WithWWWAuthenticate = false,
                Payload = "{}"
            };

            var client = new HoistClient("MYAPI", httplayer);
            bool caughtException = false;
            try
            {
                HoistBucket bucket = client.UpdateBucket(new HoistBucket()
                {
                    key = "pong",
                    meta = new List<HoistModel>() { new HoistModel(new Dictionary<string, string>() { { "a", "b" } }) }
                });
            }
            catch (Exceptions.NotFoundException)
            {
                caughtException = true;

            }

            Assert.IsTrue(caughtException, "404 should return NotFoundException");            
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/pong/meta", "MYAPI", null, "[{\"a\":\"b\"}]", null), httplayer.Calls[0]);
        
        }

        [TestMethod]
        public void UpdateBucket403ReturnsException()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 403,
                WithWWWAuthenticate = false,
                Payload = "{}"
            };

            var client = new HoistClient("MYAPI", httplayer);
            bool caughtException = false;
            try
            {
                HoistBucket bucket = client.UpdateBucket(new HoistBucket()
                {
                    key = "pong",
                    meta = new List<HoistModel>() { new HoistModel(new Dictionary<string, string>() { { "a", "b" } }) }
                });
            }
            catch (Exceptions.UnauthorisedException)
            {
                caughtException = true;

            }

            Assert.IsTrue(caughtException, "403 should return NotFoundException");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/pong/meta", "MYAPI", null, "[{\"a\":\"b\"}]", null), httplayer.Calls[0]);

        }

        [TestMethod]
        public void UpdateBucket401ReturnsException()
        {
            var httplayer = new MockHttpLayer();
            httplayer.Response = new Http.ApiResponse()
            {
                Code = 401,
                WithWWWAuthenticate = false,
                Payload = "{}"
            };

            var client = new HoistClient("MYAPI", httplayer);
            bool caughtException = false;
            try
            {
                HoistBucket bucket = client.UpdateBucket(new HoistBucket()
                {
                    key = "pong",
                    meta = new List<HoistModel>() { new HoistModel(new Dictionary<string, string>() { { "a", "b" } }) }
                });
            }
            catch (Exceptions.UnauthorisedException)
            {
                caughtException = true;

            }

            Assert.IsTrue(caughtException, "403 should return NotFoundException");
            Assert.IsTrue(httplayer.Calls.Count == 1, "Should call API");
            ConfirmCall(MockHttpLayer.HttpCall.POST("https://auth.hoi.io/bucket/pong/meta", "MYAPI", null, "[{\"a\":\"b\"}]", null), httplayer.Calls[0]);

        }


        
        /*
         * Bucket Auth Spec:

    POST /bucket/<KEY>/meta
      when the user is not logged in
        - will return a 401 (unathorised) response
        - will not modify the bucket

      if no bucket with the key exists
        - will return a 404 (not found) response

      if the user doesn't own the bucket
        - will return a 403 (forbidden) response

      if the user has ownership of the bucket
        - will return a 200 (ok) response
        - will replace the meta data for the bucket with the POST body
*/


    }
}
