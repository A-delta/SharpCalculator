using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class Power : IFunction
    {
        private String _docstring = "Returns x^y";
        public String Docstring
        {
            get => _docstring;

        }

        private int _argumentsCount = 2;
        public int ArgumentsCount
        {
            get => _argumentsCount;

        }

        private String _infixOperator = "^";
        public String InfixOperator
        {
            get => _infixOperator;

        }

        private int _infixOperatorPriority = 4;
        public int InfixOperatorPriority
        {
            get => _infixOperatorPriority;

        }

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("pow");
            _aliases.Add("^");
            return _aliases;
        }

        public string ExecuteFunction(List<Double> args)
        {
            return Math.Pow(args[1], args[0]).ToString();
        }

    }

}