using System;
using System.Collections.Generic;

namespace SharpCalculator
{
    interface IFunction
    {
        int ArgumentsCount { get; }
        String InfixOperator { get; }
        int InfixOperatorPriority { get; }

        public double ExecuteFunction(List<Double> args);

        public List<String> getAliases();
    }
}
