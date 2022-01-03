//
// File Name: BaseContainerTest.cs
// ----------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;
using MDModeling;
using MDModeling.Utils;

// #if !NUNIT
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using Category = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
// #else
// #if !NUNIT
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using Category = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
// #else
using NUnit.Framework;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestContext = System.Object;
using TestProperty = NUnit.Framework.PropertyAttribute;
using TestClass = NUnit.Framework.TestFixtureAttribute;// #if !NUNIT
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using Category = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
// #else
using TestMethod = NUnit.Framework.TestAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
// #endif


namespace MDModelingUnitTest
{
    /// <summary>
    ///This is a test class for BaseContainerTest and is intended
    ///to contain all BaseContainerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BaseContainerTest
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
        ///A test for BaseContainer Constructor
        ///</summary>
        [TestMethod()]
        public void BaseContainerConstructorTest()
        {
            ushort id = (ushort)GenRadom.GenRandomInt(0, 64000);
            ushort typeId = (ushort)GenRadom.GenRandomInt(0, 64000);
            string name = GenRadom.GenRandomString(25);
            BaseContainer target = new BaseContainer(id, typeId, name);
            Assert.AreEqual(id, target.Id);
            Assert.AreEqual(typeId, target.TypeId);
            Assert.AreEqual(name, target.Name);
        }

        /// <summary>
        ///A test for AddChild
        ///</summary>
        [TestMethod()]
        public void AddChildTest()
        {
            BaseContainer target = GenRadom.GenIdTypeNameObj<BaseContainer>() as BaseContainer;
            BaseContainer newKid = GenRadom.GenIdTypeNameObj<BaseContainer>() as BaseContainer;
            bool expected = true;
            bool actual;
            actual = target.AddChild(newKid);
            Assert.AreEqual(expected, actual);
            BaseContainer actualKid = target.Child(newKid.locationId);
            Assert.AreEqual(newKid, actualKid);
        }

        /// <summary>
        ///A test for AddProperty
        ///</summary>
        [TestMethod()]
        public void AddPropertyTest()
        {
            BaseContainer target = GenRadom.GenIdTypeNameObj<BaseContainer>() as BaseContainer;
            PropertyBase expected = GenRadom.GenIdTypeNameObj<RangedProperty<int>>() as PropertyBase;
            PropertyBase actual;
            actual = target.Property(expected.locationId);
            Assert.IsNull(actual);
            Assert.IsTrue(target.AddProperty(expected));
        }

