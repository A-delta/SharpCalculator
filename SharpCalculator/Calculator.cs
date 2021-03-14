using System;
using System.Collections.Generic;
using System.Text;

namespace VarCalculator
{
    class Calculator
    {
        public Calculator()
        {
            Console.WriteLine("Calculator initialised");
        }

        public void ProcessExpression(String expression)
        {
            Console.WriteLine("\nProcessing : " + expression);

            // if fcn in expression : do fcn

            if (expression.Contains('*') || expression.Contains('/'))
            {

            }

            

        }


        private int _Sum(int x, int y)
        {
            return x + y;
        }

        ~Calculator()
        {
            Console.WriteLine("Calculator deleted");
        }

    }
}