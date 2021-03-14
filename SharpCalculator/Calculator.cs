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
          

        }

        ~Calculator()
        {
            Console.WriteLine("Calculator deleted");
        }

    }
}