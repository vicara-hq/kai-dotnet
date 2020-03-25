using System;
using System.IO;

namespace Kai.Module
{
    public static class Log
    {
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

        public static void Init(string logLocation, Level level)
        {
            if (ready)
                return;
            logStream = new StreamWriter(logLocation, true)
            {
                AutoFlush = true
            };
            Log.level = level;
            ready = true;

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
        }

        public static void Verbose(string str)
        {
            if (level > Level.Verbose) // Only log deeper errors
                return;
            var prt = $"[VERBOSE] {DateTime.Now}: {str}";
            WriteLine(prt);
        }

        public static void Info(string str)
        {
            if (level > Level.Info)
                return;
            var prt = $"[INFO] {DateTime.Now}: {str}";
            WriteLine(prt);
        }

        public static void Warn(string str)
        {
            if (level > Level.Warning)
                return;
            var prt = $"[WARN] {DateTime.Now}: {str}";
            WriteLine(prt);
        }

        public static void Error(string str)
        {
            if (level > Level.Error)
                return;
            var prt = $"[ERROR] {DateTime.Now}: {str}";
            WriteLine(prt);
        }

        private static void Write(string str)
        {
            if (!ready)
                throw new ApplicationException("You must call Init() before trying trying to log");
            logStream.Write(str);
        }

        private static void WriteLine(string str)
        {
            if (!ready)
                throw new ApplicationException("You must call Init() before trying trying to log");
            logStream.WriteLine(str);
        }
    }
}