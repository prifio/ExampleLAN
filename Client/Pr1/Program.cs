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
    class Client
    {
        //public Queue<int> q { get; private set; }
        private Socket SocketReceive;
        private Mutex mutOut;
        private int Send = 0, Receive = 0;
        public Task<int> TryReceiveAsync(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            return Task<int>.Factory.FromAsync(
                SocketReceive.BeginReceive(buffer, offset, size, flags, null, SocketReceive), SocketReceive.EndReceive);
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
                SocketReceive.BeginSend(buffer, offset, size, flags, null, SocketReceive), SocketReceive.EndReceive);
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

        public Task connect(string IP)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IP), 11000);
            return Task.Factory.FromAsync(
                SocketReceive.BeginConnect(ip, null, SocketReceive), SocketReceive.EndConnect);
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
                string s = "456";// Console.ReadLine();
                await SendAsync(s);
                mutOut.WaitOne();
                ++Send;
                Console.WriteLine("\t\t\t\tYou: " + s + " " + Receive + " " + Send);
                mutOut.ReleaseMutex();
            }
        }
        public Client()
        {
            SocketReceive = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mutOut = new Mutex();
        }
    }

    class A
    {
        const string IP = "192.168.1.48";
        public async Task run()
        {
            Client client = new Client();
            await client.connect(IP);
            Console.WriteLine("endConnect");
            client.InfReceive();
            await client.InfSend();
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