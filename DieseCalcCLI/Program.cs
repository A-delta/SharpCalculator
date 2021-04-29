using System;

namespace DieseCalcCLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleApplication app = new(args);
            app.Run();
        }
    }
}