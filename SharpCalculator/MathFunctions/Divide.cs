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

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("divide");
            _aliases.Add("/");
            return _aliases;
        }

        public double ExecuteFunction(List<Double> args)
        {
            return args[0] / args[1];
        }

    }

}