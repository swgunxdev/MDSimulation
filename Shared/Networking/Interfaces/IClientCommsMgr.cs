using System.Collections.Generic;
using SimulationInterface;

namespace Networking.Interfaces
{
    public delegate void VDeviceArrival(ulong id);
    public delegate void VDeviceLeave(ulong id);

    public interface IDeviceDiscoveryClient
    {
        void StartDiscovery();

        void StopDiscovery();

        event VDeviceArrival OnVDeviceArrival;

        event VDeviceLeave OnVDeviceLeaving;

        Dictionary<int, IMDModel> DeviceList { get; }
    }

    public interface IConnectionClient
    {
        bool Connect(int id, string username, string password);

        bool Disconnect(int connectionId);

        //bool SubscribeToCommands(ulong connectionId, ISendReceiveCmds cmdInterface);

        //bool SubscribeToData(ulong connectionid, ISendRecieveData dataInterface);
    }

    public interface IClientCommsMgr : IConnectionClient, IDeviceDiscoveryClient
    {

        void StopAll();
    }
}
