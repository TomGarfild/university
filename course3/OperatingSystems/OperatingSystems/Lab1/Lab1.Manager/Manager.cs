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
            Console.WriteLine("Manager started. Waiting for connections...");

            var handler = _socket.Accept();
            while (true)
            {
                Console.Write("Enter message:");
                var x = Console.ReadLine();

                try
                {
                    handler.Send(Encoding.Unicode.GetBytes(x));
                    var data = new byte[256];
                    var builder = new StringBuilder();

                    do
                    {
                        var bytes = handler.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (handler.Available > 0);

                    var res = double.Parse(builder.ToString());
                    Console.WriteLine($"Result: {res}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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