using ClientEngine;
using ClientEngine.EngineStates;
using Slf;
using System;

namespace WindowsTerminal
{
    class Program
    {

        static ILogger[] loggers = new ILogger[] { LoggerService.GetLogger("debuglogger"), LoggerService.GetLogger() };
        static ILogger logger = null;
        static  SimulatorEngine deviceSim = null;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
        
            logger = new Slf.CompositeLogger(loggers);
            deviceSim = new SimulatorEngine();

            deviceSim.Start();
            logger.Debug("Device Simulation Started");

            // Spin until done
            while (deviceSim.State != EndState.Instance)
            {
                if(deviceSim.State != RunState.Instance)
                    deviceSim.NextState();
                else
                    // do processing
                    System.Threading.Thread.Sleep(500);
            }

            logger.Debug("Device Simulation Exiting");
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
