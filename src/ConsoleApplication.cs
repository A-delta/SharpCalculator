using DieseCalcLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace DieseCalcCLI
{
    public class ConsoleApplication
    {
        private bool _verbose;
        private readonly Calculator calc;

        private string appDocsUrl = "https://github.com/DieseCalc/DieseCalcCLI/blob/master/docs/docs.md";

        public ConsoleApplication(string[] args)
        {
            calc = new Calculator(false);
            ProcessCLIArguments(args);

            Version Appversion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            Version libVersion = System.Reflection.Assembly.Load("DieseCalcLib").GetName().Version;

            Console.WriteLine($"DièseCalcCLI {Appversion.Major}.{Appversion.Minor}.{Appversion.Build}");
            Console.WriteLine($"Using DièseCalcLib {libVersion.Major}.{libVersion.Minor}.{libVersion.Build}\n");

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
                    try
                    {
                        ProcessExpression(input);
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (_verbose)
                        {
                            Console.Write($"[DEBUG ERROR] {e}\n");
                        }
                        else
                        {
                            Console.Write($"[ERROR] {e.Message}\n");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
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

                case "math":
                case "helpmath":
                    PrintMathHelp();
                    break;

                case "help":
                case "cmds":
                case "commands":
                    Console.Write("\nApp documentation : ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(appDocsUrl + "\n");
                    Console.ForegroundColor = ConsoleColor.Cyan;
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
            _verbose = false;
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
                            PrintMathHelp();
                            break;

                        case "-v":
                        case "--verbose":
                            _verbose = !_verbose;
                            keepOpen = true;
                            calc.ChangeVerboseState();
                            break;

                        case "-f":
                        case "--fraction":
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
            if (_verbose == false) { Console.Clear(); }
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

        private void PrintMathHelp() // DIRTY
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
                Console.WriteLine($"{item.Key} = {item.Value.ToString()}");
            }
        }

        private void PrintHistory()
        {
            int i = 0;
            foreach (List<string> item in calc.State.History)
            {
                DateTime time = calc.State.HistoryTimes[i];
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"[{time.Hour}:{time.Minute}:{time.Second}] \t ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{item[0]}");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" = ");
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write($"{item[1]}\n");

                i++;
            }
        }
    }
}