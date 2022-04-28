using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ChatClient
{
    public class Program
    {
        static string userName { get; set; }
        const int port = 31337;
        static TcpClient client = new TcpClient();
        public static void Main(string[] args)
        {
            
            Console.WriteLine("Client startet");
            Console.Write("Enter user name: ");
            userName = Console.ReadLine();
            IPAddress address;
            do
            {
                Console.Write("Enter server address: ");
            }while(!(IPAddress.TryParse(Console.ReadLine(), out address)));
            Console.WriteLine();
            ConnectToServer(address.ToString(), port);
            //ConnectToServer("127.0.0.1", port);
            Thread thread = new Thread(ReceiveBroadcast);
            thread.Start();
            while (true)
            {
                string msgFromUser = ReceiveMessageFromUser();
                SendMessage(userName + ": " + msgFromUser);
            }
        }

        public static string ReceiveMessageFromUser()
        {
            Console.Write(">");
            return Console.ReadLine();
        }

        public static void SendMessage(string message)
        {
            if (client.Connected)
            {
                Stream stream = client.GetStream();
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
                stream.Write(bytes);
            }
        }

        public static void ReceiveBroadcast()
        {
            while (true)
            {
                if (client.Connected)
                {
                    Stream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int read = stream.Read(buffer, 0, buffer.Length);
                    string message = System.Text.Encoding.UTF8.GetString(buffer, 0, read);
                    if(!(message.Contains(userName + ": ")))
                        Console.WriteLine(message);
                }
            }
        }

        public static bool ConnectToServer(string ip, int port)
        {
            client.Connect(ip, port);
            return client.Connected;
        } 
    }

}