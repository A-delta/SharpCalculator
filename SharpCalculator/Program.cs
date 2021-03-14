using System;

namespace VarCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculator calc = new Calculator();
            calc.ProcessExpression("1+2+33+4*4");
        }
    }
}