        /// <summary>
        ///A test for Child
        ///</summary>
        [TestMethod()]
        public void ChildTest()
        {
            BaseContainer target = GenRadom.GenIdTypeNameObj<BaseContainer>() as BaseContainer;
            BaseContainer expected = GenRadom.GenIdTypeNameObj<BaseContainer>() as BaseContainer;
            BaseContainer actual;
            actual = target.Child(expected.locationId);
            Assert.IsNull(actual);
            target.AddChild(expected);
            actual = target.Child(expected.locationId);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Property
        ///</summary>
        [TestMethod()]
        public void PropertyTest()
        {
            BaseContainer target = GenRadom.GenIdTypeNameObj<BaseContainer>() as BaseContainer;
            PropertyBase expected = GenRadom.GenIdTypeNameObj<RangedProperty<int>>() as PropertyBase;
            IdType actual;
            actual = target.Property(expected.locationId);
            Assert.IsNull(actual);
            target.AddProperty(expected);
            actual = target.Property(expected.locationId);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for RemoveChild
        ///</summary>
        [TestMethod()]
        public void RemoveChildTest()
        {
            BaseContainer target = GenRadom.GenIdTypeNameObj<BaseContainer>() as BaseContainer;
            BaseContainer expected = GenRadom.GenIdTypeNameObj<BaseContainer>() as BaseContainer;
            BaseContainer actual;
            actual = target.Child(expected.locationId);
            Assert.IsNull(actual);
            target.AddChild(expected);
            actual = target.Child(expected.locationId);
            Assert.AreEqual(expected, actual);
            target.RemoveChild(expected.locationId);
            actual = target.Child(expected.locationId);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for RemoveProperty
        ///</summary>
        [TestMethod()]
        public void RemovePropertyTest()
        {
            BaseContainer target = GenRadom.GenIdTypeNameObj<BaseContainer>() as BaseContainer;
            PropertyBase expected = GenRadom.GenIdTypeNameObj<RangedProperty<int>>() as PropertyBase;
            IdType actual;
            actual = target.Property(expected.locationId);
            Assert.IsNull(actual);
            target.AddProperty(expected);
            actual = target.Property(expected.locationId);
            Assert.AreEqual(expected, actual);
            target.RemoveProperty(expected.locationId);
            actual = target.Property(expected.locationId);
            Assert.IsNull(actual);
        }

        [TestMethod()]
        public void CompositingTest()
        {
            BaseContainer agc = new BaseContainer(1, (ushort)Constants.EObjects.AutomaticGainControl, "AGC");
            RangedProperty<float> gain = new RangedProperty<float>(1, (ushort)Constants.EProperties.Gain, "Gain", 0.0f);
            agc.AddProperty(gain);
            agc.AddProperty(new RangedProperty<float>(2, (ushort)Constants.EProperties.Threshold, "Threshold", 0.0f));
            agc.AddProperty(new RangedProperty<float>(3, (ushort)Constants.EProperties.Target, "Target", 0.0f));
            agc.AddProperty(new RangedProperty<UInt16>(4, (ushort)Constants.EProperties.Attack, "Attack", 0));
            agc.AddProperty(new RangedProperty<bool>(5, 25, "Enable", false));

            BaseContainer mic1 = new BaseContainer(1, (ushort)Constants.EObjects.Microphone, "Mic1");
            mic1.AddChild(agc);
            // Object inheritance gives us the objects at the same prop level as the containers so 
            // the will not be in a container.
            RangedProperty<float> Gain = new RangedProperty<float>(1, (ushort)Constants.EProperties.Gain, "Gain", 0.0f);
            mic1.AddProperty(Gain);
            mic1.AddProperty(new RangedProperty<UInt16>(4, (ushort)Constants.EProperties.ChannelID, "Channel Id", 0));
            mic1.AddProperty(new StringProperty(2, (ushort)Constants.EProperties.Name, "Channel Name"));
            mic1.AddProperty(new GenericProperty<bool>(3, (ushort)Constants.EProperties.Mute, "Mute", false));

            BaseContainer expected = mic1.Child(new LocationID(agc.Id, agc.TypeId, mic1.locationId));
            Assert.AreEqual(expected, agc);

            PropertyBase expectedGain = mic1.Property(gain.locationId);
            Assert.AreEqual(Gain, expectedGain);
        }

        [TestMethod()]
        public void BaseContainerSetPropertyTest()
        {
            BaseContainer agc = new BaseContainer(1, (ushort)Constants.EObjects.AutomaticGainControl, "AGC");
            RangedProperty<float> gain = new RangedProperty<float>(1, (ushort)Constants.EProperties.Gain, "Gain", 0.0f);
            agc.AddProperty(gain);
            agc.AddProperty(new RangedProperty<float>(2, (ushort)Constants.EProperties.Threshold, "Threshold", 0.0f));
            agc.AddProperty(new RangedProperty<float>(3, (ushort)Constants.EProperties.Target, "Target", 0.0f));
            agc.AddProperty(new RangedProperty<UInt16>(4, (ushort)Constants.EProperties.Attack, "Attack", 0));
            agc.AddProperty(new RangedProperty<bool>(5, 25, "Enable", false));
            ArrayOfProperty<byte> TxBundleStuff = new ArrayOfProperty<byte>(1, (ushort)Constants.EProperties.BundleNumber, "TxBundleNumber", null);
            agc.AddProperty(TxBundleStuff);

            BaseContainer mic1 = new BaseContainer(1, (ushort)Constants.EObjects.Microphone, "Mic1");
            mic1.AddChild(agc);
            // Object inheritance gives us the objects at the same prop level as the containers so 
            // the will not be in a container.
            RangedProperty<float> Gain = new RangedProperty<float>(1, (ushort)Constants.EProperties.Gain, "Gain", 0.0f);
            mic1.AddProperty(Gain);
            mic1.AddProperty(new RangedProperty<UInt16>(4, (ushort)Constants.EProperties.ChannelID, "Channel Id", 0));
            mic1.AddProperty(new StringProperty(2, (ushort)Constants.EProperties.Name, "Channel Name"));
            mic1.AddProperty(new GenericProperty<bool>(3, (ushort)Constants.EProperties.Mute, "Mute", false));

            Assert.IsTrue(mic1.SetProperty(gain.locationId, 25.0f));
            Assert.IsFalse(mic1.SetProperty(gain.locationId, 15));
            Assert.IsFalse(mic1.SetProperty(gain.locationId,"212"));

            Assert.IsTrue(mic1.SetProperty(TxBundleStuff.locationId, new byte[] { 1, 2, 3, 4, 5 }));
        }

        [TestMethod()]
        public void SerializeEmpytBaseContainer()
        {
            BaseContainer mic1 = new BaseContainer(1, (ushort)Constants.EObjects.Microphone, "Mic1");
            byte[] data = MetaDataPayload.ToByteArray(mic1);

            var ObjOffset = MetaDataPayload.FromByteArray(data);
            Assert.AreEqual(mic1, ObjOffset.Item1);
        }

        [TestMethod()]
        public void SerializeEmptyBCInEmptyBC()
        {
            BaseContainer mic1 = new BaseContainer(1, (ushort)Constants.EObjects.Microphone, "Mic1");
            BaseContainer agc = new BaseContainer(1, (ushort)Constants.EObjects.AutomaticGainControl, "AGC");
            mic1.AddChild(agc);
            byte[] data = MetaDataPayload.ToByteArray(mic1);

            var ObjOffset = MetaDataPayload.FromByteArray(data);
            Assert.AreEqual(mic1, ObjOffset.Item1);
        }

        [TestMethod()]
        public void BaseContainerToArrayTest()
        {
            BaseContainer agc = new BaseContainer(1, (ushort)Constants.EObjects.AutomaticGainControl, "AGC");
            RangedProperty<float> gain = new RangedProperty<float>(1, (ushort)Constants.EProperties.Gain, "Gain", 0.0f);
            agc.AddProperty(gain);
            agc.AddProperty(new RangedProperty<float>(2, (ushort)Constants.EProperties.Threshold, "Threshold", 0.0f));
            agc.AddProperty(new RangedProperty<float>(3, (ushort)Constants.EProperties.Target, "Target", 0.0f));
            agc.AddProperty(new RangedProperty<UInt16>(4, (ushort)Constants.EProperties.Attack, "Attack", 0));
            agc.AddProperty(new RangedProperty<bool>(5, 25, "Enable", false));
            ArrayOfProperty<byte> TxBundleStuff = new ArrayOfProperty<byte>(1, (ushort)Constants.EProperties.BundleNumber, "TxBundleNumber", null);
            agc.AddProperty(TxBundleStuff);

            BaseContainer mic1 = new BaseContainer(1, (ushort)Constants.EObjects.Microphone, "Mic1");
            mic1.AddChild(agc);
            // Object inheritance gives us the objects at the same prop level as the containers so 
            // the will not be in a container.
            RangedProperty<float> Gain = new RangedProperty<float>(1, (ushort)Constants.EProperties.Gain, "Gain", 0.0f);
            mic1.AddProperty(Gain);
            mic1.AddProperty(new RangedProperty<UInt16>(4, (ushort)Constants.EProperties.ChannelID, "Channel Id", 0));
            mic1.AddProperty(new StringProperty(2, (ushort)Constants.EProperties.Name, "Channel Name", "Channel 1"));
            mic1.AddProperty(new GenericProperty<bool>(3, (ushort)Constants.EProperties.Mute, "Mute", false));
            mic1.AddProperty(new GenericProperty<int>(5, 12, "Spencer", 15));

            byte[] data = MetaDataPayload.ToByteArray(mic1);

            string fileName = System.IO.Path.GetTempFileName();
            if(File.Exists(fileName)) File.Delete(fileName);
            FileStream file = new FileStream(fileName, FileMode.CreateNew);
            file.Write(data, 0, data.Length);
            file.Close();


            var ObjOffset = MetaDataPayload.FromByteArray(data);
            Assert.AreEqual(mic1, ObjOffset.Item1);
        }


		public int TestCountWalkPropertyTree (BaseContainer bc)
		{
			int counter = 0;
			foreach (PropertyBase pb in bc.WalkPropertyTree()) {
				if (pb != null) {
					counter += 1;
				}
			}
			return counter;
		}

		public int TestCountWalkTree (BaseContainer bc)
		{
			int counter = 0;
			foreach (IdType it in bc.WalkTree()) {
				if (it != null) {
					Console.WriteLine(it.ToString());
					counter += 1;
				}
			}
			return counter;
		}

		[TestMethod()]
		public void WalkTreeEmptyContainerTest ()
		{
			BaseContainer bc = new BaseContainer();
			Assert.AreEqual(0, TestCountWalkPropertyTree(bc));
			Assert.AreEqual(1, TestCountWalkTree(bc));
		}
		[TestMethod()]
		public void WalkTreeNonContainerTest()
		{
            BaseContainer agc = new BaseContainer(1, (ushort)Constants.EObjects.AutomaticGainControl, "AGC");
            RangedProperty<float> gain = new RangedProperty<float>(1, (ushort)Constants.EProperties.Gain, "Gain", 0.0f);
            agc.AddProperty(gain);
            agc.AddProperty(new RangedProperty<float>(2, (ushort)Constants.EProperties.Threshold, "Threshold", 0.0f));
            agc.AddProperty(new RangedProperty<float>(3, (ushort)Constants.EProperties.Target, "Target", 0.0f));
            agc.AddProperty(new RangedProperty<UInt16>(4, (ushort)Constants.EProperties.Attack, "Attack", 0));
            agc.AddProperty(new RangedProperty<bool>(5, 25, "Enable", false));
            ArrayOfProperty<byte> TxBundleStuff = new ArrayOfProperty<byte>(1, (ushort)Constants.EProperties.BundleNumber, "TxBundleNumber", null);
            agc.AddProperty(TxBundleStuff);

            BaseContainer mic1 = new BaseContainer(1, (ushort)Constants.EObjects.Microphone, "Mic1");
            mic1.AddChild(agc);
            // Object inheritance gives us the objects at the same prop level as the containers so 
            // the will not be in a container.
            RangedProperty<float> Gain = new RangedProperty<float>(1, (ushort)Constants.EProperties.Gain, "Gain", 0.0f);
            mic1.AddProperty(Gain);
            mic1.AddProperty(new RangedProperty<UInt16>(4, (ushort)Constants.EProperties.ChannelID, "Channel Id", 0));
            mic1.AddProperty(new StringProperty(2, (ushort)Constants.EProperties.Name, "Channel Name", "Channel 1"));
            mic1.AddProperty(new GenericProperty<bool>(3, (ushort)Constants.EProperties.Mute, "Mute", false));
            mic1.AddProperty(new GenericProperty<int>(5, 12, "Spencer", 15));
			Assert.AreEqual(11, TestCountWalkPropertyTree(mic1));
			Assert.AreEqual(13, TestCountWalkTree(mic1));
		}

		[TestMethod()]
		public void ModelChanged ()
		{
			StringProperty strProp = new StringProperty (1, 1, "fred");
			EventChangeCounter counter = new EventChangeCounter ();
			strProp.PropertyChanged += counter.EventChangedHandler;
			Assert.AreEqual(0,counter.Counter);
			strProp.Value = "John";
			Assert.AreEqual (1, counter.Counter);
		}
    }
}