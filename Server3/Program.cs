using System.Net.Sockets;
using System.Net;

namespace Server3
{
    internal class Program
    {
        public static List<Socket> list = new List<Socket>();
        static void Main(string[] args)
        {
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 2110);
            Socket listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //TcpListener tcpListener;
            //tcpListener = new TcpListener(ipAddr, 4110);
            // listenSocket에 IPEndPoint값과 소켓 타입, 프로토콜 설정 (TCP)
            listenSocket.Bind(endPoint);
            listenSocket.Listen(100);
            while (true)
            {
                Console.WriteLine("Listening....");

                // 클라이언트를 입장 시킴 (대리인)
                Socket clientSocket = listenSocket.Accept();

                // 클라이언트에서 오는 문자열 패킷 받기.
                while (true)
                {
                    try
                    {
                        byte[] recvBuff = new byte[1024];
                        int recvBytes = clientSocket.Receive(recvBuff);

                        if (recvBytes != 0)
                        {
                            Span<byte> span = recvBuff.AsSpan().Slice(0, recvBytes);
                            Console.WriteLine($"Recv : {BitConverter.ToString(span.ToArray()).Replace("-", " ")}");

                            Random rd = new Random();
                            int rand = rd.Next(251);
                            rand += 4;

                            //byte[] sendBuff = new byte[rand];
                            //rd.NextBytes(sendBuff);
                            //sendBuff[1] = 0x80;
                            //sendBuff[2] = (byte)(rand - 4);
                            //sendBuff[3] = 0x00;
                            //sendBuff[0] = 0x64;
                            byte[] sendBuff = { 0x64, 0x80, 0x0A, 0x00, 0x31, 0x32, 0x37, 0x2E, 0x30, 0x2E, 0x30, 0x2E, 0x31, 0x00 };


                            Console.WriteLine($"Send : {BitConverter.ToString(sendBuff).Replace("-", " ")}");
                            clientSocket.Send(sendBuff);
                        }
                        else
                            break;
                    }
                    catch
                    {
                        Console.WriteLine($"연결끊김");
                        break;
                    }
                }




                // 클라이언트 
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
    }
}