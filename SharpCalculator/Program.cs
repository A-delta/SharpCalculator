using System;

namespace SharpCalculatorApp
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