using System;
using System.Collections.Generic;

namespace SharpCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enable verbose mode ? Y/[N] : ");
            String answer = Console.ReadLine();
            bool verbose = answer.ToLower().Contains("y");


            Calculator calc = new Calculator(verbose);

            while (true)
            {
                Console.Write("$  ");
                String input = Console.ReadLine();
                if (input.Length != 0)
                {
                    if (input == "cls") { Console.Clear(); }
                    else if (input == "help") { calc.PrintHelp(); }
                    else
                    {
                        /*try
                        {*/
                        calc.ProcessExpressionDebug(input);
                        /*}
                        catch
                        {
                            Console.WriteLine("An error occrured");
                        }*/
                    }
                }
            }
        }
    }
}