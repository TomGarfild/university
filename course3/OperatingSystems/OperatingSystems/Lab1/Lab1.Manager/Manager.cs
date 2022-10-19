using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using Lab1.Common;
using Serilog;

namespace Lab1.Manager;

public class Manager
{
    private readonly bool IsDebug;
    private readonly Socket _socket;
    private const string Address = "127.0.0.1";
    private const int Port = 8005;

    private const string Path =
        "D:\\university\\course3\\OperatingSystems\\OperatingSystems\\Lab1\\Lab1.Clients\\bin\\Debug\\net6.0\\Lab1.Clients.exe";

    private readonly List<Process> _processes = new();

    public Manager(bool isDebug = false)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IsDebug = isDebug;
    }

    public void Start()
    {
        try
        {
            var ipPoint = new IPEndPoint(IPAddress.Parse(Address), Port);
            _socket.Bind(ipPoint);
            _socket.Listen(1);
            Log.Information("Manager started");


            using var f = StartClient(Functions.F);
            using var g = StartClient(Functions.G);

            var cancelled = false;

            while (!cancelled)
            {
                string x;
                do
                {
                    Console.Write("Enter x:");
                    x = Console.ReadLine();
                } while (!int.TryParse(x, out _));
                ClientThread.Reset();
                f.XChanged?.Invoke(this, new ClientThread.XArgs { X = x });
                g.XChanged?.Invoke(this, new ClientThread.XArgs { X = x });

                cancelled = Prompt();
            }

            Console.WriteLine("Application is stopped...");
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
        var process = IsDebug && func == "f" ? Process.GetProcessesByName("Lab1.Clients")[0] : Process.Start(Path, func);
        _processes.Add(process);
        var handler = _socket.Accept();
        var serverThread = new ClientThread(handler);
        var thread = new Thread(serverThread.Start);
        thread.Start();

        return serverThread;
    }


    private static bool Prompt()
    {
        var showPrompt = true;
        var cancelled = false;
        while (!cancelled)
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
                        cancelled = true;
                        break;
                    default:
                        Console.WriteLine("Wrong key");
                        continue;
                }
            }

            if (!(cancelled || ClientThread.IsReady)) continue;

            if (ClientThread.Errors.Any())
            {
                foreach (var error in ClientThread.Errors)
                {
                    Console.WriteLine(error);
                }
                break;
            }

            if (ClientThread.Result.TryGetValue(Functions.F, out var fRes) && cancelled)
            {
                Console.WriteLine("F client did not finish before cancellation");
            }

            if (ClientThread.Result.TryGetValue(Functions.G, out var gRes) && cancelled)
            {
                Console.WriteLine("G client did not finish before cancellation");
            }

            if (!cancelled)
            {
                var res = fRes * gRes;

                Console.WriteLine($"f: {fRes}, g:{gRes}, binary operation: *");
                Console.WriteLine($"Result: {res}");
            }
            break;
        }

        return cancelled;
    }
}