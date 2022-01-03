using System;
using System.IO;
using MDModeling;
// #if !NUNIT
// using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    ///This is a test class for LocationIDTest and is intended
    ///to contain all LocationIDTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LocationIDTest
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
        ///A test for LocationID Constructor
        ///</summary>
        [TestMethod()]
        public void LocationIDConstructorTest()
        {
            ushort id = 1;
            ushort type = 2;
            LocationID target = new LocationID(id, type, null);
            Assert.AreEqual(id, target.NodeId);
            Assert.AreEqual(type, target.NodeType);
            Assert.AreEqual(1, target.Count);
        }

        /// <summary>
        ///A test for LocationID Constructor
        ///</summary>
        [TestMethod()]
        public void LocationIDConstructorTest1()
        {
            LocationID parent = new LocationID(new IdNode[] { new IdNode(1, 2), new IdNode(2, 3) });
            LocationID target = new LocationID(parent);
            Assert.AreEqual(parent[0].Id, target[0].Id);
            Assert.AreEqual(parent[0].Type, target[0].Type);
            Assert.AreEqual(parent[1].Id, target[1].Id);
            Assert.AreEqual(parent[1].Type, target[1].Type);
            Assert.AreEqual(2, target.Count);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            LocationID target = new LocationID(new IdNode[] { new IdNode(1, 2), new IdNode(2, 3) });
            LocationID x = new LocationID(target);
            LocationID y = new LocationID(target);
            bool expected = true;
            bool actual;
            actual = target.Equals(x, y);
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(x.Equals(y));
            Assert.IsTrue(x == y);
			Assert.IsFalse( x != y);
			Assert.AreEqual(x,y);
            y = null;
            Assert.IsTrue(x != y);
            Assert.IsFalse(x == y);
        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod()]
        public void GetHashCodeTest()
        {
            LocationID target = new LocationID(new IdNode[] { new IdNode(1, 2), new IdNode(2, 3) });
            LocationID obj = new LocationID(target);
            int expected = 1088;
            int actual;
            actual = target.GetHashCode(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod()]
        public void GetHashCodeTest1()
        {
            LocationID target = new LocationID(new IdNode[] { new IdNode(1, 2), new IdNode(2, 3) });
            int expected = 1088;
            int actual;
            actual = target.GetHashCode();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SetParent
        ///</summary>
        [TestMethod()]
        public void SetParentTest()
        {
            IdNode [] parentData = new IdNode[] { new IdNode(1, 2), new IdNode(2, 3) };
            LocationID parent = new LocationID(parentData);
            LocationID target = new LocationID(1,5,null);
            target.SetParent(parent);
            for (int i = 0; i < parentData.Length; i++)
            {
                Assert.AreEqual(parentData[i], target[i]);
            }
        }

        /// <summary>
        ///A test for NodeId
        ///</summary>?
        [TestMethod()]
        public void NodeIdTest()
        {
            LocationID target = new LocationID(1,1,null);
            ushort expected = 1;
            ushort actual;
            target.NodeId = expected;
            actual = target.NodeId;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for NodeType
        ///</summary>
        [TestMethod()]
        public void NodeTypeTest()
        {
            LocationID target = new LocationID(1, 1, null);
            ushort expected = 1;
            ushort actual;
            target.NodeType = expected;
            actual = target.NodeType;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToByteArrayTest()
        {
            LocationID expected = new LocationID(new IdNode[] { new IdNode(1, 2), new IdNode(2, 3) });
            LocationID target = new LocationID();
            byte[] data = expected.ToByteArray();
            int consumed = target.FromByteArray(data,0);
            Assert.AreEqual(12, consumed);
			Assert.IsTrue(expected.Equals(target));
            Assert.AreEqual(expected, target);
        }

#if MSGPACK        
        [TestMethod]
        public void ToWriterTest()
        {
            LocationID expected = new LocationID(new IdNode[] { new IdNode(1, 2), new IdNode(2, 3) });
            LocationID target = new LocationID();
            byte[] data = expected.ToByteArray();
            MemoryStream ms = new MemoryStream();
            MsgPackWriter mp = new MsgPackWriter(ms);
            target.ToWriter(mp);
            byte[] arrayData = ms.ToArray();
            ms = new MemoryStream(data);
            MsgPackReader reader = new MsgPackReader(ms);
            target.FromReader(reader);
            Assert.AreEqual(expected, target);
        }
#endif

        /// <summary>
        ///A test for CompareTo
        ///</summary>
        [TestMethod()]
        public void CompareToTest()
        {
            LocationID target = new LocationID(1,2,null);
            LocationID other = new LocationID(1,2,null);
            int expected = 0;
            Assert.AreEqual(expected, target.CompareTo(other));
            target = new LocationID(1, 2, null);
            other = new LocationID(2,2,null);
            expected = 1;
            Assert.AreEqual(expected, target.CompareTo(other));
            target = new LocationID(new IdNode[] { new IdNode(1, 2), new IdNode(2, 1) });
            other = new LocationID(new IdNode[] { new IdNode(1, 2), new IdNode(1, 1) });
            expected = -1;
            Assert.AreEqual(expected, target.CompareTo(other));
            target = new LocationID(new IdNode[] { new IdNode(1, 2), new IdNode(2, 1) });
            other = new LocationID(new IdNode[] { new IdNode(2, 2), new IdNode(1, 1) });
            expected = 1;
            Assert.AreEqual(expected, target.CompareTo(other));
            target = new LocationID(new IdNode[] { new IdNode(1, 2),});
            other = new LocationID(new IdNode[] { new IdNode(2, 2), new IdNode(1, 1) });
            expected = 1;
            Assert.AreEqual(expected, target.CompareTo(other));
            target = new LocationID(new IdNode[] { new IdNode(1, 2), new IdNode(2, 1) });
            other = new LocationID(new IdNode[] { new IdNode(1, 2), new IdNode(1, 1) });
            expected = -1;
            Assert.AreEqual(expected, target.CompareTo(other));
        }
    }
}
