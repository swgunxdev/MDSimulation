//
// File Name: SimulationModel
// ----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using Slf;
using TimpInterfaces;

namespace ClientEngine
{
    /// <summary>
    /// SimulationModel
    /// </summary>
    public class SimulationModel : IDSStorageModel, ISimDiscoveryData
    {
        #region Private Variables
        private string _modelName;
        #endregion // Private Variables

        #region Public Events
        public event PropertyChangedEventHandler PropertyChanged;
//        public event ModelChanged OnModelChanged;
        private ILogger _modelLogger = null;
        private ulong _modelId;
        #endregion // Public Events

        #region Constructor / Dispose
        /// <summary>
        /// The default constructor
        /// </summary>
        public SimulationModel()
        {
            _modelLogger = LoggerService.GetLogger();
        }
        #endregion // Constructor / Dispose

        #region Properties
        public ulong ModelId
        {
            get { throw new NotImplementedException(); }
            protected set { _modelId = value;}
        }

        public string ModelName
        {
            get { return _modelName; }
            protected set { _modelName = value; }
        }

        //public ISendReceiveCmds CommandInterface
        //{
        //    get { throw new NotImplementedException(); }
        //}

        public int LoadModel(string modelFileName)
        {
            // load the model
            _modelLogger.Info("Loading model from file {0}", modelFileName);
            RaisePropertyChanged("Model");
            return 0;
        }

        public int UnloadModel()
        {
            throw new NotImplementedException();
        }

        public int Model { get { return 0; } }



		public DiscoveryDataDelegate GetDiscoveryData
        {
            get
            {
                return null;// EncodeDiscoveryResponse;
            }
        }

        #endregion // Properties

        #region Public Methods
        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion Debugging Aides

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            this.OnPropertyChanged(propertyName);
        }

        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        #endregion INotifyPropertyChanged Members

        //private byte[] EncodeDiscoveryResponse(uint authPort)
        //{
        //    CDiscoveryResponse response = new CDiscoveryResponse();
        //    response.VTable.AuthenticationPort = (ushort)authPort;
        //    response.VTable.VTableDevice = GenerateSingleTimp8x8VTable();
        //    response.VTable.Count = (byte)response.VTable.VTableDevice.Count();

        //    Constants.SPacket packet = new Constants.SPacket();
        //    response.GetPacketData(out packet);
        //    return ClearOneProtocol.Constants.PacketToByteArray(packet);
        //}

        //private CVTableDevice [] GenerateSingleTimp8x8VTable()
        //{
        //    CVTableDevice device = new CVTableDevice();
        //    device.DeviceType = (byte)ClearOneProtocol.Constants.EDevices.Timpanogos8x8;
        //    device.GLinkOrder = 0;
        //    device.IPAddress = (uint)IPAddress.Parse("127.0.0.1").Address;
        //    device.MacAddress = 0xDEADBEEF;
        //    device.Name = "SimulatedTimp8x8";
        //    CVTableDevice [] devices = new CVTableDevice[1];
        //    devices[0] = device;
        //    return devices;
        //}

        private uint DecodeDiscoveryResponse(byte [] data, uint offset)
        {
            uint index = offset;
            //Constants.SPacket packet = Constants.ByteArrayToPacket(data, ref index);
            //CDiscoveryResponse response = new CDiscoveryResponse();
            //response.SetPacketData(packet);
            return index;
        }

        #endregion // Public Methods

        #region Private Methods
        #endregion // Private Methods

        #region enums
        #endregion





    }
}
