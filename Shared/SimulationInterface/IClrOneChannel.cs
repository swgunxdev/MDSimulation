using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulationInterface
{
    public interface IClrOneChannel
    {
        ulong Id { get; }
        ChannelTypes ChannelType { get; }
        string Name { get; set; }
    }
    public enum ChannelTypes : uint
    {
        Microphone = 0,
        Line = 1,
        Output = 2,
        PowerAmp = 3,
        TelcoRx = 4,
        TelcoTx = 5,
        USBRx = 6,
        USBTx = 7,
        VOIPRx = 8,
        VOIPTx = 9,
        BeamformingMic = 10,

        // Logical types that don't have physical counterpart
        Broadcaster = 1001,
        Listener = 1002,
    }
}