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
            logger.Log($"SharpCalculator {System.Reflection.Assembly.GetEntryAssembly().GetName().Version} initialised\n");

            _functionsNamesList = _GetAllFunctions();

            foreach (String functionName in _functionsNamesList)
            {
                IFunction function = _GetFunction(functionName);
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


            Console.WriteLine(">>  " + result + "\n");

        }

        private List<String> _CleanInfix(String expression)
        {
            List<String> cleanedInfixExpression = new List<string>();

            List<String> finalInfixExpression = new List<string>();
            String isFunctionMemory = "";
            String last = "";

            char[] characters = expression.ToCharArray();
            foreach (char token in characters)
            {
                if (token == ' ') { continue; }

                else if (token == '(')
                {
                    if (last == "fcn")
                    {
                        cleanedInfixExpression.Add("[");
                    }

                    else if (last != "ope" && last != "space" && last != "(" && last != "") // Implicit product
                    {
                        cleanedInfixExpression.Add("*");
                        cleanedInfixExpression.Add(token.ToString());
                        
                    }
                    else
                    {
                        cleanedInfixExpression.Add(token.ToString());
                    }

                    last = "(";
                }

                else if (token == ')')
                {
                    if (_areParenthesisMatching(string.Join("", cleanedInfixExpression)))
                    {
                        cleanedInfixExpression.Add("]");
                    }
                    else
                    {
                        cleanedInfixExpression.Add(token.ToString());
                    }
                    last = ")";
                }


                else if (".0123456789".Contains(token))  // DIGIT
                {
                    if (last == "nb") // >9 number
                        cleanedInfixExpression[cleanedInfixExpression.Count - 1] += token.ToString(); 
                    else
                    {
                        if (last != "nb" && last == ")") // Implicit product
                        {
                            cleanedInfixExpression.Add("*");
                        }
                        cleanedInfixExpression.Add(token.ToString());
                        last = "nb";
                    }
                }

                else if (_infixOperators.Contains(token.ToString())) // OPERATOR
                {
                    if (token == '-' && last != "nb" && last != ")")  // NEGATIVE NUMBER
                    {
                        cleanedInfixExpression.Add("0");
                    }
                    cleanedInfixExpression.Add(token.ToString());
                    last = "ope";
                }


                else if ((97 <= (int)token && (int)token <= 122) || (65 <= (int)token && (int)token <= 90))  // Is a character -> variable or fcn ?
                {
                    if (last != "ope" && (last == "nb" || last == ")")) // no operator -> implicit product
                    {
                        cleanedInfixExpression.Add("*");
                    }
                    if (last == "char")
                    {
                        cleanedInfixExpression[cleanedInfixExpression.Count - 1] += token.ToString();
                    }
                    else
                    {
                        cleanedInfixExpression.Add(token.ToString());
                    }
                    isFunctionMemory += (token);
                    if (_IsFunctionCall(isFunctionMemory) != "None")
                    {
                        last = "fcn";
                        isFunctionMemory = "";
                    }
                    else
                    {
                        last = "char";
                    }
                }
                else if (token == ',') { cleanedInfixExpression.Add(token.ToString()); last = "space"; }

            }

            if (cleanedInfixExpression.Contains("["))
            {

                String buffer = "";

                int countOpen = 0;
                int countClose = 0;

                foreach (String token in cleanedInfixExpression)
                {
                    switch (token)
                    {
                        case "[":
                            countOpen++;
                            buffer = buffer + token;

                            continue;

                        case "]":
                            countClose++;
                            buffer = buffer + token;
                            if (countOpen == countClose && countOpen != 0)
                            {
                                finalInfixExpression.Add(buffer);
                                countOpen = 0;
                                countClose = 0;
                                buffer = "";
                            }

                            continue;

                        default:
                            if ((_IsFunctionCall(token) != "None" && !_infixOperators.Contains(token)) || countOpen != countClose)
                            {
                                buffer = buffer + token;
                            }
                            else if (countOpen == 0)
                            {
                                finalInfixExpression.Add(token);
                            }
                            continue;
                    }

                    

                }

                if (countOpen == countClose && buffer != "")
                {
                    finalInfixExpression.Add(buffer);
                }
            }
            else
            {
                finalInfixExpression = cleanedInfixExpression;
            }

            return finalInfixExpression;
        }


        private bool _canOpenParenthesis(String lastToken)
        {
            if (" ,[()0123456789".Contains(lastToken[lastToken.Length - 1]) || _infixOperators.Contains(lastToken[lastToken.Length - 1].ToString()))
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
            }
            return ((countOpen == countClose && countOpen + countClose != 0) || countOpen + countClose == 0);
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

                    String correctFunctionName = _IsFunctionCall(function_name);
                    if (correctFunctionName != "None")
                    {

                        IFunction function = _GetFunction(correctFunctionName);

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
                                switch (ch)
                                {
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

                                foreach (String convertedArg in postfix_arg)
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
                        output.Add(_IsFunctionCall(popped));
                        popped = operatorStack.Pop().ToString();
                    }

                }

                else if (_infixOperators.Contains(token))
                {
                    while (operatorStack.Count != 0 && _operatorPriorities[(string)operatorStack.Peek()] >= _operatorPriorities[token])
                    {
                        output.Add(_IsFunctionCall((string)operatorStack.Pop()));
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
                output.Add(_IsFunctionCall((string)operatorStack.Pop()));
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

                else if (_IsFunctionCall(ch) != "None")
                {

                    IFunction function = _GetFunction(ch);

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

        private static List<string> _GetAllFunctions()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => typeof(IFunction).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                 .Select(x => x.Name).ToList();
        }

        private bool _IsVariableCall(String token)
        {
            return false;
        }

        private String _IsFunctionCall(String token)
        {
            if (_functionsNamesList.Contains(token))
            {
                return token;
            }

            foreach (String functionName in _functionsNamesList)
            {

                IFunction function = _GetFunction(functionName);
                if (function.getAliases().Contains(token))
                {
                    return functionName;
                }
            }

            return "None";
        }

        private IFunction _GetFunction(String functionName)
        {
            return (IFunction)System.Activator.CreateInstance(Type.GetType("SharpCalculatorLib.MathFunctions." + functionName));
        }


        ~Calculator()
        {
            Console.WriteLine("Calculator deleted");
        }

    }
}