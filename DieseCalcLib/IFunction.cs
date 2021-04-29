using System;
using System.Collections.Generic;

namespace DieseCalcLib
{
    public interface IFunction
    {
        String Docstring { get; }
        int ArgumentsCount { get; }

        String PostfixOperator { get; }
        String InfixOperator { get; }
        int OperatorPriority { get; }

        public Fraction ExecuteFunction(State state, List<Fraction> args);

        public List<String> getAliases();
    }
}