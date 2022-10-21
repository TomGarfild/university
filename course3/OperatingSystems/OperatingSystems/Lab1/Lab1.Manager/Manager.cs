using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using Lab1.Common;

namespace Lab1.Manager;

public class Manager
{
    private readonly bool IsDebug;
    private readonly Socket _socket;
    private const string Address = "127.0.0.1";
    private const int Port = 8005;

    public string Path { get; }

    private readonly List<Process> _processes = new();

    public Manager(string path, bool isDebug = false)
    {
        Path = path;
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

            using var f = StartClient(Functions.F);
            using var g = StartClient(Functions.G);


            var cts = new CancellationTokenSource();

            while (true)
            {
                int x;
                do
                {
                    Console.Write("Enter x:");
                } while (!int.TryParse(Console.ReadLine(), out x));

                f.Send(x);
                g.Send(x);

                Prompt(f, g);
            }
        }
        catch (Exception ex)
        {
            if (IsDebug)
            {
                Console.WriteLine(ex.Message);
            }
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
        var clientThread = new ClientThread(handler, func);
        var thread = new Thread(() => clientThread.Start());
        thread.Start();

        return clientThread;
    }


    private static void Prompt(ClientThread f, ClientThread g)
    {
        var showPrompt = true;
        var cancelled = false;
        while (!cancelled)
        {
            if (showPrompt)
            {
                Console.WriteLine("(a) continue");
                Console.WriteLine("(b) continue without prompt");
                Console.WriteLine("(c) cancel");
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
                        f.Cancel();
                        g.Cancel();
                        cancelled = true;
                        break;
                    default:
                        Console.WriteLine("Wrong key");
                        continue;
                }
            }


            var fCancelled = cancelled;
            var gCancelled = cancelled;

            var fRes = f.GetResult(ref fCancelled);
            var gRes = g.GetResult(ref gCancelled);

            if (fCancelled || gCancelled)
            {
                break;
            }

            if (fRes.HasValue && gRes.HasValue)
            {
                var res = fRes * gRes;

                // Console.WriteLine($"f:{fRes}, g:{gRes}, binary operation: *");
                Console.WriteLine($"Result: {res}");
                break;
            }
        }
    }
}