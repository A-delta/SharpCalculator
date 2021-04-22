using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class Increment : IFunction
    {
        private String _docstring = "Add 1 to the variable";

        public String Docstring
        {
            get => _docstring;
        }

        private int _argumentsCount = 1;

        public int ArgumentsCount
        {
            get => _argumentsCount;
        }

        private String _infixOperator = "++";

        public String InfixOperator
        {
            get => _infixOperator;
        }

        private int _infixOperatorPriority = 4;

        public int InfixOperatorPriority
        {
            get => _infixOperatorPriority;
        }

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("increment");
            _aliases.Add("++");
            return _aliases;
        }

        public string ExecuteFunction(State state, List<string> args)
        {
            string name = args[0];
            double value = VarNumberConverter.GetNumber(state, args[0]) + 1;
            state.SetNewVariable(name, value.ToString());
            return (value).ToString();
        }
    }
}