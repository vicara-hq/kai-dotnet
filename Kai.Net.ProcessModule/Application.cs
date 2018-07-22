using System;

namespace Kai.Net.ProcessModule
{
    public static class Application
    {
        internal static bool running = true;
        
        public static void Run()
        {
            Console.CancelKeyPress += (sender, args) => Exit();
            KaiListener.Listen();
        }

        public static void Exit()
        {
            running = false;
        }
    }
}