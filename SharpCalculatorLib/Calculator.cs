using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace SharpCalculatorLib
{
    public class Calculator
    {
        Logger _logger;
        State _state;

        public Calculator(bool verbose)
        {
            _logger = new Logger(verbose);
            _state = new State();

            var ci = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            ci.NumberFormat.NegativeInfinitySymbol = "-Infinity"; // some system printed "8" instead of +oo
            ci.NumberFormat.PositiveInfinitySymbol = "+Infinity";
            Thread.CurrentThread.CurrentCulture = ci;

            
        }

        public void ChangeVerboseState(bool newVerbose)
        {
            _logger.Verbose = newVerbose;
        }
        public Dictionary<string, IFunction> GetHelp()
        {
            Dictionary<string, IFunction> functionDict = new Dictionary<string, IFunction>();

            foreach (String functionName in _state.FunctionsNamesList)
            {
                functionDict.Add(functionName, GetFunction(functionName));
            }
            return functionDict;
        }

        public string ProcessExpression(String Infixexpression)
        {
            _logger.StartWatcher();
            List<String> cleanedInfixExpression = CleanInfix(Infixexpression);
            _logger.DisplayTaskEnd("[Clean] ", cleanedInfixExpression);

            _logger.StartWatcher();
            List<String> postfixExpression = ConvertToPostfix(cleanedInfixExpression);
            _logger.DisplayTaskEnd("[Postfix] ", postfixExpression);


            _logger.StartWatcher();
            _logger.Log("[Calculation] ");
            string result = EvaluatePostfixExpression(postfixExpression);
            _logger.DisplayTaskEnd("[Calculate] ");

            _logger.LogTotalDuration();

            return result;

        }

        public List<String> CleanInfix(String expression)
        {
            List<String> cleanedInfixExpression = new List<string>();

            List<String> finalInfixExpression = new List<string>();
            String isFunctionMemory = "";
            String isInfixOperatorMemory = "";
            String last = "";
            string last2 = "";
            bool space = false;
            char[] characters = expression.ToCharArray();
            foreach (char token in characters)
            {
                if (_state.InfixOperators.Contains(token.ToString())) // OPERATOR
                {
                    isInfixOperatorMemory += token.ToString();
                }

                else if (isInfixOperatorMemory.Length != 0)  // IS END OPERATOR ?
                {
                    if (isInfixOperatorMemory == "-" && last != "nb" && last != ")")  // NEGATIVE NUMBER
                    {
                        cleanedInfixExpression.Add("0");
                        cleanedInfixExpression.Add(isInfixOperatorMemory);
                    }
                    else
                    {
                        cleanedInfixExpression.Add(isInfixOperatorMemory);
                    }
                    isInfixOperatorMemory = "";
                    last = "ope";
                }

                if ((97 <= (int)token && (int)token <= 122) || (65 <= (int)token && (int)token <= 90))  // Is a character -> variable or fcn ?
                {
                    if (last != "ope" && (last == "nb" || last == ")" || (last == "char" && space == true))) // no operator -> implicit product
                    {
                        cleanedInfixExpression.Add("*");
                        last = "ope";
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
                else
                {
                    if (isFunctionMemory.Length != 0)
                    {
                        isFunctionMemory = "";
                    }
                }


                if (token == ',') { cleanedInfixExpression.Add(token.ToString()); last = "comma"; }


                space = (token == ' ' ? true : false);
                //else if (token == ' ') { space = true; continue; }

                if (token == '(')
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

                last2 = last;
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
                            if ((_IsFunctionCall(token) != "None" && !_state.InfixOperators.Contains(token)) || countOpen != countClose)
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
            return _VerifyCleanedExpression(finalInfixExpression);
        }

        private bool _canOpenParenthesis(String lastToken)
        {
            if (" ,[()0123456789".Contains(lastToken[lastToken.Length - 1]) || _state.InfixOperators.Contains(lastToken[lastToken.Length - 1].ToString()))
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

        private List<String> _VerifyCleanedExpression(List<string> cleanedExpression)
        {
            string expression = string.Join("  ", cleanedExpression.ToArray());
            if (expression.Contains(" = "))
            {
                string[] parts = expression.Split(" = ");
                string varName = parts[0];
                if (parts.Length > 2)
                {
                    throw new ArgumentException($"Bad argument number for variable assignements");
                }
                if (varName.Contains("True") || varName.Contains("False"))
                {
                    throw new NotSupportedException($"Illegal name {varName} for variable");
                }

                foreach (string ope in _state.InfixOperators)
                {
                    if (varName.Contains(ope))
                    {
                        throw new NotSupportedException($"Illegal name {varName} for variable");
                    }
                }

                foreach (char nb in "0.123456789")
                {
                    if (varName.Contains(nb))
                    {
                        throw new NotSupportedException($"Illegal name {varName} for variable");
                    }
                }
            }

            return cleanedExpression;
        }
        public List<String> ConvertToPostfix(List<String> expression)
        {
            Stack operatorStack = new Stack();
            List<String> output = new List<string>();
            expression.Add("");  // for checking if token is var to get or to assign...
            int index = -1;

            foreach (string token in expression)
            {
                index++;
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

                        IFunction function = GetFunction(correctFunctionName);

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
                            throw new ArgumentException("Error in arguments number for " + function_name);
                        }
                        else
                        {
                            foreach (String arg in args)
                            {
                                String cleanableArg = arg.Replace('[', '(').Replace(']', ')');

                                List<String> cleanedArg = CleanInfix(cleanableArg);

                                List<String> postfix_arg = ConvertToPostfix(cleanedArg);

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

                else if (_state.InfixOperators.Contains(token))
                {
                    while (operatorStack.Count != 0 && _state.OperatorPriorities[(string)operatorStack.Peek()] >= _state.OperatorPriorities[token])
                    {
                        output.Add(_IsFunctionCall((string)operatorStack.Pop()));
                    }
                    operatorStack.Push(token);
                }

                else if (token.Contains('(') && token.Contains(')'))
                {
                    continue;
                }

                else if (token == "True" || token == "False")
                {
                    output.Add(token);
                }

                else if (_IsVariableCall(token) && expression[index+1] != "=")
                {
                    output.Add(token);
                    output.Add("Get");

                }

                else if (expression[index + 1] == "=")
                {
                    output.Add(token);
                }
                else 
                {
                    //throw new InvalidOperationException();
                    output.Add(token);
                    output.Add("Get"); // temp but used to throws exception
                }

            }

            while (operatorStack.Count != 0)
            {
                output.Add(_IsFunctionCall((string)operatorStack.Pop()));
            }

            return output;
        }

        public string EvaluatePostfixExpression(List<String> PostfixExpression)
        {
            string result;
            double temp;

            Stack operands = new Stack();

            foreach (String ch in PostfixExpression)
            {

                if (Double.TryParse(ch, out temp))
                {
                    operands.Push(ch);
                }

                else if (ch == "true" || ch == "false")
                {
                    return ch;
                }

                else if (_IsFunctionCall(ch) != "None")
                {

                    IFunction function = GetFunction(ch);

                    int argumentsCount = function.ArgumentsCount;
                    List<string> args = new List<string>();
                    for (int i = 0; i < argumentsCount; i++)
                    {
                        var popped = operands.Pop();
                        args.Add(popped.ToString());
                    }

                    result = function.ExecuteFunction(_state, args);

                    _logger.LogCalculation(ch, args, result);
                    operands.Push(result);
                }
                else
                {
                    operands.Push(ch);
                }
            }
            return (string)operands.Pop();
        }

        public static List<string> GetAllFunctions()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => typeof(IFunction).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                 .Select(x => x.Name).ToList();
        }

        private bool _IsVariableCall(String token)
        {
            return _state.VarManager.ContainsKey(token) && !(token == "True") && !(token == "False");
        }

        private String _IsFunctionCall(String token)
        {
            if (_state.FunctionsNamesList.Contains(token))
            {
                return token;
            }

            foreach (String functionName in _state.FunctionsNamesList)
            {
                IFunction function = GetFunction(functionName);
                if (function.getAliases().Contains(token))
                {
                    return functionName;
                }
            }

            return "None";
        }

        public static IFunction GetFunction(String functionName)
        {
            return (IFunction)System.Activator.CreateInstance(Type.GetType("SharpCalculatorLib.MathFunctions." + functionName));
        }


        ~Calculator()
        {
            Console.WriteLine("Calculator deleted");
        }

    }
}