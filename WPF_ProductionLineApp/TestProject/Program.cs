using Google.Protobuf;

namespace TestProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WebSocket_Server server = new WebSocket_Server();
            server.Open();
            Console.WriteLine("WebSocket server started. Press any key to stop.");
            Console.ReadKey();

        }


    }
}