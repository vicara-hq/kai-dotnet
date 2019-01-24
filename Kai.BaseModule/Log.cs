using System;

namespace Kai.Module
{
    public static class Log
    {
        public static LogStream Stream;
        public static void Warning(string message)
        {
            Stream?.Invoke(message);
        }
    }
}