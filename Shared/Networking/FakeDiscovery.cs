//
// File Name: FakeDiscovery
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SimulationInterface;
using System.Threading;


namespace Networking
{
    /// <summary>
    /// FakeDiscovery
    /// </summary>
    public class FakeDiscovery
    {
        #region Private Variables
        //Thread _discoveryThread = null;
        bool _StopThread = false;
        bool _foundAlready = false;
        object _lockObj = new object();
        IDiscoverableDevice _info = null;
        #endregion // Private Variables

        #region Public Events
        /// <summary>
        /// This delegate is used when a new device has arrived
        /// </summary>
        /// <param name="deviceType">This is the device type of the found device</param>
        /// <param name="connectionString">this is the IP address or other connection string of the new device</param>
        /// <param name="name">this is the name of the device or system</param>
        /// <param name="data">this is data specific to the device type that can be used by the subscribing object</param>
        public delegate void DeviceArrivalDelegate(string connectionString, string name, object data);
        /// <summary>
        /// This is the event with which to subscribe for device arrivals
        /// </summary>
        public event DeviceArrivalDelegate DeviceArrival;
        /// <summary>
        /// This is the delegate used to notify about device removals
        /// </summary>
        /// <param name="connectionString">this is the IP address or other connection string of the removed device</param>
        public delegate void DeviceRemovalDelegate(string connectionString);
        /// <summary>
        /// This is the event with which to subscribe for device removals
        /// </summary>
        public event DeviceRemovalDelegate DeviceRemoval;
        //private ushort _authPort;
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        #endregion // Constructor / Dispose

        #region Properties
        #endregion // Properties

        #region Public Methods
        public static void DeleteFakeDiscoveryFile()
        {
            string fakeFileName = GenFakeDiscoveryName();
            if (File.Exists(fakeFileName)) File.Delete(fakeFileName);
        }

        //public static void CreateFakeDiscoveryFile(IHaveServerPorts srvrPorts)
        //{
        //    DeleteFakeDiscoveryFile();
        //    ISerializeByteArray arraySerilizer = srvrPorts as ISerializeByteArray;
        //    if (arraySerilizer != null)
        //    {
        //        FileStream fs = File.Open(GenFakeDiscoveryName(), FileMode.Create);
        //        byte[] data = arraySerilizer.ToByteArray();
        //        fs.Write(data, 0, data.Length);
        //        fs.Close();
        //    }
        //}

        public static void CreateFakeDiscoveryFile(byte [] discoveryData)
        {
            DeleteFakeDiscoveryFile();
            try
            {
                FileStream fs = File.Open(GenFakeDiscoveryName(), FileMode.Create);
                fs.Write(discoveryData, 0, discoveryData.Length);
                fs.Close();

            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public static string GenFakeDiscoveryName()
        {
            string tmpPath = Path.GetTempPath();
            return Path.Combine(tmpPath, "FAKEDISCOVERY.bin");
        }

        public static IDiscoverableDevice ReadFakeDiscovery()
        {
            IDiscoverableDevice device = null;
            string fakeDiscoveryFilename = GenFakeDiscoveryName();
            if (File.Exists(fakeDiscoveryFilename))
            {
                byte [] data = File.ReadAllBytes(GenFakeDiscoveryName());
                device = new DiscoverableDevice(data, 0);
            }
            return device;
        }

        public static bool Exists { get { return File.Exists(GenFakeDiscoveryName()); } }

        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion

        public void StartLocating()
        {
            ThreadStart ts = new ThreadStart(DiscoveryThread);
            Thread _discoveryThread = new Thread(ts);
            _discoveryThread.Start();
        }

        public void StopLocating()
        {
            _StopThread = true;
        }

        public void DiscoveryThread()
        {
            int timeToWaitInMs = 10000;

            while (!_StopThread)
            {
                if (Exists && !_foundAlready)
                {
                    lock (_lockObj)
                    {
                        _info = FakeDiscovery.ReadFakeDiscovery();
                        if (_info != null)
                        {
                            _foundAlready = true;
                            if (DeviceArrival != null)
                            {
                                DeviceArrival(_info.ConnectString, _info.Name, _info.Data);
                            }
                        }
                    }
                }
                else
                {
                    if (_foundAlready)
                    {
                        lock (_lockObj)
                        {
                            _foundAlready = false;
                            if (DeviceRemoval != null)
                            {
                                DeviceRemoval(_info.ConnectString);
                                _info = null;
                            }
                        }
                    }
                }

                // sleep for a bit
                int waitTime = 0;
                while (waitTime <= timeToWaitInMs)
                {
                    Thread.Sleep(100);
                    waitTime += 100;
                    if (_StopThread) break;
                }
            }
        }
    }
}
