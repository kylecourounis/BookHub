namespace BookHub.Server.Core
{
    using System.Net;
    using System.Net.Sockets;

    internal static class Constants
    {
        /// <summary>
        /// Returns this computer's public IP address.
        /// </summary>
        public static IPAddress PublicIP
        {
            get
            {
                return IPAddress.Parse(new WebClient().DownloadString("http://api.ipify.org/"));
            }
        }

        /// <summary>
        /// Returns this computer's local IP address.
        /// </summary>
        public static IPAddress LocalIP
        {
            get
            {
                try
                {
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP))
                    {
                        socket.Connect("10.0.2.4", 65530);
                        return ((IPEndPoint)socket.LocalEndPoint).Address;
                    }
                }
                catch
                {
                    return IPAddress.Parse("127.0.0.1");
                }
            }
        }
    }
}
