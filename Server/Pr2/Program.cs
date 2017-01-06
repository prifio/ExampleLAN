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
        public Socket SocketList, SocketSend;
        
        public Task<int> TryReceiveAsync(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            SocketSend.BeginReceive(buffer, offset, size, flags, iar =>
            {
                tcs.SetResult(SocketSend.EndReceive(iar));
            }, SocketSend);
            return tcs.Task;
        }
        public async Task<string> ReceiveAsync()
        {
            int size = 0;
            byte[] data = new byte[1024];
            while (true)
            {
                var received = await TryReceiveAsync(data, size, 1024 - size, SocketFlags.None);
                size += received;
                if (data[size - 1] == '\n')
                    return Encoding.ASCII.GetString(data, 0, size - 1);
                Console.WriteLine(received);
            }
        }
        public Task<int> TrySendAsync(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            SocketSend.BeginSend(buffer, offset, size, flags, iar =>
            {
                tcs.SetResult(SocketSend.EndSend(iar));
            }, SocketSend);
            return tcs.Task;
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
        public async Task InfReceive()
        {
            while (true)
            {
                string s = await ReceiveAsync();
                Console.WriteLine("\t\t\t\tYour frined: " + s);
            }
        }
        public async Task InfSend()
        {
            while (true)
            {
                string s = Console.ReadLine();
                await SendAsync(s);
                Console.WriteLine("\t\t\t\tYou: " + s);
            }
        }
        public void Create(string IP)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IP), 11000);
            SocketList.Bind(ip);
            SocketList.Listen(20);
            Console.WriteLine("endCreate");
        }
        public Task<Socket> Accept()
        {
            return Task<Socket>.Factory.FromAsync(
                SocketList.BeginAccept, SocketList.EndAccept, null);
        }
        public Server()
        {
            SocketList = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
    }

    class A
    {
        const string IP = "192.168.2.220";
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