using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCalculator
{
    class Calculator
    {
        bool _verbose;

        public Calculator(bool verbose)
        {
            _verbose = verbose;
            Console.WriteLine("Calculator initialised\n");
        }

        public void ProcessExpression(String Infixexpression)
        {

            List<String> cleanedInfixExpression = _CleanInfix(Infixexpression);
            List<String> postfixExpression = _ConvertToPostfix(cleanedInfixExpression);
            decimal result = _EvaluatePostfixExpression(postfixExpression);
            Console.WriteLine(">>  " + result);

        }

        
        private List<String> _CleanInfix(String expression)
        {

            List<String> cleanedInfixExpression = new List<string>();
            int last = 0;  // 0 : space ; 1 : num ; 2 : operator ; 3 : ( ; 4 : ) ; 5 : fcn

            char[] characters = expression.ToCharArray();
            foreach (char token in characters)
            {
                if (token == ' ')
                {
                    continue;
                }
                
                if (last == 5)  // Last was a char -> function call
                {


                    if (token == ')')
                    {
                        cleanedInfixExpression[cleanedInfixExpression.Count - 1] += ']';
                        last = 4;
                    }
                    else if (token == '(')
                    {
                        cleanedInfixExpression[cleanedInfixExpression.Count - 1] += '[';
                        last = 5;
                    }

                    else
                    {
                        cleanedInfixExpression[cleanedInfixExpression.Count - 1] += token.ToString();
                        last = 5;
                    }
                }

                else if (last == 2 && "-+*/".Contains(token))  // double oeprator -> error
                {
                    Exception error = new Exception("Error in input expression."+token);
                    Console.WriteLine(error);
                    
                }

                else if ("0123456789".Contains(token) || token == '.')
                {
                    if (last == 1)
                        cleanedInfixExpression[cleanedInfixExpression.Count-1] += token.ToString();
                    else
                    {

                        if (last != 2 && last == 4)
                        {
                            Console.WriteLine("Implicit product");
                            cleanedInfixExpression.Add("*");
                        }

                        cleanedInfixExpression.Add(token.ToString());
                        last = 1;
                    }
                }
                else if ("()".Contains(token))
                {
                    if (token == '('  && last != 2 && last != 0)
                    {
                        Console.WriteLine("Implicit product");
                        cleanedInfixExpression.Add("*");
                    }
                    cleanedInfixExpression.Add(token.ToString());

                    if (token == '(')
                    {
                        last = 3;
                    }
                    else if (token == ')')
                    {
                        last = 4;
                    }
                }

                else if ("-+*/".Contains(token))
                {

                    if (token == '-' && last != 1 && last != 4)
                    {

                        cleanedInfixExpression.Add("0");
                    }

                    cleanedInfixExpression.Add(token.ToString());
                    last = 2;
                }

                else if (97 <= (int)token && (int)token <= 122)  // Is a character -> variable or fcn ?
                {
                    if (last != 2 && (last == 1 || last == 4)) // no operator 
                    {
                        Console.WriteLine("Implicit product");
                        cleanedInfixExpression.Add("*");
                    }

                    cleanedInfixExpression.Add(token.ToString());
                    last = 5;
                }

            }
            if (_verbose)
            {
                Console.Write("Cleaned : ");
                foreach (string a in cleanedInfixExpression)
                {
                    Console.Write(a + ' ');
                }
                Console.WriteLine();
            }

            return cleanedInfixExpression;
        }

        private List<String> _ConvertToPostfix(List<String> expression)
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
                int temp_parse_int;
                float temp_parse_float;

                if (token == "")
                {
                    continue;
                }

                else if (token.Contains('[')) {
                    String function_name = token.Split('[')[0];
                    String temp = token.Substring(token.IndexOf('[') + 1);
                    temp = temp.Remove(temp.Length-1);
                    Array args = temp.Split(',');
                    foreach (String arg in args)
                    {
                        List<String> cleaned_arg = _CleanInfix(arg);
                        List<String> postfix_arg = _ConvertToPostfix(cleaned_arg);

                        foreach (String pf_arg in postfix_arg)
                        {
                            output.Add(pf_arg);
                        }

                        output[output.Count-1] += ";"; // MAYBE ALREADY EVALUATE ARGS AT THAT POINT

                    }
                    output.Add(function_name);

                }

                else if (int.TryParse(token, out temp_parse_int))
                {
                    output.Add(token);
                }

                else if (float.TryParse(token, out temp_parse_float))
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

                else if (token.Contains('(') && token.Contains(')')) {
                    continue;
                }

            }

            while (operatorStack.Count!=0)
            {
                output.Add((string)operatorStack.Pop());
            }

            //String postfixExpression = string.Join(" ", output.ToArray());

            if (_verbose)
            {
                Console.Write("Postfix : ");
                foreach (string a in output)
                {
                    Console.Write(a + ' ');
                }
                Console.WriteLine();
            }
            
            return output;
        }

        private Decimal _EvaluatePostfixExpression(List<String> PostfixExpression)
        {
            decimal result = 0;
            decimal temp;

            decimal ope1;
            decimal ope2;

            Stack operands = new Stack();
            foreach(String ch in PostfixExpression)
            {

                if (Decimal.TryParse(ch, out temp))
                {
                    operands.Push(temp);
                }



                else if ("-+/*".Contains(ch))
                {

                    Decimal.TryParse(operands.Pop().ToString(), out ope1);
                    Decimal.TryParse(operands.Pop().ToString(), out ope2);
                    switch (ch)
                    {
                        case "-":
                            result = ope2 - ope1;
                            Console.WriteLine(ope2 + ch + ope1);
                            break;
                        case "+":
                            result = ope1 + ope2;
                            Console.WriteLine(ope2 + ch + ope1);
                            break;

                        case "/":
                            result = ope2 / ope1;

                            Console.WriteLine(ope2 + ch + ope1);
                            break;

                        case "*":
                            result = ope1 * ope2;

                            Console.WriteLine(ope2 + ch + ope1);
                            break;

                    }

                    operands.Push(result);
                }
            }
            Decimal.TryParse(operands.Pop().ToString(), out result);
            return result;
        }

        private bool _isFunctionCall()
        {
            return false;
        }

        private bool _isInt()
        {
            return false;
        }

        private bool _isFloat()
        {
            return false;
        }

        ~Calculator()
        {
            Console.WriteLine("Calculator deleted");
        }

    }
}