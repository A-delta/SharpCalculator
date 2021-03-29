using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCalculatorLib
{
    
    class State
    {
        public Dictionary<String, Double> VarManager;


        public State()
        {
            VarManager = new Dictionary<string, double>();

        }

        public void SetNewVariable(string name, double value)
        {
            VarManager.Add(name, value);
        }
    }
}
