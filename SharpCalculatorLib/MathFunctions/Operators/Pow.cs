using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class Pow : IFunction
    {
        private String _docstring = "Returns the a^b";

        public String Docstring
        {
            get => _docstring;
        }

        private int _argumentsCount = 2;

        public int ArgumentsCount
        {
            get => _argumentsCount;
        }

        private String _infixOperator = "^";

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
            _aliases.Add("power");
            _aliases.Add("pow");
            _aliases.Add("^");
            return _aliases;
        }

        public Fraction ExecuteFunction(State state, List<string> args)
        {
            Fraction arg1 = Fraction.Parse(args[0]);
            Fraction arg2 = Fraction.Parse(args[1]);

            return (Fraction.Pow(arg1, arg2));
        }
    }
}