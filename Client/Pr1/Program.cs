﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
//using System.Net.Sockets;
using System.Net;
using Sockets.Plugin;

namespace ConsoleApplication1
{
    class Client
    {
        private TcpSocketClient SocketReceive;
        /*public Task<int> TryReceiveAsync(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            SocketReceive.BeginReceive(buffer, offset, size, flags, iar =>
            {
                tcs.SetResult(SocketReceive.EndReceive(iar));
            }, SocketReceive);
            return tcs.Task;
        }*/
        public async Task<string> ReceiveAsync()
        {
            int size = 0;
            byte[] data = new byte[1024];
            while (true)
            {
                var received = await SocketReceive.ReadStream.ReadAsync(data, size, 1024 - size);//TryReceiveAsync(data, size, 1024 - size, SocketFlags.None);
                size += received;
                if (data[size - 1] == '\n')
                    return Encoding.ASCII.GetString(data, 0, size - 1);
                Console.WriteLine(received);
            }
        }
        /*public Task<int> TrySendAsync(byte[] buffer, int offset, int size, SocketFlags flags)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            SocketReceive.BeginSend(buffer, offset, size, flags, iar =>
            {
                tcs.SetResult(SocketReceive.EndSend(iar));
            }, SocketReceive);
            return tcs.Task;
        }
        */
        public async Task SendAsync(string s)
        {
            byte[] data = Encoding.ASCII.GetBytes(s + "\n");
            int itt = 0;
            int left = data.Length;
            while (left > 0)
            {
                await SocketReceive.WriteStream.WriteAsync(data, itt, left);// TrySendAsync(data, itt, left, SocketFlags.None);
                left = 0;
            }
        }
        public async Task connect(string IP)
        {
            await SocketReceive.ConnectAsync(IP, 11000);
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
        public Client()
        {
            SocketReceive = new TcpSocketClient();
        }
    }

    class A
    {
        const string IP = "127.0.0.1";
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