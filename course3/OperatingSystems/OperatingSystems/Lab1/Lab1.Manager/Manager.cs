using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Lab1.Manager;

public class Manager
{
    private readonly Socket _socket;
    private const string Address = "127.0.0.1";
    private const int Port = 8005;

    public Manager()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public void Start()
    {
        try
        {
            var ipPoint = new IPEndPoint(IPAddress.Parse(Address), Port);
            _socket.Bind(ipPoint);
            _socket.Listen(1);
            Console.WriteLine("Manager started");

            var handlerF = _socket.Accept();
            var serverThreadF = new ClientThread(handlerF);
            var threadF = new Thread(serverThreadF.Start);
            threadF.Start();
            
            var handlerG = _socket.Accept();
            var serverThreadG = new ClientThread(handlerG);
            var threadG = new Thread(serverThreadG.Start);
            threadG.Start();

            while (true)
            {
                Console.Write("Enter message:");
                var x = Console.ReadLine();
                serverThreadF.XChanged?.Invoke(this, new ClientThread.XArgs { X = x });
                serverThreadG.XChanged?.Invoke(this, new ClientThread.XArgs { X = x });
                Prompt();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            _socket.Close();
        }
    }

    private static void Prompt()
    {
        Console.WriteLine("(a) continue");
        Console.WriteLine("(b) continue without prompt");
        Console.WriteLine("(c) stop");
        var key = Console.ReadKey();

        switch (key.Key)
        {
            case ConsoleKey.A:
                Console.WriteLine("a");
                break;
            case ConsoleKey.B:
                Console.WriteLine("b");
                break;
            case ConsoleKey.C:
                return;

        }
    }
    

    public static void SoftFail()
    {
        Console.WriteLine("Soft fail");
        Environment.Exit(1);
    }

    public static void HardFail()
    {
        Console.WriteLine("Hard fail");
        Environment.Exit(2);
    }
}