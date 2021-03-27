using System;
using System.Collections.Generic;

namespace SharpCalculator
{
    class Logger
    {
        bool _verbose;
        System.Diagnostics.Stopwatch _watcher;

        public Logger(bool verbose)
        {
            _verbose = verbose;
            

            Console.WriteLine("Verbose enabled");
        }

        public void Log(String log)
        {
            if (_verbose) { Console.WriteLine(log); }
        }

        public void StartWatcher()
        {
            if (_verbose) { _watcher = System.Diagnostics.Stopwatch.StartNew(); }
        }

        public void DisplayTaskEnd(String task, List<String> taskOutput)
        {
            if (_verbose)
            {
                _watcher.Stop();

                Console.Write(task);
                foreach (String i in taskOutput)
                {
                    Console.Write(i+" ");
                }

                Console.Write("\n[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("✓");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("]");
                Console.WriteLine("[" + _watcher.ElapsedMilliseconds + "ms]\n");


                
            }
        }
        public void DebugLog(String log)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(log);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void DisplayTaskEnd(String task)
        {
            if (_verbose)
            {
                _watcher.Stop();

                Console.WriteLine(task);


                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("✓");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("]");
                Console.WriteLine("[" + _watcher.ElapsedMilliseconds + "ms]\n");
            }
        }

        public void LogCalculation(String functionName, List<Double> args, Double result)
        {
            if (_verbose)
            {
                String log;
                log = functionName + "(";
                args.Reverse();
                foreach (Double arg in args)
                {
                    log = log + arg + ", ";
                }
                log = "\t" + log.Substring(0, log.Length-2) + ") = " + result;


                Console.WriteLine(log);
            }
        }
    }
}


// double total = (double)watchCleaning.ElapsedMilliseconds + (double)watchConversion.ElapsedMilliseconds + (double)watchEvaluation.ElapsedMilliseconds;
//Console.WriteLine("[Tot: " + total + "ms]");