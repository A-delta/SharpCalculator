using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class Min : IFunction
    {
        private int _argumentsCount = 2;
        public int ArgumentsCount
        {
            get => _argumentsCount;

        }

        private String _infixOperator = "None";
        public String InfixOperator
        {
            get => _infixOperator;

        }

        private int _infixOperatorPriority = 0;
        public int InfixOperatorPriority
        {
            get => _infixOperatorPriority;

        }



        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("min");
            return _aliases;
        }

        public double ExecuteFunction(List<Double> args)
        {
            return Math.Min(args[0], args[1]);
        }

    }

}