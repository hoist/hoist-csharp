using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hoist.Api;
using Hoist.Api.Model;
using System.Collections.Generic;
using Hoist.Api.Http;
using Hoist.Api.Exceptions;

namespace Hoist.Api.Test
{
    [TestClass]
    public class DataTests
    {
        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public class PersonWithHoist
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string _id { get;set;}
            public string _rev {get;set;}
        }

        [TestMethod]
        public void CanGetCollection()
        {
            var mockHttp = new MockHttpLayer();
            var client = new HoistClient("MYAPI", mockHttp);
            var collection = client.GetCollection("MyCollection");
            Assert.IsNotNull(collection);
            Assert.AreEqual("MyCollection", collection.Name);
            Assert.AreEqual(typeof(HoistModel), collection.ConversionType);
            Assert.AreEqual(0, mockHttp.Calls.Count, "Non calls are made just to create collection");
        }

        [TestMethod]
        public void CanGetTypedCollection()
        {
            var mockHttp = new MockHttpLayer();
            var client = new HoistClient("MYAPI", mockHttp);
            var collection = client.GetCollection<Person>("MyCollection");
            Assert.IsNotNull(collection);
            Assert.AreEqual("MyCollection", collection.Name);
            Assert.AreEqual(typeof(Person), collection.ConversionType);
            Assert.AreEqual(0, mockHttp.Calls.Count, "Non calls are made just to create collection");
        }

