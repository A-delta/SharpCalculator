using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCalculator
{
    class Calculator
    {
        public Calculator()
        {
            Console.WriteLine("Calculator initialised\n");
        }

        public void ProcessExpression(String Infixexpression)
        {

            List<String> cleanedInfixExpression = _CleanInfix(Infixexpression);
            String postfixExpression = _ConvertToPostfix(cleanedInfixExpression);
            Console.WriteLine(">>  " + postfixExpression);

        }

        private List<String> _CleanInfix(String expression)
        {
            List<String> cleanedInfixExpression = new List<string>();
            bool lastWasNumeric = false;

            char[] charachters = expression.ToCharArray();
            foreach (char token in charachters)
            {
                if (token == ' ')
                {
                    lastWasNumeric = false;
                    continue;
                }

                else if ("0123456789".Contains(token))
                {
                    if (lastWasNumeric)
                        cleanedInfixExpression[cleanedInfixExpression.Count-1] += token.ToString();
                    else
                    {
                        cleanedInfixExpression.Add(token.ToString());
                        lastWasNumeric = true;
                    }
                }
                else
                {
                    lastWasNumeric = false;
                    cleanedInfixExpression.Add(token.ToString());
                }
            }

            return cleanedInfixExpression;
        }

        private String _ConvertToPostfix(List<String> expression)
        {

            Dictionary<String, int> priorities = new Dictionary<String, int>();
            priorities.Add("*", 3);
            priorities.Add("/", 3);
            priorities.Add("+", 2);
            priorities.Add("-", 2);
            priorities.Add("(", 1);

            Stack operatorStack = new Stack();
            List<String> output = new List<string>();

            foreach (string token in expression)
            {
                int temp;

                if (token == "")
                {
                    continue;
                }

                else if (int.TryParse(token, out temp))
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

                    if (popped == "(") {
                        Console.WriteLine("Implicit product");
                        output.Add("*");
                    }

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
            return postfixExpression;
        }


        ~Calculator()
        {
            Console.WriteLine("Calculator deleted");
        }

    }
}