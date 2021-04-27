using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public static class FractionSimplifier
    {
        public static string Simplify(State state, List<string> args)
        {
            string b = VarNumberConverter.GetNumber(state, args[0]).ToString();
            string a = VarNumberConverter.GetNumber(state, args[1]).ToString();

            IFunction pgcd = new PGCD();
            double divider = double.Parse(pgcd.ExecuteFunction(state, args));
            if (divider > 1)
            {
                double top = double.Parse(a) / divider;
                double bot = double.Parse(b) / divider;

                return (bot != 1 ? top + " / " + bot : top.ToString());
            }

            return a + " / " + b;
        }
    }
}