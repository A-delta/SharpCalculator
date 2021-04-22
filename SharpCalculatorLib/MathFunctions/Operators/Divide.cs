using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class Divide : IFunction
    {
        private String _docstring = "Returns the division of two numbers";

        public String Docstring
        {
            get => _docstring;
        }

        private int _argumentsCount = 2;

        public int ArgumentsCount
        {
            get => _argumentsCount;
        }

        private String _postfixOperator = "None";

        public String PostfixOperator
        {
            get => _postfixOperator;
        }

        private String _infixOperator = "/";

        public String InfixOperator
        {
            get => _infixOperator;
        }

        private int _operatorPriority = 4;

        public int OperatorPriority
        {
            get => _operatorPriority;
        }

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("divide");
            _aliases.Add("/");
            return _aliases;
        }

        public string ExecuteFunction(State state, List<string> args)
        {
            double arg1 = VarNumberConverter.GetNumber(state, args[0]);
            double arg2 = VarNumberConverter.GetNumber(state, args[1]);

            return (arg2 / arg1).ToString();
        }
    }
}