using System;

namespace SharpCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculator calc = new Calculator();
            calc.ProcessExpression("6+9*7+22*(4-8*6)-8*(8-(9*7*(7-9)))");
        }
    }
}