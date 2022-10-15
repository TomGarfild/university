namespace Lab1.Clients;

public class ClientParams
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