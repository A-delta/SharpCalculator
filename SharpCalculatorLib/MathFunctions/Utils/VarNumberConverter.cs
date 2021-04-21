using System;
using System.Collections.Generic;

namespace SharpCalculatorLib.MathFunctions
{
    public static class VarNumberConverter
    {
        public static double GetNumber(State state, string token)
        {
            if (Double.TryParse(token, out double result))
            {
                return result;
            }

            if (token == "True" || token == "False")
            {
                Logger.InfoLog($"Boolean variable was replaced by {(bool.Parse(token) ? 1 : 0)}");
                return (bool.Parse(token) ? 1 : 0);
            }

            Get varGet = new();
            List<string> argsForm = new();
            argsForm.Add(token);
            string value = varGet.ExecuteFunction(state, argsForm);
            return GetNumber(state, value);
        }
    }
}