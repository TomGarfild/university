using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Serilog;

namespace Lab1.Manager;

public class ClientThread
{
    public static readonly ConcurrentDictionary<string, double> Result = new();
    public static readonly ConcurrentBag<string> Errors = new();
    public static bool IsReady { get; private set; }

    private readonly Socket _socket;

    public EventHandler<XArgs> XChanged;

    public ClientThread(Socket socket)
    {
        _socket = socket;
        XChanged += SendMessage;
    }

    public void Start()
    {
        while (true)
        {
            var data = new byte[256];
            var messageBuilder = new StringBuilder();

            do
            {
                var bytes = _socket.Receive(data, data.Length, 0);
                messageBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (_socket.Available > 0);

            ProcessMessage(messageBuilder.ToString());

            if (Result.Count == 2 || Errors.Any())
            {
                IsReady = true;
            }
        }
    }

    public static void Reset()
    {
        Result.Clear();
        Errors.Clear();
        IsReady = false;
    }

    private void SendMessage(object sender, XArgs xArgs)
    {
        _socket.Send(Encoding.Unicode.GetBytes(xArgs.X));
    }

    private static void ProcessMessage(string message)
    {
        if (message != null)
        {
            if (message.StartsWith("Error:"))
            {
                Log.Error(message);
                Errors.Add(message);
                return;
            }

            var res = JsonConvert.DeserializeObject<KeyValuePair<string, double>>(message);
            if (!Result.TryAdd(res.Key, res.Value))
            {
                Log.Error($"Could not add {res.Key}:{res.Value}");
            }
        }
    }

    public class XArgs : EventArgs
    {
        public string X { get; set; }
    }
}