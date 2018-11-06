using System;
using System.Threading;
using WebSocketSharp;

// ReSharper disable once CheckNamespace
namespace Kai.Module
{
	public partial class KaiListener
	{
		private static WebSocket webSocket;
		
		partial static void SetupConnections()
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

		partial static void Send(string data)
		{
			webSocket?.Send(data);
		}

		private static void OnWebSocketMessage(object sender, MessageEventArgs e)
		{
			Handle(e.Data);
		}
	}
}