using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace pingex
{
    class Program
    {
        private static object lockConsoleObject = false;
        private const string DateFormat = "dd.MM.yyyy hh:mm:ss.ffff";

        static void Main(string[] args)
        {
            List<Thread> tp = new List<Thread>();

            foreach (var arg in args)
            {
                string[] data = arg.Split(':');
                Thread th = new Thread(new ParameterizedThreadStart(Ping));
                th.Start(new object[] {data[0], int.Parse(data[1])});
            }

            do
            {
                Thread.Sleep(1000);
            }
            while(true);
        }

        static void Ping(object args)
        {
            string host = (string)((object[]) args)[0];
            int port = (int)((object[])args)[1];

            do
            {
                try
                {
                    Socket s = ConnectSocket(host, port);
                    if (s != null)
                    {
                        lock (lockConsoleObject)
                        {
                            Console.WriteLine($"{DateTime.Now.ToString(DateFormat)} [OK] {host}:{port} connected");
                        }
                        s.Close();
                    }
                    else
                    {
                        lock (lockConsoleObject)
                        {
                            Console.WriteLine($"{DateTime.Now.ToString(DateFormat)} [ERR] {host}:{port} filed to connect");
                        }
                    }
                }
                catch (Exception e)
                {
                    lock (lockConsoleObject)
                    {
                        Console.WriteLine($"{DateTime.Now.ToString(DateFormat)} [ERR] {host}:{port} {e.Message}");
                    }
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            } while (true);
        }
        
        private static Socket ConnectSocket(string host, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            hostEntry = Dns.GetHostEntry(host);

            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }
    }
}
