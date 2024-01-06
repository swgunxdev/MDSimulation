using System;
using System.Collections;
using MDModeling;

// #if !NUNIT
//using Microsoft.VisualStudio.TestTools.UnitTesting;
// using Category = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
// #else
using NUnit.Framework;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestContext = System.Object;
using TestProperty = NUnit.Framework.PropertyAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
// #endif


namespace MDModelingUnitTest
{
    
    
    /// <summary>
    ///This is a test class for LocationIdIteratorTest and is intended
    ///to contain all LocationIdIteratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LocationIdIteratorTest
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


        /// <summary>
        ///A test for LocationIdIterator Constructor
        ///</summary>
        [TestMethod()]
        public void LocationIdIteratorConstructorTest()
        {
            LocationID locId = new LocationID(1, 1, null);
            LocationIdIterator target = new LocationIdIterator(locId);
            Assert.IsNotNull(target);
            Assert.AreEqual(target.Current, locId[0]);
            Assert.IsFalse(target.MoveNext());
            Assert.IsTrue(target.IsDone());
        }


        /// <summary>
        ///A test for MoveNext
        ///</summary>
        [TestMethod()]
        public void MoveNextTest()
        {
            LocationID locId = new LocationID(new IdNode[] { new IdNode(1, 1), new IdNode(2, 1), new IdNode(3, 1), new IdNode(4, 1) });
            LocationIdIterator target = new LocationIdIterator(locId);
            int i = 1;
            while (target.IsDone() == false)
            {
                Assert.AreEqual(i, target.Current.Id);
                Assert.IsTrue(target.MoveNext());
                i += 1;
            }
            Assert.AreEqual(locId.Count, i);
        }

        /// <summary>
        ///A test for Reset
        ///</summary>
        [TestMethod()]
        public void ResetTest()
        {
            LocationID locId = new LocationID(new IdNode[] { new IdNode(1, 1), new IdNode(2, 1), new IdNode(3, 1), new IdNode(4, 1) });
            LocationIdIterator target = new LocationIdIterator(locId);
            Assert.AreEqual(1,target.Current.Id);
            Assert.IsTrue(target.MoveNext());
            Assert.AreEqual(2, target.Current.Id);
            Assert.IsTrue(target.MoveNext());
            Assert.AreEqual(3, target.Current.Id);
            target.Reset();
            Assert.That(()=> target.Current.Id, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        ///A test for Reset
        ///</summary>
        [TestMethod()]
        public void OriginalTest()
        {
            LocationID locId = new LocationID(new IdNode[] { new IdNode(1, 1), new IdNode(2, 1), new IdNode(3, 1), new IdNode(4, 1) });
            LocationIdIterator target = new LocationIdIterator(locId);
            Assert.AreEqual(1, target.Current.Id);
            Assert.IsTrue(target.MoveNext());
            Assert.AreEqual(2, target.Current.Id);
            Assert.IsTrue(target.MoveNext());
            Assert.AreEqual(3, target.Current.Id);
            Assert.AreEqual(locId, target.Original);
        }

        /// <summary>
        ///A test for Reset
        ///</summary>
        [TestMethod()]
        public void CurrentLocationTest()
        {
            LocationID locId = new LocationID(new IdNode[] { new IdNode(1, 1), new IdNode(2, 1), new IdNode(3, 1), new IdNode(4, 1) });
            LocationIdIterator target = new LocationIdIterator(locId);
            Assert.AreEqual(1, target.Current.Id);
            Assert.IsTrue(target.MoveNext());
            Assert.AreEqual(2, target.Current.Id);
            Assert.IsTrue(target.MoveNext());
            Assert.AreEqual(3, target.Current.Id);
            Assert.IsTrue(target.MoveNext());
			Assert.IsNotNull (target.CurrentLocation);
			Assert.IsNotNull(locId);
            Assert.IsTrue(locId == target.CurrentLocation);
        }
    }
}
