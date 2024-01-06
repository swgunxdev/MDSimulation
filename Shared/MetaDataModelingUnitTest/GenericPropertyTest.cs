using System;
using System.IO;
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
    ///This is a test class for GenericPropertyTest and is intended
    ///to contain all GenericPropertyTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GenericPropertyTest
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
        public void EqualsTestHelper<T>(T value)
        {
            GenericProperty<T> target = new GenericProperty<T>();
            GenericProperty<T> x = GenerateGenericProperty<T>();
            x.Value = value;
            GenericProperty<T> y = new GenericProperty<T>(x);
            bool expected = true;
            bool actual;
            actual = target.Equals(x, y);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            EqualsTestHelper<int>(5);
            EqualsTestHelper<float>(5.0f);
            EqualsTestHelper<double>(578655456.0);
            EqualsTestHelper<byte>(5);
            EqualsTestHelper<byte>(255);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        public void EqualsTest1Helper<T>(T value)
        {
            GenericProperty<T> target = GenerateGenericProperty<T>();
            target.Value = value;
            GenericProperty<T> other = new GenericProperty<T>(target);
            bool expected = true;
            bool actual;
            actual = target.Equals(other);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void EqualsTest1()
        {
            EqualsTest1Helper<int>(5);
            EqualsTest1Helper<float>(5.0f);
            EqualsTest1Helper<double>(578655456.0);
            EqualsTest1Helper<byte>(5);
            EqualsTest1Helper<byte>(255);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        public void EqualsTest2Helper<T>(T value)
        {
            GenericProperty<T> target = GenerateGenericProperty<T>();
            target.Value = value;
            object obj = target;
            bool expected = true;
            bool actual;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void EqualsTest2()
        {
            EqualsTest1Helper<int>(GenRadom.GenRandomInt(3424));
            EqualsTest1Helper<float>(GenRadom.NextFloat());
            EqualsTest1Helper<double>(GenRadom.NextFloat());
            EqualsTest1Helper<byte>(32);
            EqualsTest1Helper<byte>(43);
        }

        ///// <summary>
        /////A test for GetHashCode
        /////</summary>
        //public void GetHashCodeTestHelper<T>()
        //{
        //    GenericProperty<T> target = new GenericProperty<T>(); // TODO: Initialize to an appropriate value
        //    GenericProperty<T> mdObj = null; // TODO: Initialize to an appropriate value
        //    int expected = 0; // TODO: Initialize to an appropriate value
        //    int actual;
        //    actual = target.GetHashCode(mdObj);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        //[TestMethod()]
        //public void GetHashCodeTest()
        //{
        //    GetHashCodeTestHelper<GenericParameterHelper>();
        //}

        ///// <summary>
        /////A test for GetHashCode
        /////</summary>
        //public void GetHashCodeTest1Helper<T>()
        //{
        //    GenericProperty<T> target = new GenericProperty<T>(); // TODO: Initialize to an appropriate value
        //    int expected = 0; // TODO: Initialize to an appropriate value
        //    int actual;
        //    actual = target.GetHashCode();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        //[TestMethod()]
        //public void GetHashCodeTest1()
        //{
        //    GetHashCodeTest1Helper<GenericParameterHelper>();
        //}

        /// <summary>
        ///A test for SetValue
        ///</summary>
        public void SetValueTestHelper<T>(T value, bool expectedOutCome)
        {
            GenericProperty<T> target = new GenericProperty<T>();
            bool actual;
            actual = target.SetValue(value);
            Assert.AreEqual(expectedOutCome, actual);
        }

        [TestMethod()]
        public void SetValueTest()
        {
            SetValueTestHelper<int>(5, true);
            SetValueTestHelper<float>(5.0f, true);
            SetValueTestHelper<double>(578655456.0, true);
            SetValueTestHelper<byte>(5, true);
            SetValueTestHelper<byte>(255, true);
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        public void op_EqualityTestHelper<T>(T value)
        {
            GenericProperty<T> generic1 = GenerateGenericProperty<T>();
            generic1.Value = value;
            GenericProperty<T> generic2 = new GenericProperty<T>(generic1);
            bool expected = true;
            bool actual;
            actual = (generic1 == generic2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void op_EqualityTest()
        {
            op_EqualityTestHelper<int>(5);
            op_EqualityTestHelper<float>(5.0f);
            op_EqualityTestHelper<double>(578655456.0);
            op_EqualityTestHelper<byte>(5);
            op_EqualityTestHelper<byte>(255);
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        public void op_InequalityTestHelper<T>()
        {
            GenericProperty<T> generic1 = GenerateGenericProperty<T>();
            GenericProperty<T> generic2 = GenerateGenericProperty<T>();
            bool expected = true;
            bool actual;
            actual = (generic1 != generic2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void op_InequalityTest()
        {
            op_InequalityTestHelper<int>();
        }

        /// <summary>
        ///A test for Value
        ///</summary>
        public void ValueTestHelper<T>(T value)
        {
            GenericProperty<T> target = GenerateGenericProperty<T>();
            T expected = value;
            T actual;
            target.Value = expected;
            actual = target.Value;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GenericPropertyValueTest()
        {
            ValueTestHelper<int>(500);
            ValueTestHelper<float>(6.787f);
            ValueTestHelper<Byte>(234);
        }

        public GenericProperty<T> GenerateGenericProperty<T>()
        {
            ushort id = GenRadom.GenRandomUshort(32000);
            ushort type = GenRadom.GenRandomUshort(32000);
            string name = GenRadom.GenRandomString(5);
            return new GenericProperty<T>(id, type, name, default(T));
        }

        [TestMethod]
        public void TestSerialization()
        {
            SerializeTestHelper<bool>(true);
            SerializeTestHelper<ushort>(GenRadom.GenRandomUshort(32000));
            SerializeTestHelper<int>(GenRadom.GenRandomInt(32000));
            SerializeTestHelper<long>(GenRadom.GenRandomInt(64000));
            SerializeTestHelper<ulong>((ulong)GenRadom.GenRandomInt(640000));
        }

        /// <summary>
        ///A test for Value
        ///</summary>
        public void SerializeTestHelper<T>(T value)
        {
            GenericProperty<T> target = GenerateGenericProperty<T>();
            target.Value = value;
            byte[] data = MetaDataPayload.ToByteArray(target);

            var actual = MetaDataPayload.FromByteArray(data);

            Assert.AreEqual(target, actual.Item1);
        }

        /// <summary>
        ///A test for Value
        ///</summary>
        public void SerializeTwoHelper<T>(T value)
        {
            GenericProperty<T> target = GenerateGenericProperty<T>();
            target.Value = value;
            byte[] data = target.ToByteArray();

            GenericProperty<T> target2 = GenerateGenericProperty<T>();
            target2.Value = value;

            byte[] data2 = target2.ToByteArray();

            MemoryStream ms = new MemoryStream();

            ms.Write(data, 0, data.Length);
            ms.Write(data2, 0, data2.Length);

            byte[] twoData = ms.ToArray();

            GenericProperty<T> actual = new GenericProperty<T>();
            int curPos = actual.FromByteArray(twoData, 0);
            GenericProperty<T> actual2 = new GenericProperty<T>();
            curPos = actual2.FromByteArray(twoData, curPos);

            Assert.AreEqual(target, actual);
            Assert.AreEqual(target2, actual2);
        }

        [TestMethod]
        public void TestTwoSerialization()
        {
            SerializeTwoHelper<bool>(true);
            SerializeTwoHelper<ushort>(GenRadom.GenRandomUshort(32000));
            SerializeTwoHelper<int>(GenRadom.GenRandomInt(32000));
            SerializeTwoHelper<long>(GenRadom.GenRandomInt(64000));
            SerializeTwoHelper<ulong>((ulong)GenRadom.GenRandomInt(640000));
        }

        /// <summary>
        ///A test for Value
        ///</summary>
        public void SerializeMetadataTwoHelper<T>(T value)
        {
            GenericProperty<T> target = GenerateGenericProperty<T>();
            target.Value = value;
            byte[] data = MetaDataPayload.ToByteArray(target);

            GenericProperty<T> target2 = GenerateGenericProperty<T>();
            target2.Value = value;

            byte[] data2 = MetaDataPayload.ToByteArray(target2);

            MemoryStream ms = new MemoryStream();

            ms.Write(data, 0, data.Length);
            ms.Write(data2, 0, data2.Length);

            byte[] twoData = ms.ToArray();

            Tuple<IdType, int> retVal = MetaDataPayload.FromByteArray(twoData, 0);
            GenericProperty<T> actual = retVal.Item1 as GenericProperty<T>;
            retVal = MetaDataPayload.FromByteArray(twoData, retVal.Item2);
            GenericProperty<T> actual2 = retVal.Item1 as GenericProperty<T>;

            Assert.AreEqual(target, actual);
            Assert.AreEqual(target2, actual2);
        }

        [TestMethod]
        public void TestMetadataTwoSerialization()
        {
            SerializeMetadataTwoHelper<bool>(true);
            SerializeMetadataTwoHelper<ushort>(GenRadom.GenRandomUshort(32000));
            SerializeMetadataTwoHelper<int>(GenRadom.GenRandomInt(32000));
            SerializeMetadataTwoHelper<long>(GenRadom.GenRandomInt(64000));
            SerializeMetadataTwoHelper<ulong>((ulong)GenRadom.GenRandomInt(640000));
        }

		[TestMethod]
		public void GenericPropertyChangedEventTest ()
		{
            ushort id = GenRadom.GenRandomUshort(345);
            ushort type = GenRadom.GenRandomUshort(345);
            string name = GenRadom.GenRandomString(12);
            EventChangeCounter counter = new EventChangeCounter();
            GenericProperty<int> target = new GenericProperty<int>(id,type,name,0);
			target.PropertyChanged += (counter.EventChangedHandler);
			int expectedValue = 6;
			target.SetValue(expectedValue);
			Assert.AreEqual(expectedValue, target.GetValue());
			Assert.AreEqual(1,counter.Counter);
			MetadataChangedEventArgs eventArgs = counter.EventArgs as MetadataChangedEventArgs;
			Assert.IsNotNull(eventArgs);
			Assert.AreEqual(eventArgs.PropertyName, "Value");
			Assert.AreEqual(target.locationId, eventArgs.Location);
		}
    }
}
