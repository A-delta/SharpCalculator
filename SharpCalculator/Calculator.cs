using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpCalculator
{
    class Calculator
    {
        bool _verbose;
        List<String> _functionsNamesList;

        public Calculator(bool verbose)
        {
            _verbose = verbose;
            Console.WriteLine("Calculator initialised\n");
            _functionsNamesList = GetAllFunctions();


        }

        public void PrintHelp()
        {
            foreach (String functionName in _functionsNamesList)
            {
                Console.WriteLine(functionName);
            }
        }

        public void ProcessExpressionDebug(String Infixexpression)
        {
            var watchCleaning = System.Diagnostics.Stopwatch.StartNew();

            List<String> cleanedInfixExpression = _CleanInfix(Infixexpression);

            watchCleaning.Stop();
            if (_verbose)
            {
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("✓");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("]");

                Console.WriteLine("[" + watchCleaning.ElapsedMilliseconds + "ms]\n");
            }



            var watchConversion = System.Diagnostics.Stopwatch.StartNew();

            List<String> postfixExpression = _ConvertToPostfix(cleanedInfixExpression);

            watchConversion.Stop();
            if (_verbose)
            {
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("✓");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("]");


                Console.WriteLine("[" + watchConversion.ElapsedMilliseconds + "ms]\n");
            }



            var watchEvaluation = System.Diagnostics.Stopwatch.StartNew();

            Double result = _EvaluatePostfixExpression(postfixExpression);

            watchEvaluation.Stop();
            if (_verbose)
            {
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("✓");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("]");
                Console.WriteLine("[" + watchEvaluation.ElapsedMilliseconds + "ms]\n");

                double total = (double)watchCleaning.ElapsedMilliseconds + (double)watchConversion.ElapsedMilliseconds + (double)watchEvaluation.ElapsedMilliseconds;
                Console.WriteLine("[Tot: " + total + "ms]");
            }

            Console.WriteLine(">>  " + result);


        }


        public void ProcessExpression(String Infixexpression)
        {
            List<String> cleanedInfixExpression = _CleanInfix(Infixexpression);
            List<String> postfixExpression = _ConvertToPostfix(cleanedInfixExpression);
            Double result = _EvaluatePostfixExpression(postfixExpression);
            Console.WriteLine(">>  " + result);

        }


        private List<String> _CleanInfix(String expression)  // So dirty...
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

                else if (last == 2 && "-+*/^".Contains(token))  // double operator -> error
                {
                    Exception error = new Exception("Error in input expression." + token);
                    Console.WriteLine(error);

                }

                else if ("0123456789".Contains(token) || token == '.')
                {
                    if (last == 1)
                        cleanedInfixExpression[cleanedInfixExpression.Count - 1] += token.ToString();
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
                    if (token == '(' && last != 2 && last != 0)
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

                else if ("-+*/^".Contains(token))
                {

                    if (token == '-' && last != 1 && last != 4)
                    {

                        cleanedInfixExpression.Add("0");
                    }

                    cleanedInfixExpression.Add(token.ToString());
                    last = 2;
                }

                else if ((97 <= (int)token && (int)token <= 122) || (65 <= (int)token && (int)token <= 90))  // Is a character -> variable or fcn ?
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
                Console.Write("[Clean] ");
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
            priorities.Add("^", 4);
            priorities.Add("*", 3);
            priorities.Add("/", 3);
            priorities.Add("+", 2);
            priorities.Add("-", 2);  // -> properties
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

                else if (token.Contains('['))
                {
                    String function_name = token.Split('[')[0];

                    String correctFunctionName = _isFunctionCall(function_name);
                    if (correctFunctionName != "None")
                    {

                        IFunction function = _getFunction(correctFunctionName);
                        String temp = token.Substring(token.IndexOf('[') + 1);
                        temp = temp.Remove(temp.Length - 1);
                        Array args = temp.Split(',');

                        if (function.ArgumentsCount != args.Length)
                        {
                            throw new Exception("Error in arguments number for " + function_name);
                        }
                        else
                        {
                            foreach (String arg in args)
                            {
                                List<String> cleaned_arg = _CleanInfix(arg);
                                List<String> postfix_arg = _ConvertToPostfix(cleaned_arg);
                                double to_output = _EvaluatePostfixExpression(postfix_arg);
                                output.Add(to_output.ToString());

                            }
                            output.Add(correctFunctionName);
                        }
                    }
                    else
                    {
                        throw new Exception("Error in function call");
                    }

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
                        //output.Add(popped);

                        output.Add(_isFunctionCall(popped));
                        popped = operatorStack.Pop().ToString();

                    }

                }

                else if ("+-*/^".Contains(token))
                {
                    while (operatorStack.Count != 0 && priorities[(string)operatorStack.Peek()] >= priorities[token])
                    {
                        //output.Add((string)operatorStack.Pop());
                        output.Add(_isFunctionCall((string)operatorStack.Pop()));
                    }
                    operatorStack.Push(token);
                }

                else if (token.Contains('(') && token.Contains(')'))
                {
                    continue;
                }

            }

            while (operatorStack.Count != 0)
            {
                //output.Add((string)operatorStack.Pop());
                output.Add(_isFunctionCall((string)operatorStack.Pop()));
            }

            //String postfixExpression = string.Join(" ", output.ToArray());

            if (_verbose)
            {

                Console.Write("[Postfix] ");
                foreach (string a in output)
                {
                    Console.Write(a + ' ');
                }
                Console.WriteLine();
            }

            return output;
        }

        private Double _EvaluatePostfixExpression(List<String> PostfixExpression)
        {
            Double result = 0;
            Double temp;

            Double ope1;
            Double ope2;

            Stack operands = new Stack();

            if (_verbose)
            {
                Console.Write("[Calculate] :\n");
            }


            foreach (String ch in PostfixExpression)
            {

                if (Double.TryParse(ch, out temp))
                {
                    operands.Push(temp);
                }

                else if (_isFunctionCall(ch) != "None")
                {

                    IFunction function = _getFunction(ch);

                    int argumentsCount = function.ArgumentsCount;
                    List<Double> args = new List<Double>();
                    for (int i = 0; i < argumentsCount; i++)
                    {
                        args.Add(Convert.ToDouble(operands.Pop()));

                    }

                    result = function.ExecuteFunction(args);
                    operands.Push(result);
                }
                /*else if (_isVariableCall(ch))
                {
                    Console.WriteLine("This function doesnt exist. => is it a variable ?");
                }*/

                else if ("-+/*^".Contains(ch))
                {

                    Double.TryParse(operands.Pop().ToString(), out ope1);
                    Double.TryParse(operands.Pop().ToString(), out ope2);
                    switch (ch)
                    {
                        case "-":
                            result = ope2 - ope1;
                            break;
                        case "+":
                            result = ope1 + ope2;
                            break;

                        case "/":

                            if (operands.Count == 0)
                            {
                                Console.WriteLine("Last ?");
                                result = ope2 / ope1;
                            }

                            result = ope2 / ope1;

                            break;

                        case "*":
                            result = ope1 * ope2;

                            break;


                        case "^":
                            result = Math.Pow(ope2, ope1);

                            break;

                    }
                    if (_verbose) { Console.WriteLine("\t >>  " + ope2 + ch + ope1 + " = " + result); }
                    operands.Push(result);
                }
            }
            Double.TryParse(operands.Pop().ToString(), out result);


            return result;
        }




        private static List<string> GetAllFunctions()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => typeof(IFunction).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                 .Select(x => x.Name).ToList();
        }



        private bool _isVariableCall(String token)
        {
            return false;
        }

        private String _isFunctionCall(String token)
        {
            if (_functionsNamesList.Contains(token)) 
            {
                return token; 
            }

            foreach (String functionName in _functionsNamesList)
            {

                IFunction function = _getFunction(functionName);
                if (function.getAliases().Contains(token)) {
                    return functionName;
                }
            }

            return "None";
        }

        private IFunction _getFunction(String functionName)
        {
            return (IFunction)System.Activator.CreateInstance(Type.GetType("SharpCalculator.MathFunctions." + functionName));
        }


        ~Calculator()
        {
            Console.WriteLine("Calculator deleted");
        }

    }
}