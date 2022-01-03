using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SimulationInterface.Implementations;

namespace SimulationInterface
{

    public interface IThorSimulatorStateMethods : IStateContext
    {
        void Start();

        void Init();

        void StartServers();

        void Run();

        void Stop();

        void StopServers();

        void Shutdown();

        void End();
    }

    public interface IThorSimulator : INotifyPropertyChanged, IThorSimulatorStateMethods
    {
        bool SimulatorRunning { get; }

        int LoadModel(string modelFileName);

        int UnloadModel();
    }

    public interface IThorSimulatorVisual
    {
        bool DiscoveryServerRunning { get; }
        bool AuthenticationServerRunning { get; }
    }

    public interface IDeviceCommMgr : INotifyPropertyChanged
    {
        void StartAuthenticationServer();
        void StopAuthenticationServer();
        bool AuthenticationServerRunning { get; }

        void StartDiscoveryServer();
        void StopDiscoveryServer();
        bool DiscoveryServerRunning { get; }

        void StartCommandServer();
        void StopCommandServer();
        bool CommandServerRunning { get; }

        void StartGLinkDiscovery();
        void StopGLinkDiscovery();
        bool GLinkDiscoveryServerRunning { get; }

        // need to add glink discovery events
        // need to add command queue events
        // need to add access to command queue
        // need to add access to Send Data/Requests

		IDSStorageModel DataModel { get; set; }
    }

    public interface ISimCommsMgr : INotifyPropertyChanged
    {
        bool DiscoveryServerRunning { get; }
        int DiscoveryServerPort { get; }
        bool AuthenticationServerRunning { get; }
        int AuthenticationServerPort { get; }
    }

}
