using System;
using System.Collections.Generic;

namespace SharpCalculator.MathFunctions
{
    public class SquareRoot : IFunction
    {
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

        private int _infixOperatorPriority = 4;
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

        public double ExecuteFunction(List<Double> args)
        {
            return Math.Sqrt(args[0]);
        }

    } 

}
