using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class Get : IFunction
    {
        private String _docstring = "Used to get a variable's value";
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
            _aliases.Add("get");
            return _aliases;
        }

        public string ExecuteFunction(State state, List<string> args)
        {
            string name = args[0];
            if (!state.VarManager.ContainsKey(name))
            {
                throw new ArgumentException("This variable isn't initialised");
            }
            return state.VarManager[name];
        }

    }

}