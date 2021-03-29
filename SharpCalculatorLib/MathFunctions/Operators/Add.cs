using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public class Add : IFunction
    {
        private String _docstring = "Returns the sum of two numbers";
        public String Docstring
        {
            get => _docstring;

        }

        private int _argumentsCount = 2;
        public int ArgumentsCount
        {
            get => _argumentsCount;

        }



       private String _infixOperator = "+";
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
            _aliases.Add("add");
            _aliases.Add("+");
            return _aliases;
        }

        public string ExecuteFunction(List<Double> args)
        {
            return (args[0] + args[1]).ToString();
        }

    }

}