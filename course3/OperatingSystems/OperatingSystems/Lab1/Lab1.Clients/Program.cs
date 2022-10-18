using Lab1.Common.CompFuncs.Advanced;
using Microsoft.CodeAnalysis;

namespace Lab1.Clients
{
    internal class Program
    {
        private static readonly Dictionary<string, Func<int, Optional<Optional<double>>>> Functions = new()
            { { "f", DoubleOps.TrialF }, { "g", DoubleOps.TrialG } };


        private static void Main(string[] args)
        {
            if (Functions.TryGetValue(args[0], out var func))
            {
                var client = new Client(new ClientParams(args[0]));
                client.Start(func);
            }
            else
            {
                Console.WriteLine($"No such function: {args[0]}");
            }
        }
    }
}