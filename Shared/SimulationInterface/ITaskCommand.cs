using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulationInterface
{
    public interface IObjAddress
    { }

    public interface ITimpCommand
    { }

    public interface ITaskCommand
    {
        long Execute();
    }

    // TaskMgr -- collects the commands and asks for them to be
    // executed.
    public interface ITaskCommandClient
    {
        long AddCommand(ITaskCommand cmd);
    }

    /// <summary>
    /// Simulation Engine
    /// </summary>
    public interface ITaskCommandReciever
    {
        // connection commands
        long Connect(int id, string userid, string password);
        long Disconnect(int id);
        long SendCommand(int id,IClrOneCommand cmd);
        long ReadModelConfiguration(int id, string fileName);
        long SendFirmwareUpdate(int id, string fileName);
        long DownloadLogFileTo(int id, string fileName);
        long ClearLogFiles(int id);
        long PrintMessage(int id,string _msg);

        // Model commands
        long SetProperty(IObjAddress objAddress, uint propertyId, object value);
        long CompareValue(IObjAddress objAddress, uint propertyId, object value);
        long OpenModel(string filename);
        long CloseModel();
        long NewModel(string modelName);
    }

   // Client Simulator that puts the commands into to be run.
    public interface ITaskInvoker
    {
        int AddCommand(ITaskCommand cmd);
    }
}
