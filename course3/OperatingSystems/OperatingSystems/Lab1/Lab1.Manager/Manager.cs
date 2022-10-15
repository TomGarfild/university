using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using Lab1.Common;
using Serilog;

namespace Lab1.Manager;

public class Manager
{
    private readonly Socket _socket;
    private const string Address = "127.0.0.1";
    private const int Port = 8005;

    private const string Path =
        "D:\\university\\course3\\OperatingSystems\\OperatingSystems\\Lab1\\Lab1.Clients\\bin\\Debug\\net6.0\\Lab1.Clients.exe";

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
            Log.Information("Manager started");
            Process.Start(Path, "f");

            var handlerF = _socket.Accept();
            var serverThreadF = new ClientThread(handlerF);
            var threadF = new Thread(serverThreadF.Start);
            threadF.Start();

            Process.Start(Path, "g");
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
            Log.Error(ex.Message);
        }
        finally
        {
            _socket.Close();
        }
    }

    private static void Prompt()
    {
        var showPrompt = true;
        ConsoleKey key = default;
        while (true)
        {
            if (showPrompt)
            {
                Console.WriteLine("(a) continue");
                Console.WriteLine("(b) continue without prompt");
                Console.WriteLine("(c) stop");
                key = Console.ReadKey().Key;
                Console.WriteLine();
            }

            switch (key)
            {
                case ConsoleKey.A:
                    break;
                case ConsoleKey.B:
                    showPrompt = false;
                    break;
                case ConsoleKey.C:
                    return;
                default:
                    Console.WriteLine("Wrong key");
                    continue;

            }

            if (ClientThread.IsReady)
            {
                var fRes = ClientThread.Result[Functions.F];
                var gRes = ClientThread.Result[Functions.G];

                Console.WriteLine($"Result {fRes} + {gRes}");
                break;
            }
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