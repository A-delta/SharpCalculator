using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCalculator
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

            _ConvertPostfix(expression);

        }

        private void _ConvertPostfix(String expression)
        {
            Console.WriteLine("Start converting to postfix");

            Dictionary<String, int> priorities = new Dictionary<String, int>();
            priorities.Add("*", 3);
            priorities.Add("/", 3);
            priorities.Add("+", 2);
            priorities.Add("-", 2);
            priorities.Add("(", 1);

            Stack operatorStack = new Stack();
            List<String> output = new List<string>();

            char[] infixExpression = expression.ToCharArray();

            foreach (char t in infixExpression)
            {

                String token = t.ToString();

                if ("1234567890".Contains(token))
                {
                    output.Add(token);
                }

                else if ("(" == (token))
                {
                    operatorStack.Push(token);
                }

                else if (")" == (token))
                {
                    String popped = operatorStack.Pop().ToString();
                    while (!(popped == "("))
                    {
                        output.Add(popped);
                        popped = operatorStack.Pop().ToString();
                    }

                }

                else if ("+-*/".Contains(token))
                {
                    

                    while (operatorStack.Count!=0 && priorities[(string)operatorStack.Peek()] >= priorities[token]) {

                        output.Add((string)operatorStack.Pop());
                    }

                    operatorStack.Push(token);


                }

            }

            while (operatorStack.Count!=0)
            {
                output.Add((string)operatorStack.Pop());
            }

            String postfixExpression = string.Join(" ", output.ToArray());
            Console.WriteLine("\nPostfix expression : " + postfixExpression);
        }


        ~Calculator()
        {
            Console.WriteLine("Calculator deleted");
        }

    }
}