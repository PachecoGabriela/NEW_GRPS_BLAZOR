using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkUtils.Controllers
{
    public class FileAttachmentServerController
    {
        public static string LoadFromStream(string serveraddress, int serverport, string token, string filename, Stream stream)
        {
            //FA:I:TOKEN:SIZE:FILENAME
            string cmd = $"FA:I:{token}:{stream.Length}:{filename}";

            using (Connection conn = GetConnection(serveraddress, serverport))
            {
                WriteLine(cmd, conn.Stream);
                string result = ReadLine(conn.Stream);
                if (result == "OK")
                {
                    long sent = 0;
                    while (sent < stream.Length)
                    {
                        byte[] data = new byte[4096];
                        int len = stream.Read(data, 0, data.Length);
                        conn.Stream.Write(data, 0, len);
                        sent += len;
                    }

                    return ReadLine(conn.Stream);
                }

                return null;
            }
        }

        public static bool SaveToStream(string serveraddress, int serverport, string token, string filename, Stream stream)
        {
            string cmd = $"FA:O:{token}:{stream.Length}:{filename}";            
            using (Connection conn = GetConnection(serveraddress, serverport))
            {
                WriteLine(cmd, conn.Stream);
                long length = Convert.ToInt64(ReadLine(conn.Stream));
                if (length > 0)
                {
                    WriteLine("OK", conn.Stream);

                    long read = 0;
                    while (read < length)
                    {
                        byte[] buffer = new byte[5000];
                        int count = conn.Stream.Read(buffer, 0, buffer.Length);
                        stream.Write(buffer, 0, count);
                        read += count;
                    }

                    return true;
                }
                else
                    throw new InvalidOperationException("The file could not be located on the server");
            }
        }

        public static string MoveFile(string serveraddress, int serverport, string token, string relativeFilePath, string filePath)
        {
            string cmd = $"FA:M:{token}:{relativeFilePath}:{filePath}";

            using (Connection conn = GetConnection(serveraddress, serverport))
            {
                WriteLine(cmd, conn.Stream);
                return ReadLine(conn.Stream);
            }
        }

        public static string DeleteFile(string serveraddress, int serverport, string token, string relativeFilePath)
        {
            string cmd = $"FA:D:{token}:null:{relativeFilePath}";

            using (Connection conn = GetConnection(serveraddress, serverport))
            {
                WriteLine(cmd, conn.Stream);
                return ReadLine(conn.Stream);
            }
        }

        internal static string ReadLine(SslStream stream)
        {
            string text = "";
            while (!text.Contains("\r\n"))
            {
                byte[] buffer = new byte[4096];
                int len = stream.Read(buffer, 0, buffer.Length);
                text += Encoding.UTF8.GetString(buffer, 0, len);
                if (len == 0)
                    break;
            }

            int enterInd = text.IndexOf("\r\n");
            if (enterInd >= 0)
                text = text.Substring(0, enterInd);
            if (text.StartsWith("ERROR!"))
                throw new InvalidOperationException("An error occurred working with this attachment");

            return text;
        }

        internal static void WriteLine(string text, SslStream stream)
        {
            stream.Write(Encoding.UTF8.GetBytes(text + "\r\n"));
            stream.Flush();
        }

        private static Connection GetConnection(string serveraddress, int serverport)
        {
            TcpClient client = new TcpClient();
            client.Connect(serveraddress, serverport);
            SslStream stream = new SslStream(client.GetStream(), false, ValidateServerCertificate, null);
            stream.AuthenticateAsClient(serveraddress);
            return new Connection(stream, client);
           
        }

        public static bool ValidateServerCertificate(
             object sender,
             X509Certificate certificate,
             X509Chain chain,
             SslPolicyErrors sslPolicyErrors)
        {
            //#if DEBUG
            return true;
            //#else
            //            return sslPolicyErrors == SslPolicyErrors.None;
            //#endif
        }
    }

    internal class Connection : IDisposable
    {
        public SslStream Stream { get; private set; }
        public TcpClient Client { get; private set; }

        public Connection(SslStream stream, TcpClient client)
        {
            Stream = stream;
            Client = client;
        }

        public void Dispose()
        {
            if (Stream != null)
            {
                Stream.Dispose();
                Stream = null;
                Client = null;
            }
        }
    }
}
