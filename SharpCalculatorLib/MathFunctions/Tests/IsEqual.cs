using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class IsEqual : IFunction
    {
        private String _docstring = "Returns true if arguments are equal";
        public String Docstring
        {
            get => _docstring;

        }

        private int _argumentsCount = 2;
        public int ArgumentsCount
        {
            get => _argumentsCount;

        }



        private String _infixOperator = "==";
        public String InfixOperator
        {
            get => _infixOperator;

        }

        private int _infixOperatorPriority = 2;
        public int InfixOperatorPriority
        {
            get => _infixOperatorPriority;

        }


        private List<String> _aliases = new List<string>();

        public List<String> getAliases()
        {
            _aliases.Add("equal");
            _aliases.Add("==");
            return _aliases;
        }

        public string ExecuteFunction(State state, List<string> args)
        {
            //double arg1 = Double.Parse(args[0]);
            //double arg2 = Double.Parse(args[1]);

            return (args[0] == args[1]).ToString();
        }

    }

}