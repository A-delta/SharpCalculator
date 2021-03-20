using System;

namespace SharpCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculator calc = new Calculator(true);

            while (true)
            {
                Console.Write("$  ");
                String input = Console.ReadLine();
                if (input.Length != 0)
                {
                    if (input == "cls") { Console.Clear(); }
                    else
                    {
                        calc.ProcessExpressionDebug(input);
                    }
                }
            }
        }
    }
}