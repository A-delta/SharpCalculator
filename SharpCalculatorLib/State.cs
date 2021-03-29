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
        public List<String> InfixOperators = new List<string>();
        public Dictionary<String, int> OperatorPriorities = new Dictionary<String, int>();

        public Dictionary<String, string> VarManager;


        public State()
        {
            FunctionsNamesList = Calculator.GetAllFunctions();
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
            VarManager.Add(name, value);
        }
    }
}
