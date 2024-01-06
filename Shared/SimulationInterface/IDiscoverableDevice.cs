using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulationInterface
{
    public interface IDiscoverableDevice
    {
        uint DeviceType { get; }
        string Name { get; }
        string ConnectString { get; }
        object Data { get;  }
        int Id { get; }
    }
}
