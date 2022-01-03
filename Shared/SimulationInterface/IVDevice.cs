using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulationInterface
{ 
    public interface IMDModel
    {
        /// <summary>
        /// The id of the device or stack
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the device or stack
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Does it have a configuration?
        /// </summary>
        bool HasConfiguration { get; }

        /// <summary>
        /// Configuration data
        /// </summary>
        byte[] Configuration { get; set; }
    }

    public interface IVDevice : IMDModel
    {
        /// <summary>
        /// The type of device
        /// </summary>
        ulong DeviceType { get; }
    }

    public interface IDeviceStack : IMDModel
    {
        /// <summary>
        /// The type of stack that this represents
        /// </summary>
        ulong StackType { get; }
        /// <summary>
        /// This is the list of devices that make up this device stack
        /// </summary>
        List<IVDevice> Devices { get; }
    }
}
