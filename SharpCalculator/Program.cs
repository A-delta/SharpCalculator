using System;

namespace SharpCalculatorApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleApplication app = new(args);
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("$  ");
                Console.ForegroundColor = ConsoleColor.White;
                String input = Console.ReadLine();
                if (input.Length != 0)
                {
                    try
                    {
                        app.ProcessExpression(input);
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(e.Message + "\n");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }
    }
}