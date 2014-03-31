using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hoist.Api;
using Hoist.Api.Model;
using System.Collections.Generic;
using Hoist.Api.Http;

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

        [TestMethod]
        public void CanGetCollection()
        {
            var mockHttp = new MockHttpLayer();
            var client = new Hoist("MYAPI", mockHttp);
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
            var client = new Hoist("MYAPI", mockHttp);
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

            var client = new Hoist("MYAPI", httpLayer);
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
            Assert.AreEqual("https://data.hoi.io/MyCollection", httpLayer.Calls[0].Item1);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].Item2);
            Assert.AreEqual( "{\"k1\":\"v1\"}" , httpLayer.Calls[0].Item3);
            Assert.AreEqual(null, httpLayer.Calls[0].Item4);

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

            var client = new Hoist("MYAPI", httpLayer);
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
            Assert.AreEqual("https://data.hoi.io/MyCollection", httpLayer.Calls[0].Item1);
            Assert.AreEqual("MYAPI", httpLayer.Calls[0].Item2);
            Assert.AreEqual("{\"Name\":\"Andrew\",\"Age\":34}", httpLayer.Calls[0].Item3);
            Assert.AreEqual(null, httpLayer.Calls[0].Item4);
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


            var client = new Hoist("MYAPI", httpLayer);
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
            Assert.AreEqual("https://data.hoi.io/MyCollection", httpLayer.Calls[1].Item1);
            Assert.AreEqual("MYAPI", httpLayer.Calls[1].Item2);
            Assert.AreEqual("{\"Name\":\"Andrew\",\"Age\":34}", httpLayer.Calls[1].Item3);
            Assert.AreEqual(hoistSession, httpLayer.Calls[1].Item4);
        }



        [TestMethod]
        public void CanLoopOverCollection()
        {

        }


        
    }
}
