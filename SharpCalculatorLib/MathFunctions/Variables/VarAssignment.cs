using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class VarAssignment : IFunction
    {
        private String _docstring = "Used when creating new variables";

        public String Docstring
        {
            get => _docstring;
        }

        private int _argumentsCount = 2;

        public int ArgumentsCount
        {
            get => _argumentsCount;
        }

        private String _postfixOperator = "None";

        public String PostfixOperator
        {
            get => _postfixOperator;
        }

        private String _infixOperator = "=";

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
            _aliases.Add("var");
            _aliases.Add("=");
            return _aliases;
        }

        public string ExecuteFunction(State state, List<string> args)
        {
            string name = args[1];
            string value = string.Empty;

            if (args[0] == "False" || args[0] == "True")
            {
                value = args[0];
            }
            else
            {
                value = VarNumberConverter.GetNumber(state, args[0]).ToString();
            }

            state.SetNewVariable(name, value);
            return value;
        }
    }
}