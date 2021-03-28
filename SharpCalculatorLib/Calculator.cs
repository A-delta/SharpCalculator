using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpCalculatorLib
{
    public class Calculator
    {
        bool _verbose;
        Logger logger;

        List<String> _functionsNamesList;
        List<String> _infixOperators = new List<string>();

        Dictionary<String, int> _operatorPriorities = new Dictionary<String, int>();

        public Calculator(bool verbose)
        {
            _verbose = verbose;
            logger = new Logger(verbose);

            logger.Log("Calculator initialised\n");

            _functionsNamesList = GetAllFunctions();

            foreach (String functionName in _functionsNamesList)
            {
                IFunction function = _getFunction(functionName);
                String infixOperator = function.InfixOperator;

                if (infixOperator != "None")
                {
                    _infixOperators.Add(infixOperator);
                    _operatorPriorities.Add(infixOperator, function.InfixOperatorPriority);

                }
            }

            _operatorPriorities.Add("(", 1);
        }

        public void PrintHelp()  // Not finished
        {
            foreach (String functionName in _functionsNamesList)
            {
                Console.WriteLine(functionName);
            }
        }

        public void ProcessExpression(String Infixexpression)
        {

            logger.StartWatcher();
            List<String> cleanedInfixExpression = _CleanInfix(Infixexpression);
            logger.DisplayTaskEnd("[Clean] ", cleanedInfixExpression);


            logger.StartWatcher();
            List<String> postfixExpression = _ConvertToPostfix(cleanedInfixExpression);
            logger.DisplayTaskEnd("[Postfix] ", postfixExpression);


            logger.StartWatcher();
            logger.Log("[Calculation] ");
            Double result = _EvaluatePostfixExpression(postfixExpression);
            logger.DisplayTaskEnd("[Calculate] ");

            logger.LogTotalDuration();


            Console.WriteLine(">>  " + result);

        }



        private List<String> _CleanInfix(String expression)
        {
            List<String> cleanedInfixExpression = new List<string>();
            int last = 0;  // 0 : space ; 1 : num ; 2 : operator ; 3 : ( ; 4 : ) ; 5 : fcn
            char lastChar;
            int fcnToFinish = 0;

            char[] characters = expression.ToCharArray();
            foreach (char token in characters)
            {
                lastChar = token;
                if (token == ' ')
                {
                    continue;
                }

                if (last == 5)  // Last was a char -> function call
                {
                    if (token == ')')
                    {
                        if (_areParenthesisMatching(cleanedInfixExpression[cleanedInfixExpression.Count - 1]))
                        {
                            cleanedInfixExpression[cleanedInfixExpression.Count - 1] += ']';
                            fcnToFinish--;
                            if (fcnToFinish != 0)
                            {
                                last = 5;
                            }
                            else
                            {
                                last = 4;
                            }
                        }
                        else
                        {
                            cleanedInfixExpression[cleanedInfixExpression.Count - 1] += ')';
                            last = 5;
                        }
                    }
                    else if (token == '(')
                    {
                        if (_canOpenParenthesis(cleanedInfixExpression[cleanedInfixExpression.Count - 1]))
                        {
                            cleanedInfixExpression[cleanedInfixExpression.Count - 1] += '(';
                            last = 5;
                        }
                        else
                        {
                            cleanedInfixExpression[cleanedInfixExpression.Count - 1] += '[';
                            fcnToFinish++;
                            last = 5;
                        }
                    }
                    else
                    {
                        cleanedInfixExpression[cleanedInfixExpression.Count - 1] += token.ToString();
                        last = 5;
                    }
                }

                else if (last == 2 && "-+*/^".Contains(token))  // double operator -> error ( but what about == or || ?)
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

                else if (_infixOperators.Contains(token.ToString()))
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
            return cleanedInfixExpression;
        }


        private bool _canOpenParenthesis(String lastToken)
        {
            if (" ,[(0123456789".Contains(lastToken[lastToken.Length - 1]) || _infixOperators.Contains(lastToken[lastToken.Length - 1].ToString())) 
            {
                return true;
            }
            return false;
        }
        private bool _areParenthesisMatching(String lastToken)
        {
            if (lastToken.Contains(','))
            {
                lastToken = lastToken.Substring(lastToken.LastIndexOf(','));
            }


            int countOpen = 0;
            int countClose = 0;
            foreach (char ch in lastToken)
            {
                if (ch == '(')
                {
                    countOpen++;
                }

                if (ch == ')')
                {
                    countClose++;
                }
                if (countOpen == countClose && countOpen + countClose != 0)
                {
                    return true;
                }
            }
            return countOpen + countClose == 0;
        }

        private List<String> _ConvertToPostfix(List<String> expression)
        {
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

                        List<String> args = new List<string>();


                        if ((temp.Contains("[") || temp.Contains("]")))
                        {

                            String buffer = "";

                            int countOpen = 0;
                            int countClose = 0;

                            foreach (char ch in temp)
                            {
                                switch (ch) {
                                    case '[':
                                        countOpen++;

                                        buffer = buffer + ch;

                                        continue;

                                    case ']':
                                        countClose++;
                                        buffer = buffer + ch;
                                        


                                        continue;

                                    case ',':
                                        if (countOpen == countClose && buffer != "")
                                        {
                                            args.Add(buffer);
                                            buffer = "";
                                            countOpen = 0;
                                            countClose = 0;
                                        }
                                        else
                                        {
                                            buffer = buffer + ch;
                                        }

                                        continue;

                                    default:

                                        buffer = buffer + ch;
                                        continue;
                                }

                            }

                            if (countOpen == countClose && buffer != "")
                            {
                                args.Add(buffer);
                            }
                        }

                        

                        else  // simple args
                        {
                            args = temp.Split(',').ToList<String>();
                        }

                        if (function.ArgumentsCount != args.Count)
                        {
                            throw new Exception("Error in arguments number for " + function_name);
                        }
                        else
                        {
                            foreach (String arg in args)
                            {
                                String cleanableArg = arg.Replace('[', '(').Replace(']', ')');

                                List<String> cleanedArg = _CleanInfix(cleanableArg);

                                List<String> postfix_arg = _ConvertToPostfix(cleanedArg);

                                foreach(String convertedArg in postfix_arg)
                                {
                                    output.Add(convertedArg);
                                }
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
                        output.Add(_isFunctionCall(popped));
                        popped = operatorStack.Pop().ToString();
                    }

                }

                else if (_infixOperators.Contains(token))
                {
                    while (operatorStack.Count != 0 && _operatorPriorities[(string)operatorStack.Peek()] >= _operatorPriorities[token])
                    {
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
                output.Add(_isFunctionCall((string)operatorStack.Pop()));
            }

            return output;
        }


        private Double _EvaluatePostfixExpression(List<String> PostfixExpression)
        {
            Double result = 0;
            Double temp;

            Stack operands = new Stack();


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

                    logger.LogCalculation(ch, args, result);
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
            return (IFunction)System.Activator.CreateInstance(Type.GetType("SharpCalculatorLib.MathFunctions." + functionName));
        }


        ~Calculator()
        {
            Console.WriteLine("Calculator deleted");
        }

    }
}