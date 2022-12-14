namespace Lab1.Clients;

public class ClientParams
{
    public ClientParams(string shortName)
    {
        ShortName = shortName;
        Name = $"{shortName.ToUpper()} client";
    }

    internal string ShortName { get; }
    internal string Name { get; }
    internal int Port { get; init; } = 8005;
    internal string Address { get; init; } = "127.0.0.1";
    internal int MaxAttemptsCount { get; init; } = 3;
    internal int SleepTime { get; init; } = 2000;
}