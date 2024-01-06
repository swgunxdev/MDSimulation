using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SimulationInterface
{

    public delegate int GetOperatingPortDelegate();

    public delegate byte[] DiscoveryDataDelegate(uint authPort);

    public interface IDSStorageModel : INotifyPropertyChanged
    {
        ulong ModelId { get; }

        string ModelName { get; }

        //ISendReceiveCmds CommandInterface { get; }

        int LoadModel(string modelFileName);

        int UnloadModel();
    }

    public interface ISimDiscoveryData
    {
        DiscoveryDataDelegate GetDiscoveryData { get; }
    }
}
