using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Networking
{
    public class IPAddressHelper
    {
        public static IEnumerable<IPAddress> GetIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return (from ip in host.AddressList where !IPAddress.IsLoopback(ip) select ip).ToList();
        }

        public static IEnumerable<IPAddress> GetIP4Address()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return (from ip in host.AddressList where (!IPAddress.IsLoopback(ip) && (ip.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)) select ip).ToList();
        }
    }
}
