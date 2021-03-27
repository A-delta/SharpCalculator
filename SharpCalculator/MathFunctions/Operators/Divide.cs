using System;
using System.Collections.Generic;

namespace SharpCalculator.MathFunctions
{
    public class Divide : IFunction
    {
        private int _argumentsCount = 2;
        public int ArgumentsCount
        {
            get => _argumentsCount;

        }

        private String _infixOperator = "/";
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
            _aliases.Add("divide");
            _aliases.Add("/");
            return _aliases;
        }

        public double ExecuteFunction(List<Double> args)
        {
            return args[1] / args[0];
        }

    }

}