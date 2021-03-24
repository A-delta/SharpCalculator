using System;
using System.Collections.Generic;

namespace SharpCalculator.MathFunctions
{
    public class Max : IFunction
    {
        private int _argumentsCount = 2;
        public int ArgumentsCount
        {
            get => _argumentsCount;

            set => _argumentsCount = value;
        }



        public double ExecuteFunction(List<Double> args)
        {
            return Math.Max(args[0], args[1]);
        }

    } 

}
