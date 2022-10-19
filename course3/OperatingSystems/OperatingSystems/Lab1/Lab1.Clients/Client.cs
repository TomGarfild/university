using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Lab1.Common;
using Newtonsoft.Json;
using Serilog;

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
        Log.Information($"{_params.Name} started");

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
                var error = string.Empty;

                while (attemptsCount < _params.MaxAttemptsCount)
                {

                    var result = func(x);

                    if (result.IsPresent())
                    {
                        var mid = result.Get();

                        if (mid.IsPresent())
                        {
                            var str = JsonConvert.SerializeObject(new KeyValuePair<string, string>(_params.ShortName, mid.Get()!.ToString()));
                            _socket.Send(Encoding.Unicode.GetBytes(str));
                            Log.Information($"{_params.Name} have sent result: {result}");
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

                if (error != string.Empty)
                {
                    _socket.Send(Encoding.Unicode.GetBytes(error));
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
        finally
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}