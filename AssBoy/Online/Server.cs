using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AssBoy.Desktop
{
    public class Server
    {
        public Action<string> DataReceived;

        public void StartServer()
        {
            byte[] bytes = new byte[2048];
            
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 69420);
            
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Socket handler = listener.Accept();

                    string data = "";
                    while (true)
                    {
                        int size = handler.Receive(bytes);
                        data += Encoding.Unicode.GetString(bytes, 0, size);
                        if (data.IndexOf("<EOF>", StringComparison.Ordinal) > -1) break;
                    }

                    DataReceived?.Invoke(data);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Server error: {e.Message}");
            }
            
            Console.WriteLine("Server quit.");
        }
    }
}