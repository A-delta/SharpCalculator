using SharpCalculatorLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpCalculatorApp
{
    public class ConsoleApplication
    {
        private bool _verbose;
        private readonly Calculator calc;

        public ConsoleApplication(string[] args)
        {
            calc = new Calculator(false);
            ProcessCLIArguments(args);

            Version Appversion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            Version libVersion = System.Reflection.Assembly.Load("SharpCalculatorLib").GetName().Version;

            Console.WriteLine($"SharpCalculatorCLI {Appversion.Major}.{Appversion.Minor}.{Appversion.Build}");
            Console.WriteLine($"Using SharpCalculatorLib {libVersion.Major}.{libVersion.Minor}.{libVersion.Build}\n");

            if (!_verbose) { Console.WriteLine("Enter 'verbose' to see details in operations\n"); }
        }

        public void Run()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("$  ");
                Console.ForegroundColor = ConsoleColor.White;
                String input = Console.ReadLine();
                if (input.Length != 0)
                {
                    ProcessExpression(input);
                    //}
                    //try
                    //{
                    //    ProcessExpression(input);
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.ForegroundColor = ConsoleColor.Red;
                    //    Console.Write($"[ERROR] {e.Message}\n");
                    //    Console.ForegroundColor = ConsoleColor.White;
                    //}
                }
            }
        }

        private void ProcessExpression(string expression)
        {
            switch (expression)
            {
                case "verbose":
                    _verbose = !_verbose;
                    calc.ChangeVerboseState();
                    Console.WriteLine("Changed verbose state");
                    break;

                case "<":
                case "frac":
                case "dec":
                    calc.ChangeOutputType();
                    Console.WriteLine("Changed output type");
                    break;

                case "history":
                case "hist":
                    PrintHistory();
                    break;

                case "var":
                case "variables":
                case "ls":
                case "lsvar":
                    PrintUserVariables();
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
                    PrintHelp();
                    break;

                default:
                    string result = calc.ProcessExpression(expression);
                    Console.WriteLine(">>  " + result + "\n");
                    break;
            }
        }

        private void ProcessCLIArguments(string[] args)
        {
            int i = 0;
            bool keepOpen = false;
            string[] expressions;
            if (args.Length != 0)
            {
                foreach (string arg in args)
                {
                    switch (arg)
                    {
                        case "-c":
                        case "--calculate":
                            string result = calc.ProcessExpression(args[i + 1]);
                            Console.WriteLine(result);
                            break;

                        case "-h":
                        case "--help":
                            PrintHelp();
                            break;

                        case "-v":
                        case "--verbose":
                            _verbose = true;
                            keepOpen = true;
                            calc.ChangeVerboseState();
                            break;

                        case "-f":
                        case "--fraction":
                            keepOpen = true;
                            break;

                        case "-d":
                        case "--decimal":
                            calc.ChangeOutputType();
                            keepOpen = true;
                            break;

                        case "-i":
                        case "--input":
                            expressions = LoadFile(args[i + 1]);
                            int lineNumber = 1;
                            Console.WriteLine($"Processing {args[i + 1]} : ");
                            foreach (string expression in expressions)
                            {
                                Console.WriteLine($"{lineNumber}\t{expression} = {calc.ProcessExpression(expression)}");
                                lineNumber++;
                            }
                            break;

                        case "-ri":
                        case "--rawinput":
                            expressions = LoadFile(args[i + 1]);
                            foreach (string expression in expressions)
                            {
                                Console.WriteLine($"{expression} = {calc.ProcessExpression(expression)}");
                            }

                            break;
                    }
                    i++;
                }
                if (keepOpen == false) { System.Environment.Exit(1); }
            }
        }

        private static string[] LoadFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                return System.IO.File.ReadAllLines(fileName);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public static List<string> GetMathFunctions()
        {
            return Calculator.GetAllFunctions();
        }

        private void PrintHelp() // DIRTY
        {
            Dictionary<string, IFunction> functionList = calc.GetHelp();

            string columnsName = "Name";
            for (int i = 0; i < 20 - 4; i++) { columnsName += " "; }
            columnsName += "Aliases";
            for (int i = 0; i < 21; i++) { columnsName += " "; }
            columnsName += "Description";

            Console.WriteLine(columnsName + "\n");

            foreach (KeyValuePair<string, IFunction> item in functionList)
            {
                string toPrint = item.Key;
                for (int i = 0; i < (20 - item.Key.Length); i++)
                {
                    toPrint += " ";
                }

                int name = toPrint.Length;

                foreach (string alias in item.Value.getAliases())
                {
                    toPrint += alias + " ; ";
                }

                for (int i = 0; i < (35 - (toPrint.Length - name)); i++)
                {
                    toPrint += " ";
                }

                toPrint += "\t" + item.Value.Docstring + "\n";
                Console.WriteLine(toPrint);
            }
        }

        private void PrintUserVariables()
        {
            foreach (KeyValuePair<string, string> item in calc.State.VarManager.UserVars)
            {
                Console.WriteLine($"{item.Key} = {item.Value}");
            }
        }

        private void PrintHistory()
        {
            Dictionary<DateTime, List<string>> history = calc.State.History;

            foreach (KeyValuePair<DateTime, List<string>> item in history)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"[{item.Key.Hour}:{item.Key.Minute}:{item.Key.Second}] \t ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{item.Value[1]}");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" = ");
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write($"{item.Value[0]}\n");
            }
        }
    }
}