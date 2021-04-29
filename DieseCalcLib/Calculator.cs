using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using DieseCalcLib.Exceptions;

namespace DieseCalcLib
{
    public class Calculator
    {
        private readonly Logger _logger;
        private bool outputFrac;
        public State State;

        private enum TokenTypes
        {
            None,
            Character,
            Function,
            Comma,
            Number,
            Operator,
            PostFixOperator,
            RightParenthesis,
            LeftParenthesis
        }

        public Calculator(bool verbose)
        {
            _logger = new Logger(verbose);
            State = new State();

            outputFrac = true;

            CultureInfo cultureInfo = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            cultureInfo.NumberFormat.NegativeInfinitySymbol = "-Infinity"; // some system printed "8" instead of +oo
            cultureInfo.NumberFormat.PositiveInfinitySymbol = "+Infinity";
            Thread.CurrentThread.CurrentCulture = cultureInfo;
        }

        public void ChangeVerboseState()
        {
            _logger.Verbose = !_logger.Verbose;
        }

        public void ChangeOutputType()
        {
            outputFrac = !outputFrac;
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

            bool isInfixOperator = false;
            String isFunctionMemory = "";
            String isInfixOperatorMemory = "";

            TokenTypes last = TokenTypes.None;
            TokenTypes last2 = TokenTypes.None;
            bool needClose = false;
            bool space = false;

            char[] characters = expression.ToCharArray();

            foreach (char token in characters)
            {
                foreach (string ope in State.InfixOperators)
                {
                    if (ope.Contains(token) || ope == token.ToString())
                    {
                        isInfixOperatorMemory += token.ToString();
                        isInfixOperator = true;
                        break;
                    }
                }

                if (!isInfixOperator && isInfixOperatorMemory.Length != 0)  // IS END OPERATOR ?
                {
                    if (!State.InfixOperators.Contains(isInfixOperatorMemory) && !State.PostfixOperators.Contains(isInfixOperatorMemory))
                    {
                        //Logger.DebugLog(isInfixOperatorMemory);
                        throw new UnknownOperatorException("This operator does not exist");
                    }
                    if (isInfixOperatorMemory == "-" && last != TokenTypes.Number && last != TokenTypes.RightParenthesis && last != TokenTypes.Character)  // NEGATIVE NUMBER
                    {
                        if (last == TokenTypes.Operator)
                        {
                            cleanedInfixExpression.Add("(");
                            cleanedInfixExpression.Add("0");
                            cleanedInfixExpression.Add(isInfixOperatorMemory);
                            needClose = true;
                        }
                        else
                        {
                            cleanedInfixExpression.Add("0");
                            cleanedInfixExpression.Add(isInfixOperatorMemory);
                        }
                    }
                    else
                    {
                        cleanedInfixExpression.Add(isInfixOperatorMemory);
                    }

                    last = (State.PostfixOperators.Contains(isInfixOperatorMemory) ? TokenTypes.PostFixOperator : TokenTypes.Operator);
                    isInfixOperatorMemory = "";
                }

                if ((97 <= (int)token && (int)token <= 122) || (65 <= (int)token && (int)token <= 90))  // Is a character -> variable or fcn ?
                {
                    if (last != TokenTypes.Operator && (last == TokenTypes.PostFixOperator || last == TokenTypes.Number || last == TokenTypes.RightParenthesis || (last == TokenTypes.Character && space == true))) // no operator -> implicit product
                    {
                        cleanedInfixExpression.Add("*");
                        last = TokenTypes.Operator;
                    }
                    if (last == TokenTypes.Character || last == TokenTypes.Function)
                    {
                        isFunctionMemory = cleanedInfixExpression[^1];
                        cleanedInfixExpression[^1] += token.ToString();
                    }
                    else if (last != TokenTypes.Function)
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
                    for (int i = 1; i <= cleanedInfixExpression.Count; i++)
                    {
                        if (cleanedInfixExpression[^i] == "(" && !AreParenthesisMatching(string.Join("", cleanedInfixExpression)))
                        {
                            cleanedInfixExpression.Add(token.ToString());
                            break;
                        }
                        else if (cleanedInfixExpression[^i] == "[" && !AreParenthesisMatching(string.Join("", cleanedInfixExpression), "[]"))
                        {
                            cleanedInfixExpression.Add("]");
                            break;
                        }
                    }
                    last = TokenTypes.RightParenthesis;
                }
                else if (".0123456789".Contains(token))  // DIGIT
                {
                    if (last == TokenTypes.Number) // >9 number
                        cleanedInfixExpression[^1] += token.ToString();
                    else
                    {
                        if (last != TokenTypes.Number && (last == TokenTypes.RightParenthesis || last == TokenTypes.PostFixOperator)) // Implicit product
                        {
                            cleanedInfixExpression.Add("*");
                        }
                        cleanedInfixExpression.Add(token.ToString());
                        last = TokenTypes.Number;
                    }
                }

                if (needClose && last != TokenTypes.Character && IsVariableCall(cleanedInfixExpression[^2]))
                {
                    cleanedInfixExpression.Insert(cleanedInfixExpression.Count - 1, ")");
                    needClose = false;
                }

                isInfixOperator = false;
                space = (token == ' ');
                last2 = last;
            }

            if (needClose && last == TokenTypes.Character && IsVariableCall(cleanedInfixExpression[^1]))
            {
                cleanedInfixExpression.Add(")");
                needClose = false;
            }

            if (isInfixOperatorMemory.Length != 0)
            {
                if (!State.InfixOperators.Contains(isInfixOperatorMemory) && !State.PostfixOperators.Contains(isInfixOperatorMemory))
                {
                    throw new UnknownOperatorException("This operator does not exist.");
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
                last = (State.PostfixOperators.Contains(isInfixOperatorMemory) ? TokenTypes.PostFixOperator : TokenTypes.Operator);
                isInfixOperatorMemory = "";
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
                    throw new IllegalVariableNameException($"Variable names can not contain number, operators or bool value.");
                }

                foreach (string ope in State.InfixOperators)  // I could simplify all of this
                {
                    if (varName.Contains(ope))
                    {
                        throw new IllegalVariableNameException($"Variable names can not contain number, operators or bool value.");
                    }
                }

                foreach (char nb in "0.123456789")
                {
                    if (varName.Contains(nb))
                    {
                        throw new IllegalVariableNameException($"Variable names can not contain number, operators or bool value.");
                    }
                }
            }

            if (!AreParenthesisMatching(originalExpression, "()"))
            {
                throw new ParenthesisNotMatchingException("Parenthesis are not matching");
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
                else if (State.InfixOperators.Contains(token) || State.PostfixOperators.Contains(token))
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
                }
                else if (expression[index + 1] == "=")
                {
                    output.Add(token);
                }
                else
                {
                    output.Add(token);
                }
            }

