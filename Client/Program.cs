using System.Net.Sockets;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            TcpClient? client = null;

            try
            {
                client = new TcpClient();
                client.Connect("127.0.0.1", 4110);

                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream) { AutoFlush= true };

                ServerHandler serverHandler = new ServerHandler(reader);
                Thread t = new Thread(new ThreadStart(serverHandler.chat));
                t.Start();

                string dataToSend = Console.ReadLine() ?? "";

                while(true)
                {
                    writer.WriteLine(dataToSend);
                    if (dataToSend.IndexOf("<EOF>") > 1) break;
                    dataToSend = Console.ReadLine() ?? "";

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                    client = null;
                }

            }
        }
    }

    public class ServerHandler
    {
        StreamReader reader;
        public ServerHandler(StreamReader reader)
        {
            this.reader = reader;
        }

        public void chat()
        {
            try
            {
                while(true)
                {
                    Console.WriteLine(reader.ReadLine());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}