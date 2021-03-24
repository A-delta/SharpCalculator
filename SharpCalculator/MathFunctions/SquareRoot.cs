using System;
using System.Collections.Generic;

namespace SharpCalculator.MathFunctions
{
    public class SquareRoot : IFunction
    {
        private int _argumentsCount = 1;
        public int ArgumentsCount
        {
            get => _argumentsCount;

            set => _argumentsCount = value;
        }


        public double ExecuteFunction(List<Double> args)
        {
            return Math.Sqrt(args[0]);
        }

    } 

}
