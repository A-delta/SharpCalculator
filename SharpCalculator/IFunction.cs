using System;
using System.Collections.Generic;
using System.Text;

namespace SharpCalculator
{
    interface IFunction
    {
        int ArgumentsCount { get; set;  }

        public double ExecuteFunction(List<Double> args);
    }
}
