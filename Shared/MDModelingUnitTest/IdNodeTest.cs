using System;
using MDModeling;
using MDModeling.Utils;


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
    ///This is a test class for IdNodeTest and is intended
    ///to contain all IdNodeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IdNodeTest
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
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            ushort id = GenRadom.GenRandomUshort(345);
            ushort type = GenRadom.GenRandomUshort(345);
            IdNode target = new IdNode(id, type);
            IdNode other = new IdNode(id, type);
            bool expected = true;
            bool actual;
            actual = target.Equals(other);
            Assert.AreEqual(expected, actual);
			Assert.AreEqual(target,other);
        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod()]
        public void GetHashCodeTest()
        {
            ushort id = GenRadom.GenRandomUshort(345);
            ushort type = GenRadom.GenRandomUshort(345);
            IdNode target = new IdNode();
            IdNode obj = new IdNode(id, type);
            int expected = HashHelper.GetHashCode<ushort, ushort>(id, type);
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
            ushort id = GenRadom.GenRandomUshort(345);
            ushort type = GenRadom.GenRandomUshort(345);
            IdNode target = new IdNode(id, type);
            int expected = HashHelper.GetHashCode<ushort, ushort>(id, type);
            int actual;
            actual = target.GetHashCode();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod()]
        public void op_EqualityTest()
        {
            ushort id = GenRadom.GenRandomUshort(345);
            ushort type = GenRadom.GenRandomUshort(345);
            //IdNode target = new IdNode(id, type);
            //IdNode other = new IdNode(id, type);
            IdNode id1 = new IdNode(id, type);
            IdNode id2 = new IdNode(id, type);
            Assert.IsTrue(id1 == id2);
            id2 = null;
            Assert.IsFalse(id1 == id2);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod()]
        public void op_InequalityTest()
        {
            ushort id = GenRadom.GenRandomUshort(345);
            ushort type = GenRadom.GenRandomUshort(345);
            //IdNode target = new IdNode(id, type);
            //IdNode other = new IdNode(id, type);
            IdNode id1 = new IdNode(id, type);
            IdNode id2 = new IdNode(id, type);
            Assert.IsFalse(id1 != id2);
            id2 = null;
            Assert.IsTrue(id1 != id2);
        }

        /// <summary>
        ///A test for Id and Type
        ///</summary>
        [TestMethod()]
        public void IdTest()
        {
            ushort id = GenRadom.GenRandomUshort(345);
            ushort type = GenRadom.GenRandomUshort(345);
            IdNode target = new IdNode(id, type);

            Assert.AreEqual(id, target.Id);
            Assert.AreEqual(type, target.Type);
        }

        [TestMethod]
        public void ToByteArrayTest()
        {
            ushort id = GenRadom.GenRandomUshort(345);
            ushort type = GenRadom.GenRandomUshort(345);
            IdNode target = new IdNode(id, type);
            byte[] data = target.ToByteArray();
            IdNode actaul = new IdNode();
            int consumed = actaul.FromByteArray(data, 0);
            Assert.AreEqual(target, actaul);
            Assert.AreEqual(4, consumed);
        }
    }
}
