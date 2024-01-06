using System;
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
using System.ComponentModel;
#endif


namespace MetadataUnitTesting
{

    
    
    /// <summary>
    ///This is a test class for IdTypeInfoTest and is intended
    ///to contain all IdTypeInfoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IdTypeInfoTest
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
        #endregion


        internal virtual IdType CreateIdTypeInfo()
        {
            IdType target = new GenericProperty<int>();
            return target;
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            IdType target = CreateIdTypeInfo();
            IdType x = new GenericProperty<int>(1, 2, "fred", 5);
            IdType y = new GenericProperty<int>(1, 2, "fred", 5);
            bool expected = true;
            bool actual;
            actual = target.Equals(x, y);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void EqaulityTest()
        {

        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod()]
        public void GetHashCodeTest()
        {
            IdType target = CreateIdTypeInfo();
            IdType obj = new GenericProperty<int>(1, 2, "fred", 5);
            int expected = 33; 
            int actual;
            actual = target.GetHashCode(obj);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IdChangedEventTest()
        {
            ushort id = GenRadom.GenRandomUshort(345);
            ushort type = GenRadom.GenRandomUshort(345);
            string name = GenRadom.GenRandomString(12);
            EventChangeCounter counter = new EventChangeCounter();
            IdType target = new GenericProperty<int>(id,type,name,0);
            target.PropertyChanged += (counter.EventChangedHandler);
            target.Id = GenRadom.GenRandomUshort(345);
            target.TypeId = GenRadom.GenRandomUshort(345);
            target.Name = GenRadom.GenRandomString(12);
            Assert.AreEqual(3,counter.Counter);
        }
    }
}
