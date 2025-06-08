using PainterServer.Services;
using System;
class Program
{
    static void Main()
    {
        var server = new TcpSketchServer();
        server.Start();

        Console.WriteLine("Server is running. Press Enter to stop.");
        Console.ReadLine();

        server.Stop();
    }
}
