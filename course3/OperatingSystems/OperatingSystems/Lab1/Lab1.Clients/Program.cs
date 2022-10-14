using System.Net;
using System.Net.Sockets;
using System.Text;
using Lab1.Clients.Functions;

namespace Lab1.Clients
{
    internal class Program
    {
        internal record ClientParams
        {
            public ClientParams(string name)
            {
                Name = name;
            }

            internal string Name { get; }
            internal int Port { get; init; } = 8005;
            internal string Address { get; init; } = "127.0.0.1";
            internal int MaxAttemptsCount { get; init; } = 5;
            internal int SleepTime { get; init; } = 2000;
        }

        private static readonly Dictionary<string, Func<int, double?>> Functions = new()
            { { "f", DoubleOps.TrialF }, { "g", DoubleOps.TrialG } };


        private static void Main(string[] args)
        {
            if (Functions.TryGetValue("f", out var func))
            {
                StartClient(func, new ClientParams($"F function client"));
            }
            else
            {
                Console.WriteLine($"No such function: F");
            }
        }

        private static void StartClient(Func<int, double?> func, ClientParams args)
        {
            var ipPoint = new IPEndPoint(IPAddress.Parse(args.Address), args.Port);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipPoint);

            try
            {
                while (true)
                {
                    var builder = new StringBuilder();
                    var data = new byte[256];

                    do
                    {
                        var bytes = socket.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (socket.Available > 0);

                    var x = int.Parse(builder.ToString());

                    var attemptsCount = 0;

                    while (attemptsCount < args.MaxAttemptsCount)
                    {
                        Thread.Sleep(args.SleepTime);
                        var rand = Random.Shared.Next();

                        if (rand % 2 == 0)
                        {
                            var result = func(x);

                            if (result != null)
                            {
                                socket.Send(Encoding.Unicode.GetBytes(result.ToString() ?? string.Empty));
                                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {args.Name} have sent result: {result}");
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{DateTime.Now.ToShortTimeString()} | {args.Name} have failed");
                        }

                        attemptsCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
    }
}