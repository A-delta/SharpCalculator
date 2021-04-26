using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCalculatorLib.MathFunctions
{
    internal class Cosinus : IFunction

    {
        private String _docstring = "Returns cosinus value of a number";

        public String Docstring
        {
            get => _docstring;
        }

        private int _argumentsCount = 1;

        public int ArgumentsCount
        {
            get => _argumentsCount;
        }

        private String _postfixOperator = "None";

        public String PostfixOperator
        {
            get => _postfixOperator;
        }

        private String _infixOperator = "None";

        public String InfixOperator
        {
            get => _infixOperator;
        }

        private int _OperatorPriority = 0;

        public int OperatorPriority
        {
            get => _OperatorPriority;
        }

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("cos");
            _aliases.Add("cosinus");
            return _aliases;
        }

        public string ExecuteFunction(State state, List<string> args)
        {
            double arg1 = VarNumberConverter.GetNumber(state, args[0]);

            return Math.Cos(arg1).ToString();
        }
    }
}