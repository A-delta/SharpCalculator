using System;

namespace VarCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculator calc = new Calculator();
            calc.ProcessExpression("1==1");
            calc.ProcessExpression("1==2");
            calc.ProcessExpression("4==5==4");
            calc.ProcessExpression("2==2==2");
        }
    }
}