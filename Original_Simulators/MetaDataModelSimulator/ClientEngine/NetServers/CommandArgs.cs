using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientEngine.NetServers
{
    public enum Command : uint
    {
        Noop,
        SendFirmwareUpdate,
        SendConfigurationFile,
        RebootStack,
        RebootDevice, // do we reboot a single device or the whole stack?
        TimeEvent,
    }

    public class CommandArgs
    {
        protected Command _cmdId = Command.Noop;
        public static readonly CommandArgs EmptyCmd = new CommandArgs();

        public CommandArgs()
        {
        }

        public CommandArgs(Command cmd)
        {
            _cmdId = cmd;
        }
    }
}