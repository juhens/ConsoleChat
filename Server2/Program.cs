using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server
{
    public class Program
    {
        public static List<Socket> clientList = new List<Socket>();
        static void Main(string[] args)
        {
            Socket? listenSocket = null;
            Socket? clientSocket = null;

            try
            {
                IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
                IPEndPoint endPoint = new IPEndPoint(ipAddr, 4110);
                listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(endPoint);
                listenSocket.Listen();

                while (true)
                {
                    Console.WriteLine("Listening....");

                    // 클라이언트를 입장 시킴 (대리인)
                    clientSocket = listenSocket.Accept();
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

                                byte[] sendBuff = new byte[rand];
                                rd.NextBytes(sendBuff);
                                sendBuff[1] = 0x80;
                                sendBuff[2] = (byte)(rand - 4);
                                sendBuff[3] = 0x00;
                                sendBuff[0] = 0x64;

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
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }
            }

            
        }
    }
 }