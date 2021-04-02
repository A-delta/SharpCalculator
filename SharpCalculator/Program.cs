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
            bool verbose = false;
            ConsoleApplication app = new ConsoleApplication(args, verbose);

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
                        if (verbose)
                        {

                            Console.Write(e + "\n");
                        }
                        else
                        {
                            Console.Write(e.Message + "\n");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }
    }
}
