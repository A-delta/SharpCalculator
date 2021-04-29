using System;
using System.Collections.Generic;

namespace DieseCalcLib.MathFunctions
{
    public class Get
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

        private int _OperatorPriority = 1;

        public int OperatorPriority
        {
            get => _OperatorPriority;
        }

        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("get");
            return _aliases;
        }

        public Fraction ExecuteFunction(State state, string varName)
        {
            if (!state.VarManager.UserVars.ContainsKey(varName))
            {
                if (!state.VarManager.Constants.ContainsKey(varName))
                {
                    throw new ArgumentException("This variable isn't initialised");
                }
                else
                {
                    return Fraction.Parse(state, state.VarManager.Constants[varName]);
                }
            }
            else
            {
                return Fraction.Parse(state, state.VarManager.UserVars[varName]);
            }
        }
    }
}