using System;
using System.Collections.Generic;

namespace SharpCalculatorLib
{
    public interface IFunction
    {
        String Docstring { get; }
        int ArgumentsCount { get; }
        String InfixOperator { get; }
        int InfixOperatorPriority { get; }

        public string ExecuteFunction(State state, List<string> args);

        public List<String> getAliases();
    }
}