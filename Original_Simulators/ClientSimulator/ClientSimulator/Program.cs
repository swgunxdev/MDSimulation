using ClientEngine;
using ClientEngine.EngineStates;
using Slf;
using System;

namespace ClientConsole
{
    class Program
    {
        static ILogger[] loggers = new ILogger[] { LoggerService.GetLogger() };
        static ILogger logger = null;
        static SimulatorClientEngine deviceSim = null;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            logger = LoggerService.GetLogger();//new Slf.CompositeLogger(loggers);
            deviceSim = new SimulatorClientEngine();

            deviceSim.Start();
            logger.Debug("Client Simulation Started");

            // Spin until done
            while (deviceSim.State != EndState.Instance)
            {
                if (deviceSim.State != RunState.Instance)
                    deviceSim.NextState();
                else
                    // do processing
                    System.Threading.Thread.Sleep(500);
            }

            logger.Debug("Client Simulation Exiting");
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            logger.Debug("Stopping Device Simulation.....");
            //deviceSim.ChangeState(StopState.Instance);
            deviceSim.ToStop();
            e.Cancel = true;
        }
    }
}
