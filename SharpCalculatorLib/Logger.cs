using System;
using System.Collections.Generic;

namespace SharpCalculatorLib
{
    internal class Logger
    {
        public bool Verbose;
        private System.Diagnostics.Stopwatch _watcher;
        private List<long> _endedTaskDuration = new();

        public Logger(bool verbose)
        {
            Verbose = verbose;
        }

        public void Log(String log)
        {
            if (Verbose) { Console.WriteLine(log); }
        }

        public void StartWatcher()
        {
            if (Verbose) { _watcher = System.Diagnostics.Stopwatch.StartNew(); }
        }

        public void ConsoleDisplayTaskEnd(String task, List<String> taskOutput)
        {
            if (Verbose)
            {
                _watcher.Stop();
                Console.Write(task);
                foreach (String i in taskOutput)
                {
                    Console.Write(i + " ");
                }

                Console.Write("\n[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("✓");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("]");
                Console.WriteLine("[" + _watcher.ElapsedMilliseconds + "ms]\n");

                _endedTaskDuration.Add(_watcher.ElapsedMilliseconds);
            }
        }

        public void ConsoleDisplayTaskEnd(String task)
        {
            if (Verbose)
            {
                _watcher.Stop();

                Console.WriteLine(task);

                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("✓");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("]");
                Console.WriteLine("[" + _watcher.ElapsedMilliseconds + "ms]\n");

                _endedTaskDuration.Add(_watcher.ElapsedMilliseconds);
            }
        }

        public void ConsoleLogCalculation(String functionName, List<string> args, string result)
        {
            if (Verbose)
            {
                String log;
                log = functionName + "(";
                args.Reverse();
                foreach (string arg in args)
                {
                    log = log + arg + ", ";
                }
                log = "\t" + log[0..^2] + ") = " + result;

                Console.WriteLine(log);
            }
        }

        public void ConsoleLogTotalDuration()
        {
            if (Verbose)
            {
                long total = 0;
                foreach (long duration in _endedTaskDuration)
                {
                    total += duration;
                }
                _endedTaskDuration = new List<long>();
                Console.WriteLine("[Tot : " + total + "ms]");
            }
        }

        public static void DebugLog(String log)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(log);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}