﻿using System;
using MetaDataModeling;
using MetaDataModeling.Utils;


#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Category = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
#else
using NUnit.Framework;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestContext = System.Object;
using TestProperty = NUnit.Framework.PropertyAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
#endif

namespace MetadataUnitTesting
{
    /// <summary>
    ///This is a test class for StringPropertyTest and is intended
    ///to contain all StringPropertyTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StringPropertyTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        //
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion Additional test attributes

        /// <summary>
        ///A test for StringProperty Constructor
        ///</summary>
        [TestMethod()]
        public void StringPropertyConstructorTest()
        {
            ushort id = 1;
            ushort typeId = 12;
            string name = "Bryce Dial Plan";
            string defaultValue = "Blah blah blah";
            StringProperty target = new StringProperty(id, typeId, name);
            target.Value = defaultValue;
            Assert.AreEqual(id, target.Id);
            Assert.AreEqual(typeId, target.TypeId);
            Assert.AreEqual(defaultValue, target.Value);
            Assert.AreEqual(name, target.Name);
        }

        /// <summary>
        ///A test for StringProperty Constructor
        ///</summary>
        [TestMethod()]
        public void StringPropertyConstructorTest1()
        {
            ushort id = 1;
            ushort typeId = 12;
            string name = "Bryce Dial Plan";
            string defaultValue = "Blah blah blaha";
            int min = 1;
            int max = 15;
            StringProperty target = new StringProperty(id, typeId, name, defaultValue, min, max);
            target.Value = defaultValue;
            Assert.AreEqual(id, target.Id);
            Assert.AreEqual(typeId, target.TypeId);
            Assert.AreEqual(defaultValue, target.Value);
            Assert.AreEqual(name, target.Name);
            Assert.IsTrue(target.IsValid());
        }

        /// <summary>
        ///A test for IsValid
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidValueException))]
        public void IsValidTest()
        {
            ushort id = 1;
            ushort typeId = 12;
            string name = "Bryce Dial Plan";
            string defaultValue = "Blah blah blaha";

            int min = 1;
            int max = 15;
            StringProperty target = new StringProperty(id, typeId, name, defaultValue, min, max);
            target.Value = defaultValue;
            Assert.AreEqual(id, target.Id);
            Assert.AreEqual(typeId, target.TypeId);
            Assert.AreEqual(defaultValue, target.Value);
            Assert.AreEqual(name, target.Name);
            Assert.IsTrue(target.IsValid());
            target.Value = "123456789ABCDEFG";
            Assert.IsTrue(target.IsValid());
            Assert.AreEqual(defaultValue, target.Value);
        }

        /// <summary>
        ///A test for StringProperty Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidValueException))]
        public void StringPropertyCopyConstructorTest()
        {
            ushort id = 1;
            ushort typeId = 12;
            string name = "Bryce Dial Plan";
            string defaultValue = "Blah blah blaha";

            int min = 1;
            int max = 15;
            StringProperty rhs = new StringProperty(id, typeId, name, defaultValue, min, max);
            rhs.Value = defaultValue;
            StringProperty target = new StringProperty(rhs);
            Assert.AreEqual(id, target.Id);
            Assert.AreEqual(typeId, target.TypeId);
            Assert.AreEqual(defaultValue, target.Value);
            Assert.AreEqual(name, target.Name);
            Assert.IsTrue(target.IsValid());
            target.Value = "123456789ABCDEFG";
            Assert.IsTrue(target.IsValid());
            Assert.AreEqual(rhs, target);
            Assert.IsFalse(target.SetValue("123456789ABCDEFG"));
            Assert.IsTrue(target.SetValue("123456789ABCDEF"));
            Assert.IsTrue(target.IsValid());
            Assert.AreNotEqual(rhs, target);
        }

        protected StringProperty GenerateStringProperty()
        {
            ushort id = GenRadom.GenRandomUshort(32000);
            ushort type = GenRadom.GenRandomUshort(32000);
            string name = GenRadom.GenRandomString(5);
            return new StringProperty(id, type, name);
        }

        [TestMethod]
        public void TestStringPropToByteArray()
        {
            StringProperty target = GenerateStringProperty();
            target.Value = GenRadom.GenRandomString(25);

            byte[] data = target.ToByteArray();

            var actual = new StringProperty();
            actual.FromByteArray(data, 0);

            Assert.AreEqual(target, actual);
        }

        [TestMethod]
        public void TestStringPropSerialization()
        {
            StringProperty target = GenerateStringProperty();
            string testValue = GenRadom.GenRandomString(25);
            target.SetValue(testValue);
            Assert.AreEqual(testValue, target.GetValue());
            Assert.AreEqual(target.Value, target.GetValue());
        }
    }
}