            while (operatorStack.Count != 0)
            {
                output.Add(IsFunctionCall((string)operatorStack.Pop()));
            }

            if (!outputFrac) { output.Add("Decimal"); }

            return output;
        }

        public string EvaluatePostfixExpression(List<String> PostfixExpression)
        {
            Fraction result;
            Stack operands = new();

            foreach (String ch in PostfixExpression)
            {
                if (IsFunctionCall(ch) != "None")
                {
                    IFunction function = GetFunction(ch);

                    int argumentsCount = function.ArgumentsCount;
                    List<Fraction> args = new();
                    for (int i = 0; i < argumentsCount; i++)
                    {
                        Fraction popped = Fraction.Parse(State, operands.Pop().ToString());
                        args.Add(popped);
                    }

                    result = function.ExecuteFunction(State, args);

                    if (result.exact)
                    {
                        operands.Push(result.RoundedValue);
                        _logger.ConsoleLogCalculation(ch, args, result.RoundedValue.ToString());
                    }
                    else
                    {
                        operands.Push(result);
                        _logger.ConsoleLogCalculation(ch, args, result.ToString());
                    }
                }
                else
                {
                    operands.Push(ch);
                }
            }
            var poppedRes = operands.Pop();

            result = Fraction.Parse(State, poppedRes.ToString());

            if (operands.Count != 0)
            {
                throw new Exception();
            }
            return result.ToString();
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

        public static bool IsVariableCall(string token, State state)
        {
            return (state.VarManager.UserVars.ContainsKey(token) || state.VarManager.Constants.ContainsKey(token)) && !(token == "True") && !(token == "False");
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
            return (IFunction)System.Activator.CreateInstance(Type.GetType("DieseCalcLib.MathFunctions." + functionName));
        }

        ~Calculator()
        {
            Console.WriteLine("Calculator deleted");
        }
    }
}