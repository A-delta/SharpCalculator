using System;
using System.Collections.Generic;

namespace SharpCalculatorLib
{
    public class State
    {
        public List<String> FunctionsNamesList;
        public List<String> InfixOperators;
        public List<string> PostfixOperators;
        public Dictionary<String, int> OperatorPriorities;

        public struct VarManagerStruct
        {
            public Dictionary<string, string> UserVars;
            public Dictionary<string, string> Constants;
        }

        public Dictionary<DateTime, List<string>> History;

        public VarManagerStruct VarManager = new();

        public State()
        {
            FunctionsNamesList = Calculator.GetAllFunctions();
            InfixOperators = new();
            OperatorPriorities = new();
            PostfixOperators = new();
            History = new();

            foreach (String functionName in FunctionsNamesList) //catching math functions
            {
                IFunction function = Calculator.GetFunction(functionName);
                String infixOperator = function.InfixOperator;
                string postfixOperator = function.PostfixOperator;

                if (infixOperator != "None")
                {
                    InfixOperators.Add(infixOperator);
                    OperatorPriorities.Add(infixOperator, function.OperatorPriority);
                }
                if (postfixOperator != "None")
                {
                    PostfixOperators.Add(postfixOperator);
                    OperatorPriorities.Add(postfixOperator, function.OperatorPriority);
                }
            }

            OperatorPriorities.Add("(", 1);

            VarManager.UserVars = new();
            VarManager.Constants = setCommonVariables();
        }

        private Dictionary<string, string> setCommonVariables()
        {
            Dictionary<string, string> variables = new();
            variables.Add("pi", "3.14159265358979323846264338327950288419716939937510");
            variables.Add("e", "2.718281828459045235360287471352662497757247093699");

            return variables;
        }

        public void SetNewVariable(string name, string value)
        {
            if (VarManager.Constants.ContainsKey(name))
            {
                throw new ArgumentException($"This variable is already declared as a constant. {name} = {VarManager.Constants[name]}");
            }
            if (VarManager.UserVars.ContainsKey(name))
            {
                VarManager.UserVars.Remove(name);
            }

            VarManager.UserVars.Add(name, value);
        }

        public void AddToHistory(string input, string result)
        {
            List<string> toSave = new();
            toSave.Add(input);
            toSave.Add(result);
            History.Add(DateTime.Now, toSave);
        }
    }
}