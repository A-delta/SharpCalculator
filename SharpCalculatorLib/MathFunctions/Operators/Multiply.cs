using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class Multiply : IFunction
    {
        private String _docstring = "Returns the product of two numbers";
        public String Docstring
        {
            get => _docstring;

        }

        private int _argumentsCount = 2;
        public int ArgumentsCount
        {
            get => _argumentsCount;

        }

        private String _infixOperator = "*";
        public String InfixOperator
        {
            get => _infixOperator;

        }

        private int _infixOperatorPriority = 3;
        public int InfixOperatorPriority
        {
            get => _infixOperatorPriority;

        }

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("multiply");
            _aliases.Add("*");
            return _aliases;
        }

        public string ExecuteFunction(List<Double> args)
        {
            return (args[0] * args[1]).ToString();
        }

    }

}