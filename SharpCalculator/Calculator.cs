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

            if (expression.Contains("=="))
            {
                _isEqual(expression);
            }

            else if (expression.Contains("+"))
            {

            }


        }

        private void _isEqual(String expression)
        {
            String[] members = expression.Split("==");

            List<int> items = new List<int>();
            foreach (String member in members)
            {
                items.Add(int.Parse(member));
            }

            foreach (int i in items)
            {
                foreach (int o in items)
                {
                    if (i != o)
                    {
                        Console.WriteLine("FALSE");
                        return;
                    }
                }
                Console.WriteLine("TRUE");
                return;

            }
        }

        ~Calculator()
        {
            Console.WriteLine("Calculator deleted");
        }

    }
}