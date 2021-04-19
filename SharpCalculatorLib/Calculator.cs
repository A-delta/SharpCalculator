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
        private readonly Logger _logger;
        public State State;

        private enum TokenTypes
        {
            None,
            Character,
            Function,
            Comma,
            Number,
            Operator,
            RightParenthesis,
            LeftParenthesis
        }

        public Calculator(bool verbose)
        {
            _logger = new Logger(verbose);
            State = new State();

            CultureInfo cultureInfo = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            cultureInfo.NumberFormat.NegativeInfinitySymbol = "-Infinity"; // some system printed "8" instead of +oo
            cultureInfo.NumberFormat.PositiveInfinitySymbol = "+Infinity";
            Thread.CurrentThread.CurrentCulture = cultureInfo;
        }

        public void ChangeVerboseState()
        {
            _logger.Verbose = !_logger.Verbose;
        }

        public Dictionary<string, IFunction> GetHelp()
        {
            Dictionary<string, IFunction> functionDict = new();

            foreach (String functionName in State.FunctionsNamesList)
            {
                functionDict.Add(functionName, GetFunction(functionName));
            }
            return functionDict;
        }

        public string ProcessExpression(String Infixexpression)
        {
            _logger.StartWatcher();
            List<String> cleanedInfixExpression = CleanInfix(Infixexpression);
            _logger.ConsoleDisplayTaskEnd("[Clean] ", cleanedInfixExpression);

            _logger.StartWatcher();
            List<String> postfixExpression = ConvertToPostfix(cleanedInfixExpression);
            _logger.ConsoleDisplayTaskEnd("[Postfix] ", postfixExpression);

            _logger.StartWatcher();
            _logger.Log("[Calculation] ");
            string result = EvaluatePostfixExpression(postfixExpression);
            _logger.ConsoleDisplayTaskEnd("[Calculate] ");

            _logger.ConsoleLogTotalDuration();

            State.AddToHistory(string.Join("", cleanedInfixExpression), result);
            return result;
        }

        public List<String> CleanInfix(String expression)
        {
            List<String> cleanedInfixExpression = new();
            List<String> finalInfixExpression = new();
            String isFunctionMemory = "";
            String isInfixOperatorMemory = "";

            TokenTypes last = TokenTypes.None;
            TokenTypes last2 = TokenTypes.None;
            bool space = false;

            char[] characters = expression.ToCharArray();

            foreach (char token in characters)
            {
                if (State.InfixOperators.Contains(token.ToString())) // OPERATOR
                {
                    isInfixOperatorMemory += token.ToString();
                }
                else if (isInfixOperatorMemory.Length != 0)  // IS END OPERATOR ?
                {
                    if (!State.InfixOperators.Contains(isInfixOperatorMemory))
                    {
                        throw new Exception("This opertator does not exist.");
                    }
                    if (isInfixOperatorMemory == "-" && last != TokenTypes.Number && last != TokenTypes.RightParenthesis)  // NEGATIVE NUMBER
                    {
                        cleanedInfixExpression.Add("0");
                        cleanedInfixExpression.Add(isInfixOperatorMemory);
                    }
                    else
                    {
                        cleanedInfixExpression.Add(isInfixOperatorMemory);
                    }
                    isInfixOperatorMemory = "";
                    last = TokenTypes.Operator;
                }

                if ((97 <= (int)token && (int)token <= 122) || (65 <= (int)token && (int)token <= 90))  // Is a character -> variable or fcn ?
                {
                    if (last != TokenTypes.Operator && (last == TokenTypes.Number || last == TokenTypes.RightParenthesis || (last == TokenTypes.Character && space == true))) // no operator -> implicit product
                    {
                        cleanedInfixExpression.Add("*");
                        last = TokenTypes.Operator;
                    }
                    if (last == TokenTypes.Character)
                    {
                        cleanedInfixExpression[^1] += token.ToString();
                    }
                    else
                    {
                        cleanedInfixExpression.Add(token.ToString());
                    }
                    isFunctionMemory += (token);
                    if (IsFunctionCall(isFunctionMemory) != "None")
                    {
                        last = TokenTypes.Function;
                        isFunctionMemory = "";
                    }
                    else
                    {
                        last = TokenTypes.Character;
                    }
                }
                else
                {
                    if (isFunctionMemory.Length != 0)
                    {
                        isFunctionMemory = "";
                    }
                }

                if (token == ',') { cleanedInfixExpression.Add(token.ToString()); last = TokenTypes.Comma; }

                if (token == '(')
                {
                    if (last == TokenTypes.Function)
                    {
                        cleanedInfixExpression.Add("[");
                    }
                    else if (last != TokenTypes.Operator && space == false && last != TokenTypes.Comma && last != TokenTypes.LeftParenthesis && last != TokenTypes.None) // Implicit product
                    {
                        cleanedInfixExpression.Add("*");
                        cleanedInfixExpression.Add(token.ToString());
                    }
                    else
                    {
                        cleanedInfixExpression.Add(token.ToString());
                    }
                    last = TokenTypes.LeftParenthesis;
                }
                else if (token == ')')
                {
                    if (AreParenthesisMatching(string.Join("", cleanedInfixExpression)))
                    {
                        cleanedInfixExpression.Add("]");
                    }
                    else
                    {
                        cleanedInfixExpression.Add(token.ToString());
                    }
                    last = TokenTypes.RightParenthesis;
                }
                else if (".0123456789".Contains(token))  // DIGIT
                {
                    if (last == TokenTypes.Number) // >9 number
                        cleanedInfixExpression[^1] += token.ToString();
                    else
                    {
                        if (last != TokenTypes.Number && last == TokenTypes.RightParenthesis) // Implicit product
                        {
                            cleanedInfixExpression.Add("*");
                        }
                        cleanedInfixExpression.Add(token.ToString());
                        last = TokenTypes.Number;
                    }
                }

                space = (token == ' ');
                last2 = last;
            }

            if (isInfixOperatorMemory.Length != 0)
            {
                Logger.DebugLog(isInfixOperatorMemory);
                if (!State.InfixOperators.Contains(isInfixOperatorMemory))
                {
                    throw new Exception("This opertator does not exist.");
                }
                if (isInfixOperatorMemory == "-" && last != TokenTypes.Number && last != TokenTypes.RightParenthesis)  // NEGATIVE NUMBER
                {
                    cleanedInfixExpression.Add("0");
                    cleanedInfixExpression.Add(isInfixOperatorMemory);
                }
                else
                {
                    cleanedInfixExpression.Add(isInfixOperatorMemory);
                }
                isInfixOperatorMemory = "";
                last = TokenTypes.Operator;
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
                            buffer += token;

                            continue;

                        case "]":
                            countClose++;
                            buffer += token;
                            if (countOpen == countClose && countOpen != 0)
                            {
                                finalInfixExpression.Add(buffer);
                                countOpen = 0;
                                countClose = 0;
                                buffer = "";
                            }

                            continue;

                        default:
                            if ((IsFunctionCall(token) != "None" && !State.InfixOperators.Contains(token)) || countOpen != countClose)
                            {
                                buffer += token;
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

            return VerifyCleanedExpression(expression, finalInfixExpression);
        }

        private bool CanOpenParenthesis(String lastToken)
        {
            if (" ,[()0123456789".Contains(lastToken[^1]) || State.InfixOperators.Contains(lastToken[^1].ToString()))
            {
                return true;
            }
            return false;
        }

        private static bool AreParenthesisMatching(String lastToken, string type = "()")
        {
            char open = type[0];
            char close = type[1];

            int countOpen = 0;
            int countClose = 0;
            foreach (char ch in lastToken)
            {
                if (ch == open)
                {
                    countOpen++;
                }

                if (ch == close)
                {
                    countClose++;
                }
            }
            return ((countOpen == countClose && countOpen + countClose != 0) || countOpen + countClose == 0);
        }

        private List<String> VerifyCleanedExpression(string originalExpression, List<string> cleanedExpression)
        {
            string expression = string.Join("  ", cleanedExpression.ToArray());
            if (expression.Contains(" = "))  // ILLEGAL NAMES FOR VARS
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

                foreach (string ope in State.InfixOperators)  // I could simplify all of this
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

            if (!AreParenthesisMatching(originalExpression, "()"))
            {
                throw new Exception("Parenthesis are not matching");
            }

            return cleanedExpression;
        }

        public List<String> ConvertToPostfix(List<String> expression)
        {
            Stack operatorStack = new();
            List<String> output = new();
            expression.Add("");  // for checking if token is var to get or to assign...
            int index = -1;

            foreach (string token in expression)
            {
                index++;

                if (token == "")
                {
                    continue;
                }
                else if (token.Contains('['))
                {
                    String function_name = token.Split('[')[0];

                    String correctFunctionName = IsFunctionCall(function_name);
                    if (correctFunctionName != "None")
                    {
                        IFunction function = GetFunction(correctFunctionName);

                        String temp = token[(token.IndexOf('[') + 1)..];
                        temp = temp.Remove(temp.Length - 1);

                        List<String> args = new();

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

                                        buffer += ch;

                                        continue;

                                    case ']':
                                        countClose++;
                                        buffer += ch;

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
                                            buffer += ch;
                                        }

                                        continue;

                                    default:

                                        buffer += ch;
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
                else if (double.TryParse(token, out double temp_parse_double))
                {
                    output.Add(temp_parse_double.ToString());
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
                        output.Add(IsFunctionCall(popped));
                        popped = operatorStack.Pop().ToString();
                    }
                }
                else if (State.InfixOperators.Contains(token))
                {
                    while (operatorStack.Count != 0 && State.OperatorPriorities[(string)operatorStack.Peek()] >= State.OperatorPriorities[token])
                    {
                        output.Add(IsFunctionCall((string)operatorStack.Pop()));
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
                else if (IsVariableCall(token) && expression[index + 1] != "=")
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
                output.Add(IsFunctionCall((string)operatorStack.Pop()));
            }

            return output;
        }

        public string EvaluatePostfixExpression(List<String> PostfixExpression)
        {
            string result;

            Stack operands = new();

            foreach (String ch in PostfixExpression)
            {
                if (Double.TryParse(ch, out double temp))
                {
                    operands.Push(ch);
                }
                else if (ch == "true" || ch == "false")
                {
                    return ch;
                }
                else if (IsFunctionCall(ch) != "None")
                {
                    IFunction function = GetFunction(ch);

                    int argumentsCount = function.ArgumentsCount;
                    List<string> args = new();
                    for (int i = 0; i < argumentsCount; i++)
                    {
                        var popped = operands.Pop();
                        args.Add(popped.ToString());
                    }

                    result = function.ExecuteFunction(State, args);

                    _logger.ConsoleLogCalculation(ch, args, result);
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

        private bool IsVariableCall(String token)
        {
            return (State.VarManager.UserVars.ContainsKey(token) || State.VarManager.Constants.ContainsKey(token)) && !(token == "True") && !(token == "False");
        }

        private String IsFunctionCall(String token)
        {
            if (State.FunctionsNamesList.Contains(token))
            {
                return token;
            }

            foreach (String functionName in State.FunctionsNamesList)
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