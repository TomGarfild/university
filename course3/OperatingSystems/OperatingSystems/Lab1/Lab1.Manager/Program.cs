namespace Lab1.Manager
{
    internal class Program
    {
        
        private static void Main(string[] args)
        {
            var manager = new Manager(true);
            manager.Start();
            Console.ReadKey();
        }
    }
}