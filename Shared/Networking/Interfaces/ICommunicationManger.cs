//
// File Namespace: ICommunicationManger.cs
// ----------------------------------------------------------------------
using SimulationInterface;

namespace Networking.Interfaces
{
    public delegate long ReceiveData(byte[] data, ulong connectionId);
    public delegate long SendData(byte [] data, ulong connectionid);

    public delegate uint ReceiveCmd(IClrOneCommand cmd);
    public delegate uint SendCmd(IClrOneCommand cmd);

    /// <summary>
    /// The object that implements this interface should be the object
    /// that is more then likely to 
    /// </summary>
    public interface ISendRecieveData
    {
        int SendData(byte[] data, ulong connectionId);

        event ReceiveData OnReceiveData;
    }

    public interface ISendReceiveCmds
    {
        int SendCmd(IClrOneCommand cmd);

        event ReceiveCmd OnReceiveCmd;
    }
}
