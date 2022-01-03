using System;
using TimpInterfaces;
using Slf;
using System.Threading.Tasks;
using ClientEngine;
using ClientEngine.EngineStates;
using System.Threading;

namespace LinuxTerminal
{
	class MainClass
	{
#if USING_COMPOSITELOG
        static ILogger[] loggers = new ILogger[] {LoggerService.GetLogger() };
#endif
        static ILogger logger = null;
        static  SimulatorEngine deviceSim = null;
		static EventWaitHandle waitForGodot = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
        
#if USING_COMPOSITELOG
			logger = new Slf.CompositeLogger(loggers);
#else
			logger = LoggerService.GetLogger();
#endif
            deviceSim = new SimulatorEngine();

            deviceSim.Start();

            logger.Debug("Device Simulation Started");

            // Spin until done
            while (deviceSim.State != EndState.Instance)
            {
                if(deviceSim.State != RunState.Instance)
                    deviceSim.NextState();
                else
					waitForGodot.WaitOne();
            }

            logger.Debug("Device Simulation Exiting");
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            logger.Debug("Stopping Device Simulation.....");
            //deviceSim.ChangeState(StopState.Instance);
            deviceSim.ToStop();
			waitForGodot.Set ();
            e.Cancel = true;
        }
	}
}
