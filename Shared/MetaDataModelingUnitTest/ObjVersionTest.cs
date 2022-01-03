//
// File Name: ObjVersionTest.cs
// ----------------------------------------------------------------------

using System;
using MetaDataModeling;
using MetaDataModeling.Utils;
using System.Linq;

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
    ///This is a test class for ObjVersionTest and is intended
    ///to contain all ObjVersionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ObjVersionTest
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
        ///A test for ObjVersion Constructor
        ///</summary>
        [TestMethod()]
        public void ObjVersionConstructorTest()
        {
            ObjVersion target = new ObjVersion();
            Assert.IsNotNull(target);
            Assert.AreEqual(0, target.Major);
            Assert.AreEqual(0, target.Minor);
            Assert.AreEqual(0, target.Revision);
            Assert.AreEqual(1, target.Build);
        }

        /// <summary>
        ///A test for ObjVersion Constructor
        ///</summary>
        [TestMethod()]
        public void ObjVersionConstructorTest1()
        {
            byte major = GenRadom.GenRandomByte(0, 255);
            byte minor = GenRadom.GenRandomByte(0, 255);
            byte revision = GenRadom.GenRandomByte(0, 255);
            byte build = GenRadom.GenRandomByte(0, 255);
            ObjVersion target = new ObjVersion(major, minor, revision, build);
            Assert.AreEqual(major, target.Major);
            Assert.AreEqual(minor, target.Minor);
            Assert.AreEqual(revision, target.Revision);
            Assert.AreEqual(build, target.Build);
        }

        /// <summary>
        ///A test for CompareTo
        ///</summary>
        [TestMethod()]
        public void CompareToTest()
        {
            // test the equals compare
            ObjVersion target = new ObjVersion(1,2,3,4);
            ObjVersion other = new ObjVersion(1,2,3,4);
            Assert.AreEqual(0, target.CompareTo(other));

            // test the greater Major compare
            target = new ObjVersion(2, 2, 3, 4);
            other = new ObjVersion(1, 2, 3, 4);
            Assert.AreEqual(1, target.CompareTo(other));

            // test the less then Major compare
            target = new ObjVersion(1, 2, 3, 4);
            other = new ObjVersion(2, 2, 3, 4);
            Assert.AreEqual(-1, target.CompareTo(other));

            // test the greater Minor compare
            target = new ObjVersion(1, 3, 3, 4);
            other = new ObjVersion(1, 2, 3, 4);
            Assert.AreEqual(1, target.CompareTo(other));

            // test the less than Minor compare
            target = new ObjVersion(1, 1, 3, 4);
            other = new ObjVersion(1, 2, 3, 4);
            Assert.AreEqual(-1, target.CompareTo(other));

            // test the greater Revision compare
            target = new ObjVersion(1, 2, 3, 4);
            other = new ObjVersion(1, 2, 2, 4);
            Assert.AreEqual(1, target.CompareTo(other));

            // test the less than Revision compare
            target = new ObjVersion(1, 2, 2, 4);
            other = new ObjVersion(1, 2, 3, 4);
            Assert.AreEqual(-1, target.CompareTo(other));

            // test the greater Build compare
            target = new ObjVersion(1, 2, 3, 4);
            other = new ObjVersion(1, 2, 3, 3);
            Assert.AreEqual(1, target.CompareTo(other));

            // test the less than Build compare
            target = new ObjVersion(1, 2, 3, 3);
            other = new ObjVersion(1, 2, 3, 4);
            Assert.AreEqual(-1, target.CompareTo(other));
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            ObjVersion target = new ObjVersion();
            ObjVersion x = GenRadom.GenObjVersion();
            ObjVersion y = new ObjVersion(x);
            bool expected = true;
            bool actual;
            actual = target.Equals(x, y);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest1()
        {
            ObjVersion target = GenRadom.GenObjVersion();
            ObjVersion other = new ObjVersion(target);
            bool expected = true;
            bool actual;
            actual = target.Equals(other);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest2()
        {
            ObjVersion target = GenRadom.GenObjVersion();
            object obj = target;
            bool expected = true;
            bool actual;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FromByteArray
        ///</summary>
        [TestMethod()]
        public void FromByteArrayTest()
        {
            ObjVersion target = GenRadom.GenObjVersion();
            byte[] data = target.ToByteArray();

            int offset = 0;
            int expected = 4;
            int actual;
            ObjVersion actualObj = new ObjVersion();

            actual = actualObj.FromByteArray(data, offset);
            Assert.AreEqual(target, actualObj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod()]
        public void GetHashCodeTest()
        {
            ObjVersion target = GenRadom.GenObjVersion();
            ObjVersion obj = new ObjVersion();
            int expected = 1;
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
            ObjVersion target = new ObjVersion();
            int expected = 1;
            int actual;
            actual = target.GetHashCode();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ToByteArray
        ///</summary>
        [TestMethod()]
        public void ToByteArrayTest()
        {
            ObjVersion target = new ObjVersion();
            byte[] expected = new byte[4] { 0, 0, 0, 1 };
            byte[] actual = null;
            actual = target.ToByteArray();

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            ObjVersion target = new ObjVersion();
            string expected = "0.0.0.1";
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod()]
        public void op_EqualityTest()
        {
            ObjVersion ver1 = GenRadom.GenObjVersion();
            ObjVersion ver2 = new ObjVersion(ver1);
            Assert.IsTrue(ver1 == ver2);

            ver1 = new ObjVersion(1,0,0,0);
            ver2 = new ObjVersion();
            Assert.IsFalse(ver1 == ver2);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod()]
        public void op_InequalityTest()
        {
            ObjVersion ver1 = GenRadom.GenObjVersion();
            ObjVersion ver2 = new ObjVersion(ver1);
            Assert.IsFalse(ver1 != ver2);

            ver1 = new ObjVersion(1, 0, 0, 0);
            ver2 = new ObjVersion();
            Assert.IsTrue(ver1 != ver2);
        }

        /// <summary>
        ///A test for Build
        ///</summary>
        [TestMethod()]
        public void BuildTest()
        {
            ObjVersion target = new ObjVersion();
            byte expected = GenRadom.GenRandomByte(0, 255);
            byte actual;
            target.Build = expected;
            actual = target.Build;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Major
        ///</summary>
        [TestMethod()]
        public void MajorTest()
        {
            ObjVersion target = new ObjVersion();
            byte expected = GenRadom.GenRandomByte(0, 255);
            byte actual;
            target.Major = expected;
            actual = target.Major;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Minor
        ///</summary>
        [TestMethod()]
        public void MinorTest()
        {
            ObjVersion target = new ObjVersion();
            byte expected = GenRadom.GenRandomByte(0, 255);
            byte actual;
            target.Minor = expected;
            actual = target.Minor;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Revision
        ///</summary>
        [TestMethod()]
        public void RevisionTest()
        {
            ObjVersion target = new ObjVersion();
            byte expected = GenRadom.GenRandomByte(0, 255);
            byte actual;
            target.Revision = expected;
            actual = target.Revision;
            Assert.AreEqual(expected, actual);
        }
    }
}
