using System.Net.Sockets;
using System.Text;

namespace Lab1.Manager;

public class ClientThread
{
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
        }
    }

    private void SendMessage(object sender, XArgs xArgs)
    {
        _socket.Send(Encoding.Unicode.GetBytes(xArgs.X));
    }

    private void ProcessMessage(string message)
    {
        Console.WriteLine(message);
        if (message != null)
        {
            if (message.Contains("Error"))
            {
                Console.WriteLine(message);
            }

            if (message.Contains("f") || message.Contains("g"))
            {
                Console.WriteLine(message);
            }
        }
    }

    public class XArgs : EventArgs
    {
        public string X { get; set; }
    }
}