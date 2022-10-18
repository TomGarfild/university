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

    private readonly List<Process> _processes = new();

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


            var f = StartClient(Functions.F);
            var g = StartClient(Functions.G);

            var cancelled = false;

            while (!cancelled)
            {
                Console.Write("Enter x:");
                var x = Console.ReadLine();
                ClientThread.Reset();
                f.XChanged?.Invoke(this, new ClientThread.XArgs { X = x });
                g.XChanged?.Invoke(this, new ClientThread.XArgs { X = x });
                cancelled = Prompt();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
        finally
        {
            Dispose();
        }
    }

    private void Dispose()
    {
        foreach (var process in _processes)
        {
            process.Kill();
            process.WaitForExit();
            process.Dispose();
        }
        _socket.Close();
    }

    private ClientThread StartClient(string func)
    {
        var process = Process.Start(Path, func);
        _processes.Add(process);
        var handlerF = _socket.Accept();
        var serverThreadF = new ClientThread(handlerF);
        var threadF = new Thread(serverThreadF.Start);
        threadF.Start();

        return serverThreadF;
    }


    private static bool Prompt()
    {
        var showPrompt = true;
        while (true)
        {
            if (showPrompt)
            {
                Console.WriteLine("(a) continue");
                Console.WriteLine("(b) continue without prompt");
                Console.WriteLine("(c) stop");
                var key = Console.ReadKey().Key;
                Console.WriteLine();

                switch (key)
                {
                    case ConsoleKey.A:
                        break;
                    case ConsoleKey.B:
                        showPrompt = false;
                        break;
                    case ConsoleKey.C:
                        Console.WriteLine("Application is stopped...");
                        return true;
                    default:
                        Console.WriteLine("Wrong key");
                        continue;
                }
            }

            if (!ClientThread.IsReady) continue;

            if (ClientThread.Errors.Any())
            {
                foreach (var error in ClientThread.Errors)
                {
                    Console.WriteLine(error);
                }
                break;
            }

            var fRes = ClientThread.Result[Functions.F];
            var gRes = ClientThread.Result[Functions.G];
            var res = fRes * gRes;

            Console.WriteLine($"Result: {res}");
            break;
        }

        return false;
    }
}