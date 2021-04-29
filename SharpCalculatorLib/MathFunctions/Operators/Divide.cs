using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class Divide : IFunction
    {
        private String _docstring = "Returns the fraction of two numbers";

        public String Docstring
        {
            get => _docstring;
        }

        private int _argumentsCount = 2;

        public int ArgumentsCount
        {
            get => _argumentsCount;
        }

        private String _infixOperator = "/";

        private String _postfixOperator = "None";

        public String PostfixOperator
        {
            get => _postfixOperator;
        }

        public String InfixOperator
        {
            get => _infixOperator;
        }

        private int _OperatorPriority = 4;

        public int OperatorPriority
        {
            get => _OperatorPriority;
        }

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("divide");
            _aliases.Add("/");
            return _aliases;
        }

        public Fraction ExecuteFunction(State state, List<Fraction> args)
        {
            return (args[1] / args[0]);
        }
    }
}