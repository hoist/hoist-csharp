using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Hoist.Api.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hoist.Api.Test
{
    [TestClass]
    public class SerialisationTests
    {

        public class Person
        {
            public int PersonID { get; set; }
            public string Name { get; set; }
            public bool Registered { get; set; }
        }

        public class Org
        {
            public string Name { get; set; }
            public List<Person> Employees { get; set; }
        }
        
        [TestMethod]
        public void ToJsonReturnsWhatsExpected()
        {
            var serializer = new JavaScriptSerializer();
            var serializedResult = serializer.Serialize(new Person { PersonID = 123, Name = "ABC", Registered = false });
            Assert.AreEqual("{\"PersonID\":123,\"Name\":\"ABC\",\"Registered\":false}", serializedResult);
        }

        [TestMethod]
        public void ToJsonReturnsWhatsExpectedWithObject()
        {
            var serializer = new JavaScriptSerializer();
            Object obj = new Person { PersonID = 123, Name = "ABC", Registered = false };
            var serializedResult = serializer.Serialize(obj);
            Assert.AreEqual("{\"PersonID\":123,\"Name\":\"ABC\",\"Registered\":false}", serializedResult);
        }



        [TestMethod]
        public void FromJsonReturnsWhatsExpected()
        {
            var serializer = new JavaScriptSerializer();
            var serializedResult = serializer.Deserialize<Person>("{\"PersonID\":123,\"Name\":\"ABC\",\"Registered\":false}");
            Assert.AreEqual(123, serializedResult.PersonID);
            Assert.AreEqual("ABC", serializedResult.Name);
            Assert.AreEqual(false, serializedResult.Registered);
        }

        [TestMethod]
        public void MissingProperties()
        {
            var serializer = new JavaScriptSerializer();
            var serializedResult = serializer.Deserialize<Person>("{\"PersonID\":123,\"Name\":\"ABC\"}");
            
            Assert.AreEqual(123, serializedResult.PersonID);
            Assert.AreEqual("ABC", serializedResult.Name);
        }

        [TestMethod]
        public void DifferentCaseProperties()
        {
            var serializer = new JavaScriptSerializer();
            var serializedResult = serializer.Deserialize<Person>("{\"personid\":123,\"name\":\"ABC\"}");

            Assert.AreEqual(123, serializedResult.PersonID);
            Assert.AreEqual("ABC", serializedResult.Name);
        }

        [TestMethod]
        public void AdditionalProperties()
        {
            var serializer = new JavaScriptSerializer();
            var serializedResult = serializer.Deserialize<Person>("{\"PersonID\":123,\"Name\":\"ABC\", \"SomeProperty\":123}");
            Assert.AreEqual(123, serializedResult.PersonID);
            Assert.AreEqual("ABC", serializedResult.Name);
        }

        [TestMethod]
        public void NestedObjects()
        {
            var serializer = new JavaScriptSerializer();
            var serializedResult = serializer.Deserialize<Org>("{'Name':'MyOrg', 'Employees':[ {\"PersonID\":123,\"Name\":\"ABC\",\"Registered\":false}, {\"PersonID\":124,\"Name\":\"DEF\",\"Registered\":false} ]}");
            Assert.AreEqual("MyOrg", serializedResult.Name);
            Assert.AreEqual(2, serializedResult.Employees.Count);
            Assert.AreEqual("ABC", serializedResult.Employees[0].Name);
            Assert.AreEqual("DEF", serializedResult.Employees[1].Name);

        }

        [TestMethod]
        public void HoistModelCanSerialise()
        {            
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new List<JavaScriptConverter>() { new HoistModelJavaScriptConverter() });
            var serializedResult = serializer.Serialize( new HoistModel() );
            Assert.AreEqual("{}", serializedResult);

            serializedResult = serializer.Serialize(new HoistModel( new Dictionary<string,string>() {{"Name","Jack"},{"Key","Value"}} ));
            Assert.AreEqual("{\"Name\":\"Jack\",\"Key\":\"Value\"}", serializedResult);

            serializedResult = serializer.Serialize(new HoistModel(new Dictionary<string, string> { { "k1", "v1" } }));
            Assert.AreEqual("{\"k1\":\"v1\"}", serializedResult);

        }

        [TestMethod]
        public void HoistModelCanDeSerialise()
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new List<JavaScriptConverter>() { new HoistModelJavaScriptConverter() });
            var serializedResult = serializer.Deserialize<HoistModel>("{}");
            Assert.AreEqual(0 , serializedResult.Keys.Count);

            serializedResult = serializer.Deserialize<HoistModel>("{\"Name\":\"Jack\",\"Key\":\"Value\"}");
            Assert.AreEqual(2, serializedResult.Keys.Count);
            Assert.AreEqual("Jack", serializedResult.Get("Name"));
            Assert.AreEqual("Value", serializedResult.Get("Key"));

        }

        [TestMethod]
        public void SerialiseEmptyString()
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new List<JavaScriptConverter>() { new HoistModelJavaScriptConverter() });
            var serializedResult = serializer.Deserialize<HoistModel>("");
            Assert.IsNull(serializedResult);
        }


       

    }
}
