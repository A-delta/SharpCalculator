using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    internal class Logarithm : IFunction
    {
        private String _docstring = "Returns log of a value with base b";

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

        private String _infixOperator = "None";

        public String InfixOperator
        {
            get => _infixOperator;
        }

        private int _OperatorPriority = 0;

        public int OperatorPriority
        {
            get => _OperatorPriority;
        }

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("logb");
            return _aliases;
        }

        public string ExecuteFunction(State state, List<string> args)
        {
            double arg1 = VarNumberConverter.GetNumber(state, args[1]);
            double arg2 = VarNumberConverter.GetNumber(state, args[0]);

            return Math.Log(arg1, arg2).ToString();
        }
    }
}