using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class Program
    {
        public static List<Socket> list = new List<Socket>();
        static void Main(string[] args)
        {
            TcpListener tcpListener;
            Socket? clientSocket = null;

            try
            {
                IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
                tcpListener = new TcpListener(ipAddr, 4110);
                tcpListener.Start();

                while(true)
                {
                    clientSocket = tcpListener.AcceptSocket();
                    ClientHandler clientHandler = new ClientHandler(clientSocket);
                    Thread t = new Thread(new ThreadStart(clientHandler.chat));
                    t.Start();
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (clientSocket != null)
                    clientSocket.Close();
            }

        }
    }

    public class ClientHandler
    {
        NetworkStream? stream;
        StreamReader? reader;
        StreamWriter? writer;
        Socket? socket;
        public ClientHandler(Socket clientSocket)
        {
            this.socket = clientSocket;
            Program.list.Add(socket);
        }
        public void chat()
        {
            try
            {
                if (socket != null)
                {
                    stream = new NetworkStream(socket);
                    reader = new StreamReader(stream);
                }
                else
                    throw new Exception();

                while (true)
                {
                    string str = reader.ReadLine() ?? "";
                    Console.WriteLine(str);


                    foreach(Socket s in Program.list)
                    {
                        if (s != socket)
                        {
                            stream = new NetworkStream(s);
                            writer = new StreamWriter(stream)
                            {
                                AutoFlush = true
                            };
                            writer.WriteLine(str);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (socket != null)
                {
                    Program.list.Remove(socket);
                    socket.Close();
                }
                socket = null;
            }
        }
    }
}