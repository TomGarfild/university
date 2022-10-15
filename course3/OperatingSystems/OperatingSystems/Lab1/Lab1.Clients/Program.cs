using Lab1.Clients.Functions;

namespace Lab1.Clients
{
    internal class Program
    {
        private static readonly Dictionary<string, Func<int, double?>> Functions = new()
            { { "f", DoubleOps.TrialF }, { "g", DoubleOps.TrialG } };


        private static void Main(string[] args)
        {
            if (Functions.TryGetValue(args[0], out var func))
            {
                var client = new Client();
                client.Start(func, new ClientParams($"{args[0].ToUpper()} function client"));
            }
            else
            {
                Console.WriteLine($"No such function: {args[0]}");
            }
        }
    }
}