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
            int last = 0;  // 0 : space ; 1 : num ; 2 : operator ; 3 : () ; 4 ; fcn

            char[] characters = expression.ToCharArray();
            foreach (char token in characters)
            {
                if (last == 2 && "-+*/".Contains(token))
                {
                    Exception error = new Exception("Error in input expression."+token);
                    Console.WriteLine(error);
                    
                }

                if (token == ' ')
                {
                    continue;
                }

                else if ("0123456789".Contains(token))
                {
                    if (last == 1)
                        cleanedInfixExpression[cleanedInfixExpression.Count-1] += token.ToString();
                    else
                    {
                        cleanedInfixExpression.Add(token.ToString());
                        last = 1;
                    }
                }
                else if ("()".Contains(token))
                {


                    cleanedInfixExpression.Add(token.ToString());

                    last = 3;
                }

                else if ("-+*/".Contains(token))
                {
                    if (token == '-' && last != 1)
                    {
                        cleanedInfixExpression.Add("0");
                    }

                    cleanedInfixExpression.Add(token.ToString());
                    last = 2;
                }

                else if (97 <= (int)token && (int)token <= 122)
                {
                    /*if (last==4)
                    {
                        cleanedInfixExpression[cleanedInfixExpression.Count-1] += token.ToString();
                    }
                    else
                    { 
                        cleanedInfixExpression.Add(token.ToString());
                    }

                    last = 4;*/
                }
            }
            foreach (string a in cleanedInfixExpression)
            {
                Console.Write(a);
            }
            Console.WriteLine();
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