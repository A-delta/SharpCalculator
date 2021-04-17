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

        private String _infixOperator = "=";

        public String InfixOperator
        {
            get => _infixOperator;
        }

        private int _infixOperatorPriority = 1;

        public int InfixOperatorPriority
        {
            get => _infixOperatorPriority;
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
            string value = args[0];

            state.SetNewVariable(name, value);
            return args[0];
        }
    }
}