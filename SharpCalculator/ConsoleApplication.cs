using SharpCalculatorLib;
using System;
using System.Collections.Generic;

namespace SharpCalculatorApp
{
    class ConsoleApplication
    {
        Calculator calc;

        public ConsoleApplication(bool verbose=false)
        {
            calc = new Calculator(false);
        }

        public List<string> GetMathFunctions()
        {
            return Calculator.GetAllFunctions();
        }
        public void ProcessExpression(string expression)
        {
            switch (expression) {
                case "verbose":
                    calc.ChangeVerboseState(true);
                    break;

                case "quit":
                case "exit":
                    System.Environment.Exit(1);
                    break;

                case "cls":
                case "clear":
                    Console.Clear();
                    break;

                case "help":
                    _PrintHelp();
                    break;

                default:
                    calc.ProcessExpression(expression);
                    break;
            }
        }

        private void _PrintHelp() // DIRTY
        {
            Dictionary<string, IFunction> functionList = calc.GetHelp();

            string columnsName = "Name";
            for (int i = 0; i < 20-4; i++) { columnsName += " "; }
            columnsName += "Aliases";
            for (int i = 0; i < 21; i++) { columnsName += " "; }
            columnsName += "Description";

            Console.WriteLine(columnsName+"\n");

            foreach (KeyValuePair<string, IFunction> item in functionList)
            {
                string toPrint = item.Key;
                for (int i = 0; i < (20 - item.Key.Length); i++) {
                    toPrint += " ";
                }

                int name = toPrint.Length;

                foreach (string alias in item.Value.getAliases())
                {
                    toPrint += alias + " ; ";
                }

                for (int i = 0; i < (35 - (toPrint.Length-name)); i++)
                {
                    toPrint += " ";
                }

                toPrint += "\t" + item.Value.Docstring + "\n";
                Console.WriteLine(toPrint);
            }
        }

    }
}
