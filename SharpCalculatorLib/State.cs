using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCalculatorLib
{
    
    public class State
    {
        public List<String> FunctionsNamesList;
        public List<String> InfixOperators;
        public Dictionary<String, int> OperatorPriorities;
        public Dictionary<String, string> VarManager;

        public Dictionary<DateTime, Dictionary<string, string>> History;

        public State()
        {
            FunctionsNamesList = Calculator.GetAllFunctions();
            InfixOperators = new List<string>();
            OperatorPriorities = new Dictionary<String, int>();
            History = new Dictionary<DateTime, Dictionary<string, string>>();

            foreach (String functionName in FunctionsNamesList)
            {
                IFunction function = Calculator.GetFunction(functionName);
                String infixOperator = function.InfixOperator;

                if (infixOperator != "None")
                {
                    InfixOperators.Add(infixOperator);
                    OperatorPriorities.Add(infixOperator, function.InfixOperatorPriority);

                }
            }
            OperatorPriorities.Add("(", 1);
            VarManager = new Dictionary<string, string>();
        }

        public void SetNewVariable(string name, string value)
        {
            if (VarManager.ContainsKey(name))
            {
                VarManager.Remove(name);
            }

            VarManager.Add(name, value);
        }
    }
}
