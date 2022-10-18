using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.CodeAnalysis;
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

    public void Start(Func<int, Optional<Optional<double>>> func)
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

                while (attemptsCount < _params.MaxAttemptsCount)
                {
                    Thread.Sleep(_params.SleepTime);
                    var rand = Random.Shared.Next();

                    if (rand % 2 == 0)
                    {
                        var result = func(x).Value.Value;

                        var str = JsonConvert.SerializeObject(new KeyValuePair<string, string>(_params.ShortName, result.ToString()!));
                        _socket.Send(Encoding.Unicode.GetBytes(str));
                        Log.Information($"{_params.Name} have sent result: {result}");
                        break;
                    }
                    else
                    {
                        Log.Error($"{_params.Name} have failed");
                    }

                    attemptsCount++;
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