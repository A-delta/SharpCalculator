using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class SquareRoot : IFunction
    {
        private String _docstring = "Returns the squareroot of a number";

        public String Docstring
        {
            get => _docstring;
        }

        private int _argumentsCount = 1;

        public int ArgumentsCount
        {
            get => _argumentsCount;
        }

        private String _infixOperator = "√";

        public String InfixOperator
        {
            get => _infixOperator;
        }

        private int _infixOperatorPriority = 5;

        public int InfixOperatorPriority
        {
            get => _infixOperatorPriority;
        }

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("sqrt");
            return _aliases;
        }

        public string ExecuteFunction(State state, List<string> args)
        {
            double arg1 = VarNumberConverter.GetNumber(state, args[0]);

            return Math.Sqrt(arg1).ToString();
        }
    }
}