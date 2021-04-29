using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class Fractional : IFunction
    {
        private String _docstring = "Returns the fraction representation of a value";

        public String Docstring
        {
            get => _docstring;
        }

        private int _argumentsCount = 1;

        public int ArgumentsCount
        {
            get => _argumentsCount;
        }

        private String _infixOperator = "None";

        private String _postfixOperator = "None";

        public String PostfixOperator
        {
            get => _postfixOperator;
        }

        public String InfixOperator
        {
            get => _infixOperator;
        }

        private int _OperatorPriority = 1;

        public int OperatorPriority
        {
            get => _OperatorPriority;
        }

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("frac");
            _aliases.Add("fraction");
            return _aliases;
        }

        public Fraction ExecuteFunction(State state, List<Fraction> args)
        {
            Fraction frac = args[0];
            double nb = args[0].RoundedValue;
            string nbString = nb.ToString();

            if (!nbString.Contains("."))
            {
                return frac;
            }

            frac = new Fraction(nb, 1);
            return frac;
        }
    }
}