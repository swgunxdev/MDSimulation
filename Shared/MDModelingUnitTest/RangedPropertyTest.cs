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
    ///This is a test class for RangedPropertyTest and is intended
    ///to contain all RangedPropertyTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RangedPropertyTest
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
        ///A test for RangedProperty`1 Constructor
        ///</summary>
        public void RangedPropertyConstructorTestHelper<T>()
        {
            ushort id = (ushort)GenRadom.GenRandomInt(254);
            ushort typeId = (ushort)GenRadom.GenRandomInt(254);
            string name = GenRadom.GenRandomString(128);
            T defaultValue = default(T);
            RangedProperty<T> target = new RangedProperty<T>(id, typeId, name, defaultValue);
            Assert.AreEqual(id, target.Id);
            Assert.AreEqual(typeId, target.TypeId);
            Assert.AreEqual(name, target.Name);
            Assert.AreEqual(defaultValue, target.Value);
        }

        [TestMethod()]
        public void RangedPropertyConstructorTest()
        {
            RangedPropertyConstructorTestHelper<int>();
            RangedPropertyConstructorTestHelper<float>();
            RangedPropertyConstructorTestHelper<double>();
            RangedPropertyConstructorTestHelper<Int64>();
            RangedPropertyConstructorTestHelper<byte>();
        }

        /// <summary>
        ///A test for RangedProperty`1 Constructor
        ///</summary>
        public void RangedPropertyConstructorTest1Helper<T>(T dv, T mn, T mx, T interval)
        {
            T defaultValue = dv;
            T min = mn;
            T max = mx;
            T frequency = interval;
            RangedProperty<T> target = GenRadom.GenIdTypeNameObj<RangedProperty<T>>() as RangedProperty<T>;
			target.DefaultValue = dv;
			target.Minimum = mn;
			target.Maximuim = mx;
			target.Frequency = interval;
            Assert.IsTrue(target.IsValid());
        }

        [TestMethod()]
        public void RangedPropertyConstructorTest1()
        {
            RangedPropertyConstructorTest1Helper<int>(5, 0, 15, 1);
        }

        /// <summary>
        ///A test for IsValid
        ///</summary>
        public void IsValidTestHelper<T>(bool expected, T value, T min, T max, T freq, bool expected2)
        {
            ushort id = (ushort)GenRadom.GenRandomInt(254);
            ushort typeId = (ushort)GenRadom.GenRandomInt(254);
            string name = GenRadom.GenRandomString(128);
            T defaultValue = default(T);
            RangedProperty<T> target = new RangedProperty<T>(id, typeId, name, defaultValue, min, max, freq);
            bool actual;
            actual = target.IsValid();
            Assert.AreEqual(expected, actual);
            target.Value = value;
            Assert.AreEqual(expected2, target.IsValid());
        }

        [TestMethod()]
        public void IsValidTest()
        {
            IsValidTestHelper<Int32>(true, 121, 0, 125, 2, false);
            IsValidTestHelper<Int32>(true, 122, 0, 124, 2, true);
        }

        /// <summary>
        ///A test for RangedProperty`1 Constructor
        ///</summary>
        public void ClrPropertyConstructorTestHelper<T>(ushort id, ushort typeId, string name, T defaultValue)
        {
            RangedProperty<T> target = new RangedProperty<T>(id, typeId, name, defaultValue);
            Assert.AreEqual(id, target.Id);
            Assert.AreEqual(typeId, target.TypeId);
            Assert.AreEqual(defaultValue, target.Value);
        }

        [TestMethod()]
        public void ClrPropertyConstructorTest()
        {
            ClrPropertyConstructorTestHelper<Int16>(1, 2, "Fred", 24);
            ClrPropertyConstructorTestHelper<float>(1, 2, "Fred", 24.5f);
        }

        /// <summary>
        ///A test for RangedProperty`1 Constructor
        ///</summary>
        public void ClrPropertyConstructorTest1Helper<T>(ushort id, ushort typeId, string name, T defaultValue, T min, T max, T freq, bool expected)
        {
            RangedProperty<T> target = new RangedProperty<T>(id, typeId, name, defaultValue, min, max, freq);
            Assert.AreEqual(id, target.Id);
            Assert.AreEqual(typeId, target.TypeId);
            Assert.AreEqual(defaultValue, target.Value);
            Assert.AreEqual(expected, target.IsValid());
        }

        [TestMethod()]
        public void ClrPropertyConstructorTest1()
        {
            ClrPropertyConstructorTest1Helper<Int16>(1, 2, "Fred", 24, 1, 25, 2, true);
            ClrPropertyConstructorTest1Helper<float>(1, 2, "Fred", 24.0f, 1.0f, 25.0f, 2.0f, true);

            ClrPropertyConstructorTest1Helper<Int16>(1, 2, "Fred", 24, 1, 25, 2, true); // test within range,good increment
            ClrPropertyConstructorTest1Helper<Int16>(1, 2, "Fred", 24, 1, 25, 7, false); // test within range, bad increment
            ClrPropertyConstructorTest1Helper<Int16>(1, 2, "Fred", 24, 1, 20, 2, false); // test out of range max, good increment
            ClrPropertyConstructorTest1Helper<Int16>(1, 2, "Fred", 2, 10, 25, 2, false); // test out of range min, good increment
            ClrPropertyConstructorTest1Helper<Int16>(1, 2, "Fred", 2, 25, 25, 3, false); // test out of range min and max, bad increment
            ClrPropertyConstructorTest1Helper<float>(1, 2, "Fred", 24.0F, 1.0F, 25.0F, 2.0F, true); // test within range,good increment
            ClrPropertyConstructorTest1Helper<float>(1, 2, "Fred", 24.0F, 1.0F, 25.0F, 7.0F, false); // test within range, bad increment
            ClrPropertyConstructorTest1Helper<float>(1, 2, "Fred", 24.0F, 1.0F, 20.0F, 2.0F, false); // test out of range max, good increment
            ClrPropertyConstructorTest1Helper<float>(1, 2, "Fred", 2.0F, 10.0F, 25.0F, 2.0F, false); // test out of range min, good increment
            ClrPropertyConstructorTest1Helper<float>(1, 2, "Fred", 2.0F, 25.0F, 25.0F, 3.0F, false); // test out of range min and max, bad increment
        }

        public RangedProperty<T> GenerateRangedProperty<T>(T defaultValue, T min, T max, T freq)
        {
            ushort id = GenRadom.GenRandomUshort(32000);
            ushort type = GenRadom.GenRandomUshort(32000);
            string name = GenRadom.GenRandomString(128);
            return new RangedProperty<T>(id, type, name, defaultValue, min, max, freq);
        }

        [TestMethod]
        public void TestRangedPropertySerialization()
        {
            SerializeTestHelper<Int16>(24, 1, 25, 2, true);
            SerializeTestHelper<float>(24.0f, 1.0f, 25.0f, 2.0f, true);

            SerializeTestHelper<Int16>(24, 1, 25, 2, true); // test within range,good increment
            SerializeTestHelper<Int16>(24, 1, 25, 7, false); // test within range, bad increment
            SerializeTestHelper<Int16>(24, 1, 20, 2, false); // test out of range max, good increment
            SerializeTestHelper<Int16>(2, 10, 25, 2, false); // test out of range min, good increment
            SerializeTestHelper<Int16>(2, 25, 25, 3, false); // test out of range min and max, bad increment
            SerializeTestHelper<float>(24.0F, 1.0F, 25.0F, 2.0F, true); // test within range,good increment
            SerializeTestHelper<float>(24.0F, 1.0F, 25.0F, 7.0F, false); // test within range, bad increment
            SerializeTestHelper<float>(24.0F, 1.0F, 20.0F, 2.0F, false); // test out of range max, good increment
            SerializeTestHelper<float>(2.0F, 10.0F, 25.0F, 2.0F, false); // test out of range min, good increment
            SerializeTestHelper<float>(2.0F, 25.0F, 25.0F, 3.0F, false); // test out of range min and max, bad increment
            SerializeTestHelper<byte>(24, 1, 25, 2, true); // test within range,good increment
            SerializeTestHelper<byte>(24, 1, 25, 7, false); // test within range, bad increment
            SerializeTestHelper<byte>(24, 1, 20, 2, false); // test out of range max, good increment
            SerializeTestHelper<byte>(2, 10, 25, 2, false); // test out of range min, good increment
            SerializeTestHelper<byte>(2, 25, 25, 3, false); // test out of range min and max, bad increment
            SerializeTestHelper<SByte>(24, 1, 25, 2, true); // test within range,good increment
            SerializeTestHelper<SByte>(24, 1, 25, 7, false); // test within range, bad increment
            SerializeTestHelper<SByte>(24, 1, 20, 2, false); // test out of range max, good increment
            SerializeTestHelper<SByte>(2, 10, 25, 2, false); // test out of range min, good increment
            SerializeTestHelper<SByte>(2, 25, 25, 3, false); // test out of range min and max, bad increment
            SerializeTestHelper<double>(24.0F, 1.0F, 25.0F, 2.0F, true); // test within range,good increment
            SerializeTestHelper<double>(24.0F, 1.0F, 25.0F, 7.0F, false); // test within range, bad increment
            SerializeTestHelper<double>(24.0F, 1.0F, 20.0F, 2.0F, false); // test out of range max, good increment
            SerializeTestHelper<double>(2.0F, 10.0F, 25.0F, 2.0F, false); // test out of range min, good increment
            SerializeTestHelper<double>(2.0F, 25.0F, 25.0F, 3.0F, false); // test out of range min and max, bad increment
        }

        /// <summary>
        ///A test for Value
        ///</summary>
        public void SerializeTestHelper<T>(T defaultValue, T min, T max, T freq, bool expectedIsValid)
        {
            var target = GenerateRangedProperty<T>(defaultValue, min, max, freq);
            byte[] data = MetaDataPayload.ToByteArray(target);

            var actual = MetaDataPayload.FromByteArray(data);

            Assert.AreEqual(target, actual.Item1);
            RangedProperty<T> actualRanged = actual.Item1 as RangedProperty<T>;
            Assert.IsNotNull(actualRanged);
            Assert.AreEqual(expectedIsValid, actualRanged.IsValid());
        }

        public void TestRangedPropertySerializationHelper2<T>(T defaultValue, T min, T max, T freq, bool expectedIsValid)
        {
            var target = GenerateRangedProperty<T>(defaultValue, min, max, freq);
            byte[] data = MetaDataPayload.ToByteArray(target);

            var actual = MetaDataPayload.FromByteArray(data);
            Assert.AreEqual(target, actual.Item1);
            RangedProperty<T> actualRanged = actual.Item1 as RangedProperty<T>;
            Assert.IsNotNull(actualRanged);
            Assert.AreEqual(expectedIsValid, actualRanged.IsValid());
        }

        [TestMethod]
        public void TestRangedPropertySerialization2()
        {
            TestRangedPropertySerializationHelper2<Int16>(24, 1, 25, 2, true);
            TestRangedPropertySerializationHelper2<float>(24.0f, 1.0f, 25.0f, 2.0f, true);

            TestRangedPropertySerializationHelper2<Int16>(24, 1, 25, 2, true); // test within range,good increment
            TestRangedPropertySerializationHelper2<Int16>(24, 1, 25, 7, false); // test within range, bad increment
            TestRangedPropertySerializationHelper2<Int16>(24, 1, 20, 2, false); // test out of range max, good increment
            TestRangedPropertySerializationHelper2<Int16>(2, 10, 25, 2, false); // test out of range min, good increment
            TestRangedPropertySerializationHelper2<Int16>(2, 25, 25, 3, false); // test out of range min and max, bad increment
            TestRangedPropertySerializationHelper2<byte>(24, 1, 25, 2, true); // test within range,good increment
            TestRangedPropertySerializationHelper2<byte>(24, 1, 25, 7, false); // test within range, bad increment
            TestRangedPropertySerializationHelper2<byte>(24, 1, 20, 2, false); // test out of range max, good increment
            TestRangedPropertySerializationHelper2<byte>(2, 10, 25, 2, false); // test out of range min, good increment
            TestRangedPropertySerializationHelper2<byte>(2, 25, 25, 3, false); // test out of range min and max, bad increment
            TestRangedPropertySerializationHelper2<float>(24.0F, 1.0F, 25.0F, 2.0F, true); // test within range,good increment
            TestRangedPropertySerializationHelper2<float>(24.0F, 1.0F, 25.0F, 7.0F, false); // test within range, bad increment
            TestRangedPropertySerializationHelper2<float>(24.0F, 1.0F, 20.0F, 2.0F, false); // test out of range max, good increment
            TestRangedPropertySerializationHelper2<float>(2.0F, 10.0F, 25.0F, 2.0F, false); // test out of range min, good increment
            TestRangedPropertySerializationHelper2<float>(2.0F, 25.0F, 25.0F, 3.0F, false); // test out of range min and max, bad increment
        }

    }
}