        [TestMethod]
        public void CanInsertIntoCollection()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"k1\":\"v1\", \"_id\":\"1231313\"}"
            };

            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection("MyCollection");
            Assert.IsNotNull(collection);
            Assert.AreEqual("MyCollection", collection.Name);
            Assert.AreEqual(typeof(HoistModel), collection.ConversionType);
            Assert.AreEqual(0, httpLayer.Calls.Count, "Non calls are made just to create collection");

            var myInsert = collection.Insert(new HoistModel(new Dictionary<string,string> { {"k1","v1"} }));
            Assert.AreEqual(1, httpLayer.Calls.Count, "Insert calls API");
            Assert.IsNotNull(myInsert);
            Assert.AreEqual("v1", myInsert.Get("k1"));
            Assert.AreEqual("1231313", myInsert.Get("_id"));
            Assert.AreEqual("https://data.hoi.io/MyCollection", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual( "{\"k1\":\"v1\"}" , httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);

        }

        [TestMethod]
        public void CanInsertIntoTypedCollection()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"Name\":\"Andrew\",\"Age\":\"34\",\"_id\":\"1231313\"}"
            };

            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection<Person>("MyCollection");
            Assert.IsNotNull(collection);
            Assert.AreEqual("MyCollection", collection.Name);
            Assert.AreEqual(typeof(Person), collection.ConversionType);
            Assert.AreEqual(0, httpLayer.Calls.Count, "Non calls are made just to create collection");

            var myInsert = collection.Insert(new Person() { Name = "Andrew", Age = 34 });
            Assert.AreEqual(1, httpLayer.Calls.Count, "Insert calls API");
            Assert.IsNotNull(myInsert);
            Assert.AreEqual("Andrew", myInsert.Name);
            Assert.AreEqual(34, myInsert.Age);
            Assert.AreEqual("https://data.hoi.io/MyCollection", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual("{\"Name\":\"Andrew\",\"Age\":34}", httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
        }

        [TestMethod]
        public void InsertUsesAuthIfThere()
        {
            var hoistSession = "hoist-session-bmsucflxdbwkaccaodcs=s%3ACU7ClH2eINsE1QDWHK9uR7AN.eKWi2Q3xQWQW8ClGq7zrGH5eHVpXgQAnBCt5A2TpSrU";
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{'role':'Member', 'id': '52b75440c69c80630a00000c'}",
                HoistSession = hoistSession
            };


            var client = new HoistClient("MYAPI", httpLayer);
            client.Login("email", "password");

            var collection = client.GetCollection<Person>("MyCollection");
            Assert.IsNotNull(collection);
            
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"Name\":\"Andrew\",\"Age\":\"34\",\"_id\":\"1231313\"}"
            };

            var myInsert = collection.Insert(new Person() { Name = "Andrew", Age = 34 });
            Assert.AreEqual(2, httpLayer.Calls.Count, "Insert calls API");
            Assert.IsNotNull(myInsert);
            Assert.AreEqual("Andrew", myInsert.Name);
            Assert.AreEqual(34, myInsert.Age);
            Assert.AreEqual("https://data.hoi.io/MyCollection", httpLayer.Calls[1].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[1].apiKey);
            Assert.AreEqual("{\"Name\":\"Andrew\",\"Age\":34}", httpLayer.Calls[1].data);
            Assert.AreEqual(hoistSession, httpLayer.Calls[1].session);
        }

        [TestMethod]
        public void InsertExceptionOn409Conflict()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 409,
                WithWWWAuthenticate = false,
                Payload = ""
            };
            var caughtError = false;
            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection<Person>("MyCollection");
            try
            {
                var myInsert = collection.Insert(new Person() { Name = "Andrew", Age = 34 });
            }
            catch (DataConflictException)
            {
                caughtError = true;
            }
            Assert.IsTrue(caughtError);
        }

        [TestMethod]
        public void InsertExceptionOn401()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 401,
                WithWWWAuthenticate = true,
                Payload = ""
            };
            var caughtError = false;
            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection<Person>("MyCollection");
            try
            {
                var myInsert = collection.Insert(new Person() { Name = "Andrew", Age = 34 });
            }
            catch (BadApiKeyException)
            {
                caughtError = true;
            }
            Assert.IsTrue(caughtError);
        }
        
        [TestMethod]
        public void CanToListTypedCollection()
        {
            var httpLayer = new MockHttpLayer();
            var client = new HoistClient("MYAPI", httpLayer);
            
            var collection = client.GetCollection<Person>("MyCollection");
                        
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "[ {\"Name\":\"Andrew\",\"Age\":\"34\",\"_id\":\"1231313\"}, {\"Name\":\"Bobby\",\"Age\":\"17\",\"_id\":\"1231314\"}] "
            };
            
            List<Person> peps = collection.ToList();
            Assert.AreEqual(2, peps.Count);
            Assert.AreEqual("Andrew",peps[0].Name);
            Assert.AreEqual("Bobby", peps[1].Name);
            Assert.AreEqual(34, peps[0].Age);
            Assert.AreEqual(17, peps[1].Age);

            Assert.AreEqual(1, httpLayer.Calls.Count, "To List Calls API");
            Assert.AreEqual("https://data.hoi.io/MyCollection", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual(null, httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
            Assert.AreEqual("GET", httpLayer.Calls[0].method);
        }

        [TestMethod]
        public void CanToListUnTypedCollection()
        {
            var httpLayer = new MockHttpLayer();
            var client = new HoistClient("MYAPI", httpLayer);

            var collection = client.GetCollection("MyCollection");

            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "[ {\"Name\":\"Andrew\",\"Age\":\"34\",\"_id\":\"1231313\"}, {\"Name\":\"Bobby\",\"Age\":\"17\",\"_id\":\"1231314\"}] "
            };

            List<HoistModel> peps = collection.ToList();
            Assert.AreEqual(2, peps.Count);
            Assert.AreEqual("Andrew", peps[0].Get("Name"));
            Assert.AreEqual("Bobby", peps[1].Get("Name"));
            Assert.AreEqual("34", peps[0].Get("Age"));
            Assert.AreEqual("17", peps[1].Get("Age"));
            Assert.AreEqual("1231313", peps[0].Get("_id"));
            Assert.AreEqual("1231314", peps[1].Get("_id"));

            Assert.AreEqual(1, httpLayer.Calls.Count, "To List Calls API");
            Assert.AreEqual("https://data.hoi.io/MyCollection", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual(null, httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
            Assert.AreEqual("GET", httpLayer.Calls[0].method);
        }

        [TestMethod]
        public void ToListExceptionOn401()
        {
            var httpLayer = new MockHttpLayer();
            var client = new HoistClient("MYAPI", httpLayer);

            var collection = client.GetCollection("MyCollection");

            httpLayer.Response = new ApiResponse
            {
                Code = 401,
                WithWWWAuthenticate = true,
                Payload = ""
            };

            var caughtException = false;
            try
            {
                List<HoistModel> peps = collection.ToList();
            }
            catch (BadApiKeyException)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);
        }

        [TestMethod]
        public void GetOnUntypedCollectionWorks()
        {
            var httpLayer = new MockHttpLayer();
            var client = new HoistClient("MYAPI", httpLayer);

            var collection = client.GetCollection("MyCollection");

            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"Name\":\"Andrew\",\"Age\":\"34\",\"_id\":\"1231313\",\"_rev\":\"xx-xx\",\"_type\":\"MyCollection\",\"_updatedDate\":\"2013-12-22T21:42:24.658Z\",\"_createdDate\":\"2013-12-22T21:42:24.658Z\" }"
            };

            HoistModel pep = collection.Get("1231313");

            Assert.AreEqual("Andrew", pep.Get("Name"));
            Assert.AreEqual("34", pep.Get("Age"));

            Assert.AreEqual("1231313", pep.Get("_id"));

            Assert.AreEqual(1, httpLayer.Calls.Count, "To List Calls API");
            Assert.AreEqual("https://data.hoi.io/MyCollection/1231313", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual(null, httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
            Assert.AreEqual("GET", httpLayer.Calls[0].method);
        }

        [TestMethod]
        public void GetOnTypedCollectionWorks()
        {
            var httpLayer = new MockHttpLayer();
            var client = new HoistClient("MYAPI", httpLayer);

            var collection = client.GetCollection<Person>("MyCollection");

            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"Name\":\"Bobby\",\"Age\":\"17\",\"_id\":\"1231313\",\"_rev\":\"xx-xx\",\"_type\":\"MyCollection\",\"_updatedDate\":\"2013-12-22T21:42:24.658Z\",\"_createdDate\":\"2013-12-22T21:42:24.658Z\" }"
            };

            Person pep = collection.Get("1231313");

            Assert.AreEqual("Bobby", pep.Name);
            Assert.AreEqual(17, pep.Age);

            //Assert.AreEqual("1231313", pep.Get("_id"));

            Assert.AreEqual(1, httpLayer.Calls.Count, "To List Calls API");
            Assert.AreEqual("https://data.hoi.io/MyCollection/1231313", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual(null, httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
            Assert.AreEqual("GET", httpLayer.Calls[0].method);

        }

        [TestMethod]
        public void GetExceptionOn401()
        {
            var httpLayer = new MockHttpLayer();
            var client = new HoistClient("MYAPI", httpLayer);

            var collection = client.GetCollection("MyCollection");

            httpLayer.Response = new ApiResponse
            {
                Code = 401,
                WithWWWAuthenticate = true,
                Payload = ""
            };

            var caughtException = false;
            try
            {
                collection.Get("1234");
            }
            catch (BadApiKeyException)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);
        }

        [TestMethod]
        public void GetExceptionOn404()
        {
            var httpLayer = new MockHttpLayer();
            var client = new HoistClient("MYAPI", httpLayer);

            var collection = client.GetCollection("MyCollection");

            httpLayer.Response = new ApiResponse
            {
                Code = 404,
                WithWWWAuthenticate = false,
                Payload = ""
            };

            var caughtException = false;
            try
            {
                collection.Get("1234");
            }
            catch (NotFoundException)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);
        }

        [TestMethod]
        public void UpdateFailsWithoutRequiredSystemParamsUntyped()
        {
            var httpLayer = new MockHttpLayer();
            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection("MyCollection");
            var caughtException = false;

            try
            {
                collection.Update(new HoistModel(new Dictionary<string, string>() { { "Name", "Bobby" } }));
            }
            catch (MissingPropertiesException ex)
            {
                caughtException = true;
                Assert.AreEqual(2, ex.MissingProperties.Count);
                Assert.IsTrue(ex.MissingProperties.Contains("_id"));
                Assert.IsTrue(ex.MissingProperties.Contains("_rev"));
            }

            Assert.IsTrue(caughtException);
            Assert.IsTrue(httpLayer.Calls.Count == 0);

            try
            {
                collection.Update(new HoistModel(new Dictionary<string, string>() { { "Name", "Bobby" } , {"_id","123"} }));
            }
            catch (MissingPropertiesException ex)
            {
                caughtException = true;
                Assert.AreEqual(1, ex.MissingProperties.Count);
                Assert.IsTrue(ex.MissingProperties.Contains("_rev"));                
            }

            Assert.IsTrue(caughtException);
            Assert.IsTrue(httpLayer.Calls.Count == 0);

        }

        [TestMethod]
        public void UpdateFailsWithoutRequiredSystemParamsTyped()
        {
            var httpLayer = new MockHttpLayer();
            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection<Person>("MyCollection");
            var caughtException = false;

            try
            {
                collection.Update(new Person() { Age = 17, Name = "Bobby" });
            }
            catch (MissingPropertiesException ex)
            {
                caughtException = true;
                Assert.AreEqual(2, ex.MissingProperties.Count);
                Assert.IsTrue(ex.MissingProperties.Contains("_id"));
                Assert.IsTrue(ex.MissingProperties.Contains("_rev"));
            }

            Assert.IsTrue(caughtException);
            Assert.IsTrue(httpLayer.Calls.Count == 0);
            
        }

        [TestMethod]
        public void UpdateSuccessWithSystemParameters()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"Name\":\"Bobby\",\"Age\":\"17\",\"_id\":\"1231313\",\"_rev\":\"xx-xx\",\"_type\":\"MyCollection\",\"_updatedDate\":\"2013-12-22T21:42:24.658Z\",\"_createdDate\":\"2013-12-22T21:42:24.658Z\" }"
            };

            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection("MyCollection");
            var pep = collection.Update(new HoistModel(new Dictionary<string, string>() { { "Name", "Bobby" }, { "_id", "1231313" }, { "_rev", "xx-xx" } }));

            Assert.AreEqual("Bobby", pep.Get("Name"));
            Assert.AreEqual("17", pep.Get("Age"));

            Assert.AreEqual("1231313", pep.Get("_id"));
            Assert.AreEqual("xx-xx", pep.Get("_rev"));

            Assert.AreEqual(1, httpLayer.Calls.Count, "To List Calls API");
            Assert.AreEqual("https://data.hoi.io/MyCollection/1231313", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual("{\"Name\":\"Bobby\",\"_id\":\"1231313\",\"_rev\":\"xx-xx\"}", httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
            Assert.AreEqual("POST", httpLayer.Calls[0].method);
        }

        [TestMethod]
        public void UpdateSuccessWithSystemParametersGoodTyped()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"Name\":\"Bobby\",\"Age\":\"17\",\"_id\":\"1231313\",\"_rev\":\"xx-xy\",\"_type\":\"MyCollection\",\"_updatedDate\":\"2013-12-22T21:42:24.658Z\",\"_createdDate\":\"2013-12-22T21:42:24.658Z\" }"
            };

            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection<PersonWithHoist>("MyCollection");
            var pep = collection.Update( new PersonWithHoist() { Name= "Bobby", _id= "1231313" , _rev =  "xx-xx" });

            Assert.AreEqual("Bobby", pep.Name);
            Assert.AreEqual(17, pep.Age);

            Assert.AreEqual("1231313", pep._id);
            Assert.AreEqual("xx-xy", pep._rev);

            Assert.AreEqual(1, httpLayer.Calls.Count, "To List Calls API");
            Assert.AreEqual("https://data.hoi.io/MyCollection/1231313", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual("{\"Name\":\"Bobby\",\"Age\":0,\"_id\":\"1231313\",\"_rev\":\"xx-xx\"}", httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
            Assert.AreEqual("POST", httpLayer.Calls[0].method);
        }

        [TestMethod]
        public void Update401WithSystemParametersRaiseException()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 401,
                WithWWWAuthenticate = true,
                Payload = ""
            };

            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection("MyCollection");
            var caughtException = false;
            try
            {
                var pep = collection.Update(new HoistModel(new Dictionary<string, string>() { { "Name", "Bobby" }, { "_id", "1231313" }, { "_rev", "xx-xx" } }));

            }
            catch (BadApiKeyException)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);
            Assert.AreEqual(1, httpLayer.Calls.Count, "To List Calls API");
            Assert.AreEqual("https://data.hoi.io/MyCollection/1231313", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual("{\"Name\":\"Bobby\",\"_id\":\"1231313\",\"_rev\":\"xx-xx\"}", httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
            Assert.AreEqual("POST", httpLayer.Calls[0].method);
        }

        [TestMethod]
        public void Update409WithSystemParametersRaiseException()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 409,
                WithWWWAuthenticate = false,
                Payload = ""
            };

            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection("MyCollection");
            var caughtException = false;
            try
            {
                var pep = collection.Update(new HoistModel(new Dictionary<string, string>() { { "Name", "Bobby" }, { "_id", "1231313" }, { "_rev", "xx-xx" } }));

            }
            catch (DataConflictException)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);
            Assert.AreEqual(1, httpLayer.Calls.Count, "To List Calls API");
            Assert.AreEqual("https://data.hoi.io/MyCollection/1231313", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual("{\"Name\":\"Bobby\",\"_id\":\"1231313\",\"_rev\":\"xx-xx\"}", httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
            Assert.AreEqual("POST", httpLayer.Calls[0].method);
        }

        [TestMethod]
        public void DeleteFailsWithExceptionOnBadApiKey()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 401,
                WithWWWAuthenticate = true,
                Payload = ""
            };

            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection("MyCollection");
            var caughtException = false;
            try
            {
                var pep = collection.Delete("1231313");

            }
            catch (BadApiKeyException)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);
            Assert.AreEqual(1, httpLayer.Calls.Count, "Delete Calls API");
            Assert.AreEqual("https://data.hoi.io/MyCollection/1231313", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual(null, httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
            Assert.AreEqual("DELETE", httpLayer.Calls[0].method);
            
        }

        [TestMethod]
        public void DeletePassesExpectedValueToApi()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{}"
            };

            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection("MyCollection");
            var wasDeleted = collection.Delete("1231313");
            Assert.IsTrue(wasDeleted);
            Assert.AreEqual(1, httpLayer.Calls.Count, "Delete Calls API");
            Assert.AreEqual("https://data.hoi.io/MyCollection/1231313", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual(null, httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
            Assert.AreEqual("DELETE", httpLayer.Calls[0].method);
        }

        [TestMethod]
        public void DeleteCollectionPassesExpectedValueToApi()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{}"
            };

            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection("MyCollection");
            var wasDeleted = collection.Delete();
            Assert.IsTrue(wasDeleted);
            Assert.AreEqual(1, httpLayer.Calls.Count, "Delete Calls API");
            Assert.AreEqual("https://data.hoi.io/MyCollection", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual(null, httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
            Assert.AreEqual("DELETE", httpLayer.Calls[0].method);
        }

        [TestMethod]
        public void UpdateRespectsForceParameter()
        {

            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"Name\":\"Bobby\",\"Age\":\"17\",\"_id\":\"1231313\",\"_rev\":\"xx-xy\",\"_type\":\"MyCollection\",\"_updatedDate\":\"2013-12-22T21:42:24.658Z\",\"_createdDate\":\"2013-12-22T21:42:24.658Z\" }"
            };

            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection<PersonWithHoist>("MyCollection");
            var pep = collection.Update(new PersonWithHoist() { Name = "Bobby", _id = "1231313", _rev = "xx-xx" }, true);

            Assert.AreEqual("Bobby", pep.Name);
            Assert.AreEqual(17, pep.Age);

            Assert.AreEqual("1231313", pep._id);
            Assert.AreEqual("xx-xy", pep._rev);

            Assert.AreEqual(1, httpLayer.Calls.Count, "Calls API");
            Assert.AreEqual("https://data.hoi.io/MyCollection/1231313?force=true", httpLayer.Calls[0].endpoint);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].apiKey);
            Assert.AreEqual("{\"Name\":\"Bobby\",\"Age\":0,\"_id\":\"1231313\",\"_rev\":\"xx-xx\"}", httpLayer.Calls[0].data);
            Assert.AreEqual(null, httpLayer.Calls[0].session);
            Assert.AreEqual("POST", httpLayer.Calls[0].method);

        }

        [TestMethod]
        public void UpdateWithDifferentKeys()
        {
            var httpLayer = new MockHttpLayer();
            httpLayer.Response = new ApiResponse
            {
                Code = 200,
                WithWWWAuthenticate = false,
                Payload = "{\"Name\":\"Bobby\",\"Age\":\"17\",\"_id\":\"1231313\",\"_rev\":\"xx-xy\",\"_type\":\"MyCollection\",\"_updatedDate\":\"2013-12-22T21:42:24.658Z\",\"_createdDate\":\"2013-12-22T21:42:24.658Z\" }"
            };

            var client = new HoistClient("MYAPI", httpLayer);
            var collection = client.GetCollection("CollectionX");
            var pep = collection.Update(new HoistModel(new Dictionary<string, string>() { { "Name", "Bobby" }, { "_id", "" }, { "_rev", "xx-xx" } }));
            Assert.AreEqual("https://data.hoi.io/CollectionX/", httpLayer.Calls[0].endpoint);
            pep = collection.Update(new HoistModel(new Dictionary<string, string>() { { "Name", "Bobby" }, { "_id", null }, { "_rev", "xx-xx" } }));
            Assert.AreEqual("https://data.hoi.io/CollectionX/", httpLayer.Calls[1].endpoint);
            pep = collection.Update(new HoistModel(new Dictionary<string, string>() { { "Name", "Bobby" }, { "_id", "-56-45?force=url" }, { "_rev", "xx-xx" } }));
            Assert.AreEqual("https://data.hoi.io/CollectionX/-56-45%3Fforce%3Durl", httpLayer.Calls[2].endpoint);

            collection = client.GetCollection("CollectionX?why");
            pep = collection.Update(new HoistModel(new Dictionary<string, string>() { { "Name", "Bobby" }, { "_id", "" }, { "_rev", "xx-xx" } }));
            Assert.AreEqual("https://data.hoi.io/CollectionX%3Fwhy/", httpLayer.Calls[3].endpoint);
            pep = collection.Update(new HoistModel(new Dictionary<string, string>() { { "Name", "Bobby" }, { "_id", null }, { "_rev", "xx-xx" } }));
            Assert.AreEqual("https://data.hoi.io/CollectionX%3Fwhy/", httpLayer.Calls[4].endpoint);
            pep = collection.Update(new HoistModel(new Dictionary<string, string>() { { "Name", "Bobby" }, { "_id", "-56-45?force=url" }, { "_rev", "xx-xx" } }));
            Assert.AreEqual("https://data.hoi.io/CollectionX%3Fwhy/-56-45%3Fforce%3Durl", httpLayer.Calls[5].endpoint);

        }




       
       
               
    }
}
