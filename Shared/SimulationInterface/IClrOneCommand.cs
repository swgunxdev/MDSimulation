using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulationInterface
{
    public enum ClrOneCommand
    {
        NOP = 0,
        Get = 1,
        Set = 2,
        Route = 3,
    }

    public interface IClrOneCommand
    {
        ClrOneCommand Command { get; }

        ulong Target1 { get; }

        ulong Target2 { get; }

        object Value { get; }
    }
}
