using System;
using System.Collections.Generic;
using SharpCalculatorLib;


namespace SharpCalculatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.Write("Enable verbose mode ? Y/[N] : ");
            //String answer = Console.ReadLine();
            //bool verbose = answer.ToLower().Contains("y");

            ConsoleApplication app = new ConsoleApplication(false);

            while (true)
            {
                Console.Write("$  ");
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
