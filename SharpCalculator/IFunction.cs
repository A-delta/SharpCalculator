using System;
using System.Collections.Generic;

namespace SharpCalculator
{
    interface IFunction
    {
        int ArgumentsCount { get; }

        public double ExecuteFunction(List<Double> args);

        public List<String> getAliases();
    }
}
