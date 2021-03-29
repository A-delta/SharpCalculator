using System;
using System.Collections.Generic;

namespace SharpCalculatorLib
{
    interface IFunction
    {
        String Docstring { get; }
        int ArgumentsCount { get; }
        String InfixOperator { get; }
        int InfixOperatorPriority { get; }

        public double ExecuteFunction(List<Double> args);

        public List<String> getAliases();
    }
}
