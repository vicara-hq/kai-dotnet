using System;
using System.IO;

namespace Kai.Module
{
    public static class Log
    {
        private static readonly string LogLocation = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "kai-dotnet.log"
        );
        
        public enum Level
        {
            Verbose = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }

        private static Level level;
        private static bool ready;
        private static StreamWriter logStream;
        
        public static ModuleLogStream moduleStream;
        
        public static void Init(Level level)
        {
            if (ready)
                return;

            if (moduleStream == null)
            {
                logStream = new StreamWriter(LogLocation, true)
                {
                    AutoFlush = true
                };
            }

            Log.level = level;

            Write("--- Kai Logger init. Set triage to ");
            switch (Log.level)
            {
                case Level.Verbose:
                    WriteLine("VERBOSE");
                    break;
                case Level.Info:
                    WriteLine("INFO");
                    break;
                case Level.Warning:
                    WriteLine("WARN");
                    break;
                case Level.Error:
                    WriteLine("ERROR");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ready = true;
        }

        public static void Verbose(string str)
        {
            if (!ready)
                return;
            if (level > Level.Verbose) // Only log deeper errors
                return;
            var prt = $"[VERBOSE] {DateTime.Now}: {str}";
            WriteLine(prt);
        }

        public static void Info(string str)
        {
            if (!ready)
                return;
            if (level > Level.Info)
                return;
            var prt = $"[INFO] {DateTime.Now}: {str}";
            WriteLine(prt);
        }

        public static void Warn(string str)
        {
            if (!ready)
                return;
            if (level > Level.Warning)
                return;
            var prt = $"[WARN] {DateTime.Now}: {str}";
            WriteLine(prt);
        }

        public static void Error(string str)
        {
            if (!ready)
                return;
            if (level > Level.Error)
                return;
            var prt = $"[ERROR] {DateTime.Now}: {str}";
            WriteLine(prt);
        }

        private static void Write(string str)
        {
            moduleStream?.Invoke(str);
            logStream?.Write(str);
        }

        private static void WriteLine(string str)
        {
            moduleStream?.Invoke(str);
            logStream?.WriteLine(str);
        }
    }
}