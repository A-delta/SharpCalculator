using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    internal class PGCD : IFunction
    {
        private String _docstring = "Returns PGCD of two values";

        public String Docstring
        {
            get => _docstring;
        }

        private int _argumentsCount = 2;

        public int ArgumentsCount
        {
            get => _argumentsCount;
        }

        private String _infixOperator = "None";

        public String InfixOperator
        {
            get => _infixOperator;
        }

        private int _infixOperatorPriority = 0;

        public int InfixOperatorPriority
        {
            get => _infixOperatorPriority;
        }

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("pgcd");
            return _aliases;
        }

        public string ExecuteFunction(State state, List<string> args)
        {
            int a;
            int b;
            try
            {
                a = Math.Abs(int.Parse(args[1]));
                b = Math.Abs(int.Parse(args[0]));
            }
            catch (Exception)
            {
                throw new ArgumentException("Only integers are allowed in arithmetic");
            }

            if (b == 0) { return a.ToString(); }
            if (b == 1) { return "1"; }
            if (a % b == 0) { return b.ToString(); }

            int r = a % b;

            while (a % b != 0)
            {
                r = a % b;
                a = b;
                b = r;
            }
            return r.ToString();
        }
    }
}