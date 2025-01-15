using System;
using System.Net;
using System.Net.Sockets;

namespace FrameworkUtils.Helpers
{
    public static class IPHelper
    {
        /// <summary>
        /// See: https://www.codegrepper.com/code-examples/csharp/c%23+get+pc+ip+address
        /// </summary>
        public static string GetLocalIPAddress(bool throwError = false)
        {
            string result = "";

            // Commented out. Does not work on VM machines
            //IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            //foreach (IPAddress ip in host.AddressList)
            //{
            //    if (ip.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        result = ip.ToString();
            //        if (result.StartsWith("192.168."))
            //            break;
            //    }
            //}

            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    result = endPoint.Address.ToString();
                }
            }
            catch (Exception)
            {
                if (throwError)
                    throw;
            }

            if (throwError)
                if (string.IsNullOrEmpty(result))
                    throw new Exception("No network adapters with an IPv4 address in the system!");

            return result;
        }
    }
}
