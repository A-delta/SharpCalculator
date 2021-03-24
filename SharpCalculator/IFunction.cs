using System;
using System.Collections.Generic;
using System.Text;

namespace SharpCalculator
{
    interface IFunction
    {
        int ArgumentsCount { get; }
        public double ExecuteFunction(List<Double> args);

        public List<String> getAliases();
    }
}
