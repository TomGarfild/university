using System.Net.Sockets;
using System.Text;

namespace Lab1.Manager;

public class ClientThread : IDisposable
{
    private string _result;

    private string Name { get; }

    private readonly Socket _socket;

    private volatile bool _isCancelled;

    public ClientThread(Socket socket, string name)
    {
        _socket = socket;
        Name = name;
    }

    public void Start()
    {
        while (true)
        {
            var data = new byte[256];
            var bytes = _socket.Receive(data, data.Length, 0);
            if (!_isCancelled)
            {
                _result = Encoding.Unicode.GetString(data, 0, bytes);
            }
        }
    }

    public void Cancel()
    {
        _isCancelled = true;
    }

    public void Send(int x)
    {
        _socket.Send(BitConverter.GetBytes(x));
        _result = null;
        _isCancelled = false;
    }
    
    public double? GetResult(ref bool cancelled)
    {
        if (_result == null)
        {
            if (cancelled)
            {
                Console.WriteLine($"{Name} client did not finish before cancellation");
            }
            return null;
        }

        if (double.TryParse(_result, out var res))
        {
            return res;
        }

        Console.WriteLine(_result); // Error
        cancelled = true;
        return null;
    }

    public void Dispose()
    {
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }
}