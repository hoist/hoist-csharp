using System;
using Hoist.Api.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hoist.Api.Test
{
    
    [TestClass]
    public class ExtractPropertiesTests
    {
        //Declare lots of types of class so that we can check if the reflection Code is working the way we want
        class WithPrivateIdFeilds
        {
            string _id;
            string a;
            int b;

            public WithPrivateIdFeilds()
            {
                _id = "10";
                a = "5";
                b = 7;
            }

            public override string ToString()
            {
                return String.Format("{0},{1},{2}", _id, a, b);
            }
        }

        class WithFields
        {
            public string _id;
            public string a;
            public string b;

            public WithFields()
            {
                _id = "";
                a = "";
                b = "";
            }
        }

        class WithProperties
        {
            public int _id { get; set; }
            public string _rev { get; set; }
            public string Name { get; set; }
            public string LongName { get; set; }
            public WithFields Fields { get; set; }
        }
        
        [TestMethod]
        public void TestWithPrivateFeilds()
        {
            var obj = new WithPrivateIdFeilds();
            TestObj(obj);
        }

        [TestMethod]
        public void TestWithPublicFeilds()
        {
            var obj = new WithFields();
            TestObj(obj, 3, "");

        }

        [TestMethod]
        public void TestWithIdSet()
        {
            var obj = new WithFields();
            obj._id = "ABC";
            TestObj(obj, 3, "ABC");
        }

        [TestMethod]
        public void TestWithProperties()
        {
            var obj = new WithProperties();
            obj._id = 123;
            TestObj(obj, 5, "123");
        }

        private static void TestObj<T>(T obj, int expectedCount =0, string expectedId = "")
        {
            var id = "";
            var x = ReflectionUtils.ExtractProperties(obj, ref id);
            Assert.AreEqual(expectedCount, x.Count);
            Assert.AreEqual(expectedId, id);
        }
    }
}
