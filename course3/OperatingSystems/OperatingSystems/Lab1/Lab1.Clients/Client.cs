using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab1.Clients;

public class Client
{
    private readonly Socket _socket;

    public Client()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); ;
    }

    public void Start(Func<int, double?> func, ClientParams args)
    {

        var ipPoint = new IPEndPoint(IPAddress.Parse(args.Address), args.Port);
        _socket.Connect(ipPoint);
        Console.WriteLine($"{args.Name} started");

        try
        {
            while (true)
            {
                var builder = new StringBuilder();
                var data = new byte[256];

                do
                {
                    var bytes = _socket.Receive(data);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (_socket.Available > 0);

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
                            _socket.Send(Encoding.Unicode.GetBytes(result.ToString() ?? string.Empty));
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
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}