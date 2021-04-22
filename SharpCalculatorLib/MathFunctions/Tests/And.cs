using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class And : IFunction
    {
        private String _docstring = "Returns True if a and b are true or 1";

        public String Docstring
        {
            get => _docstring;
        }

        private int _argumentsCount = 2;

        public int ArgumentsCount
        {
            get => _argumentsCount;
        }

        private String _infixOperator = "&&";

        public String InfixOperator
        {
            get => _infixOperator;
        }

        private int _infixOperatorPriority = 2;

        public int InfixOperatorPriority
        {
            get => _infixOperatorPriority;
        }

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("and");
            _aliases.Add("&&");
            return _aliases;
        }

        public string ExecuteFunction(State state, List<string> args)
        {
            double arg1 = VarNumberConverter.GetNumber(state, args[0]);
            double arg2 = VarNumberConverter.GetNumber(state, args[1]);

            return (arg1 == 1 && arg2 == 1).ToString();
        }
    }
}