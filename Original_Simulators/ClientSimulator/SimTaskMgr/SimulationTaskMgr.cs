using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimpInterfaces;
using TimpInterfaces.Implementations;

namespace SimTaskMgr
{
    public class SimulationTaskMgr : ITaskCommandClient
    {
        protected ITaskCommandReciever _cmdRcvr = null;
        protected List<ITaskCommand> _commands = null;
        public SimulationTaskMgr()
        {
            _commands = new List<ITaskCommand>();
        }


        public SimulationTaskMgr(ITaskCommandReciever cmdRcvr)
            : this()
        {
            _cmdRcvr = cmdRcvr;
        }

        public List<ITaskCommand> Commands { get { return _commands; } }

        public long CollectCommands()
        {
            // using MEF Collect Commands
            _commands.Add(new ConnectCmd(_cmdRcvr,"spencer", "password"));
            _commands.Add(new PrintCmd(_cmdRcvr, "Spencer sent a msg"));
            _commands.Add(new DisconnectCmd(_cmdRcvr));
            return _commands.Count;
        }

        public int Count { get { return _commands.Count();}}

        public long AddCommand(ITaskCommand cmd)
        {
            return cmd.Execute();
        }
    }
}
