using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class Decrement : IFunction
    {
        private String _docstring = "Remove 1 to the variable";

        public String Docstring
        {
            get => _docstring;
        }

        private int _argumentsCount = 1;

        public int ArgumentsCount
        {
            get => _argumentsCount;
        }

        private String _postfixOperator = "--";

        public String PostfixOperator
        {
            get => _postfixOperator;
        }

        private String _infixOperator = "None";

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
            _aliases.Add("decrement");
            _aliases.Add("--");
            return _aliases;
        }

        public string ExecuteFunction(State state, List<string> args)
        {
            string name = args[0];
            double value = VarNumberConverter.GetNumber(state, args[0]) - 1;
            state.SetNewVariable(name, value.ToString());
            return (value).ToString();
        }
    }
}