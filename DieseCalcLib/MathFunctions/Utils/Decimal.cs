using System;
using System.Collections.Generic;

namespace DieseCalcLib.MathFunctions
{
    public class Decimal : IFunction
    {
        private String _docstring = "Returns the decimal value of a fraction";

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
            _aliases.Add("decimal");
            _aliases.Add("dec");
            return _aliases;
        }

        public Fraction ExecuteFunction(State state, List<Fraction> args)
        {
            return Fraction.Parse(state, args[0].RoundedValue.ToString(), true);
            //return new Fraction(args[0].RoundedValue, 1);
        }
    }
}