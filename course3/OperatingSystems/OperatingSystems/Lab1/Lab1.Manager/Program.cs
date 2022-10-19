namespace Lab1.Manager
{
    internal class Program
    {
        
        private static void Main(string[] args)
        {
            var path = "D:\\university\\course3\\OperatingSystems\\OperatingSystems\\Lab1\\Lab1.Clients\\bin\\Debug\\net6.0\\Lab1.Clients.exe";
            if (args.Length != 0)
            {
                path = args[0];
            }

            var manager = new Manager(path);
            manager.Start();
            Console.ReadKey();
        }
    }
}