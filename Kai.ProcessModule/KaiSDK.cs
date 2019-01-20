using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Kai.Module
{
	public static partial class KaiSDK
	{
		private static bool running;
		
		static partial void SetupConnections()
		{
			running = true;
			Task.Run(ReadConsoleData);
			Console.CancelKeyPress += ConsoleCancelled;
		}

		static partial void Send(string data)
		{
			Console.WriteLine(data);
		}

		private static void ConsoleCancelled(object sender, ConsoleCancelEventArgs e)
		{
			running = false;
		}

		private static void ReadConsoleData()
		{
			var inputStr = "";
			while (running)
			{
				var input = Console.ReadLine();
				
				inputStr += input;

				if (!string.IsNullOrEmpty(input))
					continue;
				
				Handle(inputStr);
				inputStr = string.Empty;
			}
		}
	}
}