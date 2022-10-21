using System.Net;
using System.Net.Sockets;
using System.Text;
using Lab1.Common;

namespace Lab1.Clients;

public class Client
{
    private readonly Socket _socket;
    private readonly ClientParams _params;

    public Client(ClientParams @params)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _params = @params;
    }

    public void Start(Func<int, Optional<Optional<double?>>> func)
    {
        var ipPoint = new IPEndPoint(IPAddress.Parse(_params.Address), _params.Port);
        _socket.Connect(ipPoint);
        // Console.WriteLine($"{_params.Name} started");

        try
        {
            while (true)
            {
                var data = new byte[256];
                int bytes;

                do
                {
                    bytes = _socket.Receive(data);
                } while (_socket.Available > 0);
                var x = BitConverter.ToInt32(data, bytes - sizeof(int));

                var attemptsCount = 0;
                var error = string.Empty;

                while (attemptsCount < _params.MaxAttemptsCount && _socket.Available == 0)
                {

                    var result = func(x);

                    if (result.IsPresent())
                    {
                        var mid = result.Get();

                        if (mid.IsPresent())
                        {
                            if (_socket.Available == 0)
                            {
                                _socket.Send(Encoding.Unicode.GetBytes(mid.Get().ToString()!));
                                // Console.WriteLine($"\n{_params.Name} have sent result: {mid.Get()}");
                            }
                            error = string.Empty;
                            break;
                        }

                        error = $"Error: hard fail for {_params.Name} with x: {x}"; // hard fail
                        break;
                    }
                    else if(error == string.Empty)
                    {
                        error = $"Error: soft fail for {_params.Name} with x: {x}, retries: {_params.MaxAttemptsCount}"; // soft fail
                    }


                    attemptsCount++;
                }

                if (error != string.Empty && _socket.Available == 0)
                {
                    _socket.Send(Encoding.Unicode.GetBytes(error));
                    // Console.WriteLine($"\n{_params.Name}: {error}");
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