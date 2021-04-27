using System;
using System.Collections.Generic;

namespace SharpCalculatorLib
{
    public interface IFunction
    {
        String Docstring { get; }
        int ArgumentsCount { get; }

        String PostfixOperator { get; }
        String InfixOperator { get; }
        int OperatorPriority { get; }

        public Fraction ExecuteFunction(State state, List<string> args);

        public List<String> getAliases();
    }
}