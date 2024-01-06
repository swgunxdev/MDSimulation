using System;
using MDModeling;
using MDModeling.Utils;


#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Category = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
using System.Linq;
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
    ///This is a test class for ArrayOfTest and is intended
    ///to contain all ArrayOfTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ArrayOfTest
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
        ///A test for SetProperty
        ///</summary>
        public void ConstructorTestHelper<T>(T[] value, int maxSize)
            where T : struct
        {
            ushort id = (ushort)GenRadom.GenRandomInt(254);
            ushort typeId = (ushort)GenRadom.GenRandomInt(254);
            string name = GenRadom.GenRandomString(128);
            ArrayOfProperty<T> target = null;
            if(maxSize > 0)
                target = new ArrayOfProperty<T>(id, typeId, name, value, maxSize);
            else
                target = new ArrayOfProperty<T>(id, typeId, name, value);

            // Test the id, type, and name
            IdTypeNameTest(id, typeId, name, target);

            // Make sure that the values are equal
            if (maxSize > 0)
            {
                for (int i = 0; i < maxSize; i++)
                {
                    Assert.AreEqual(value[i], target[i]);
                }
            }
            else
            {
                Assert.IsTrue(Enumerable.SequenceEqual(value, target.Value));
            }
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            ConstructorTestHelper<int>(new int[] { 12, 34, 56 }, 0);
            ConstructorTestHelper<float>(new float[] { 12.0f, 34.0f, 56.0f },3);
            ConstructorTestHelper<double>(new double[] { 12E+3, 34E-23, 56E-2, }, 3);
            ConstructorTestHelper<byte>(new byte[] { 12, 34, 56, 45, 12, 255 }, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorMaxSizeTest()
        {
            ConstructorTestHelper<int>(new int[] { 12, 34, 56 }, 2);
            ConstructorTestHelper<float>(new float[] { 12.0f, 34.0f, 56.0f }, 3);
            ConstructorTestHelper<double>(new double[] { 12E+3, 34E-23, 56E-2, }, 1);
            ConstructorTestHelper<byte>(new byte[] { 12, 34, 56, 45, 12, 255 }, 0);
        }

        /// <summary>
        ///A test for SetProperty
        ///</summary>
        public void SetPropertyTestHelper<T>(T[] value) 
            where T: struct
        {
            ushort id = (ushort)GenRadom.GenRandomInt(254);
            ushort typeId = (ushort)GenRadom.GenRandomInt(254);
            string name = GenRadom.GenRandomString(128);
            ArrayOfProperty<T> target = new ArrayOfProperty<T>(id, typeId, name, value);
            IdTypeNameTest(id, typeId, name, target);
            target.Value = value;
            Assert.IsTrue(Enumerable.SequenceEqual<T>(value, target.Value));
        }

        [TestMethod()]
        public void SetPropertyTest()
        {
            SetPropertyTestHelper<int>(new int[] { 12, 34, 56 });
            SetPropertyTestHelper<float>(new float[] { 12.0f, 34.0f, 56.0f });
            SetPropertyTestHelper<double>(new double[] { 12E+3, 34E-23, 56E-2, });
            SetPropertyTestHelper<byte>(new byte[] { 12, 34, 56, 45, 12, 255 });
        }

        /// <summary>
        ///A test for Item
        ///</summary>
        public void ItemTestHelper<T>(T[] defaultValue, int index)
            where T : struct
        {
            ushort id = (ushort)GenRadom.GenRandomInt(254);
            ushort typeId = (ushort)GenRadom.GenRandomInt(254);
            string name = GenRadom.GenRandomString(128);
            ArrayOfProperty<T> target = new ArrayOfProperty<T>(id, typeId, name, defaultValue);
            IdTypeNameTest(id, typeId, name, target);
            Assert.AreEqual(defaultValue[index], target[index]);
        }

        public void IdTypeNameTest(ushort id, ushort type, string name, IdType target)
        {
            Assert.AreEqual(id, target.Id);
            Assert.AreEqual(type, target.TypeId);
            Assert.AreEqual(name, target.Name);
        }

        [TestMethod()]
        public void ItemTest()
        {
            ItemTestHelper<Byte>(new Byte[] { 1, 2, 3, 4, 5 }, 4);
            ItemTestHelper<float>(new float[] { 1.0f, 2.3f, 33.2f, 0.5f, 5.0f }, 3);
            //ItemTestHelper<string>(new string[] { "abc", "def", "ghi", "jkl", "mno" }, 2);
        }

        public void EncodeTestHelper<T>(T[] defaultValue)
            where T : struct
        {
            ushort id = (ushort)GenRadom.GenRandomInt(254);
            ushort typeId = (ushort)GenRadom.GenRandomInt(254);
            string name = GenRadom.GenRandomString(128);
            ArrayOfProperty<T> target = new ArrayOfProperty<T>(id, typeId, name, defaultValue);
            byte[] encoded = target.ToByteArray();
            ArrayOfProperty<T> nextTarget = new ArrayOfProperty<T>(id, typeId, name, null);
            nextTarget.FromByteArray(encoded, 0);
            for (int i = 0; i < defaultValue.Length; i++)
            {
                Assert.AreEqual(defaultValue[i], nextTarget[i]);
            }
        }

        
        [TestMethod()]
        public void EncodeTest()
        {
            EncodeTestHelper<byte>(new byte[] { 1, 2, 3, 4, 5 });
            EncodeTestHelper<float>(new float[] { 1.0f, 2.3f, 33.2f, 0.5f, 5.0f });
            //EncodeTestHelper<string>(new string[] { "abc123","321cba","petuniafrog","petunia",});
        }

        public void GetValueTestHelper<T>(T[] defaultValue)
            where T : struct
        {
            ushort id = (ushort)GenRadom.GenRandomInt(254);
            ushort typeId = (ushort)GenRadom.GenRandomInt(254);
            string name = GenRadom.GenRandomString(128);
            ArrayOfProperty<T> target = new ArrayOfProperty<T>(id, typeId, name, defaultValue);
            T[] testValue = target.GetValue() as T [];
            for (int i = 0; i < defaultValue.Length; i++)
            {
                Assert.AreEqual(defaultValue[i], testValue[i]);
            }
        }

        [TestMethod()]
        public void GetValueTest()
        {
            GetValueTestHelper<byte>(new byte[] { 1, 2, 3, 4, 5 });
            GetValueTestHelper<float>(new float[] { 1.0f, 2.3f, 33.2f, 0.5f, 5.0f });
        }
    }
}