using System;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

// ReSharper disable once CheckNamespace
namespace Kai.Module
{
	public static partial class KaiListener
	{
#if DEBUG
		private static WebSocket webSocket;
		
		static partial void SetupConnections()
		{
			webSocket = new WebSocket("ws://localhost:2203");
			webSocket.OnMessage += OnWebSocketMessage;
			while (true)
			{
				try
				{
					webSocket.Connect();
					break;
				}
				catch (WebSocketException)
				{
					Thread.Sleep(2000); // Potentially do exponential back-off here
				}
			}
		}

		static partial void Send(string data)
		{
			webSocket?.Send(data);
		}

		private static void OnWebSocketMessage(object sender, MessageEventArgs e)
		{
			Handle(e.Data);
		}
#else
		private static bool running;
		
		partial static void SetupConnections()
		{
			running = true;
			Task.Run(ReadConsoleData);
			Console.CancelKeyPress += ConsoleCancelled;
		}

		partial static void Send(string data)
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
#endif
	}
}