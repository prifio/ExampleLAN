using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;


namespace ConsoleApplication1
{
    class Server
    {
        //public Queue<int> q { get; private set; }
        public Socket SocketList, SocketSend;
        private Mutex mutOut;
        private int Send = 0, Receive = 0;
        public Task<int> TryReceiveAsync(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            return Task<int>.Factory.FromAsync(
                SocketSend.BeginReceive(buffer, offset, size, flags, null, SocketSend), SocketSend.EndReceive);
        }
        public async Task<string> ReceiveAsync()
        {
            int size = 0;
            //int remaining = buffer.Length;
            byte[] data = new byte[1024];
            while (true)
            {
                var received = await TryReceiveAsync(data, size, 1024 - size, SocketFlags.None);
                size += received;
                //Console.WriteLine(Encoding.ASCII.GetString(data, 0, size));
                string result = Encoding.ASCII.GetString(data, 0, size);
                int IndOf = result.IndexOf('\n');
                if (IndOf != -1)
                    return result.Substring(0, IndOf);
                Console.WriteLine(received);
                //return result.Substring(0, result.Length - 1);
            }
        }
        
        public Task<int> TrySendAsync(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            return Task<int>.Factory.FromAsync(
                SocketSend.BeginSend(buffer, offset, size, flags, null, SocketList), SocketSend.EndReceive);
        }
        public async Task SendAsync(string s)
        {
            byte[] data = Encoding.ASCII.GetBytes(s + "\n");
            int itt = 0;
            int left = data.Length;
            while (left > 0)
            {
                var sended = await TrySendAsync(data, itt, left, SocketFlags.None);
                itt += sended;
                left -= sended;
            }
        }
        public void Create(string IP)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IP), 11000);
            SocketList.Bind(ip);
            SocketList.Listen(20);
            Console.WriteLine("endCreate");
        }
        public async Task InfReceive()
        {
            while (true)
            {
                string s = await ReceiveAsync();
                mutOut.WaitOne();
                ++Receive;
                Console.WriteLine("\t\t\t\tYour frined: " + s + " " + Receive + " " + Send);
                mutOut.ReleaseMutex();
            }
        }
        public async Task InfSend()
        {
            while (true)
            {
                string s = "123";// Console.ReadLine();
                await SendAsync(s);
                mutOut.WaitOne();
                ++Send;
                Console.WriteLine("\t\t\t\tYou: " + s + " " + " " + Receive + " " + Send);
                mutOut.ReleaseMutex();
            }
        }
        public Task<Socket> Accept()
        {
            return Task<Socket>.Factory.FromAsync(
                SocketList.BeginAccept, SocketList.EndAccept, null);
        }
        public Server()
        {
            SocketList = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mutOut = new Mutex();
        }
    }

    class A
    {
        const string IP = "192.168.1.48";
        public async Task run()
        {
            Server server = new Server();
            server.Create(IP);
            server.SocketSend = await server.Accept();
            Console.WriteLine("Ac");
            server.InfReceive();
            await server.InfSend();
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            var a = new A();
            a.run();
            while (true)
            {

            }
        }
    }
}