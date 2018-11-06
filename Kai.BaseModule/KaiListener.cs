using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kai.Module
{
	public static partial class KaiListener
	{
		private static bool initialised;
		
		public static KaiCapabilities Capabilities { get; private set; }
		
		/// <summary>
		/// Occurs when a gesture is performed
		/// </summary>
		public static GestureDataHandler Gesture;
		
		/// <summary>
		/// Occurs when an unknown gesture is performed
		/// </summary>
		public static UnknownGestureDataHandler UnknownGesture;

		/// <summary>
		/// Occurs when an unrecognised data is received by the module
		/// </summary>
		public static UnknownDataHandler UnknownData;

		/// <summary>
		/// Occurs when an error is reported by the SDK
		/// </summary>
		public static KaiErrorHandler Error;

		/// <summary>
		/// Initialises the SDK. This function *has* to be called before receiving data from the Kai
		/// </summary>
		public static void Initialise()
		{
			// TODO check compatibility with SDK
			initialised = true;
		}

		/// <summary>
		/// Connects to the SDK and starts receiving data
		/// </summary>
		/// <exception cref="ApplicationException">Thrown if this function is called before Initialise()</exception>
		public static void Connect()
		{
			if(!initialised)
				throw new ApplicationException("You must call Initialise() before trying to get data");

			SetupConnections();
			
			// Test compatibility
			// Send capabilities
		}
		
		/// <summary>
		/// Sets the Kai's capabilities and subscribes to that data
		/// </summary>
		/// <param name="capabilities">The capabilities to set the Kai to</param>
		public static void SetCapabilities(KaiCapabilities capabilities)
		{
			// TODO Make SetCapabilities call send / sendAsync based on parameter
			var json = new JObject
			{
				[Constants.Type] = Constants.SetCapabilities
			};

			if ((capabilities & KaiCapabilities.GestureData) == KaiCapabilities.GestureData)
				json.Add(Constants.GestureData, true);
			
			if ((capabilities & KaiCapabilities.LinearFlicks) == KaiCapabilities.LinearFlicks)
				json.Add(Constants.LinearFlick, true);
			
			if ((capabilities & KaiCapabilities.FingerShortcut) == KaiCapabilities.FingerShortcut)
				json.Add(Constants.FingerShortcutData, true);
			
			if ((capabilities & KaiCapabilities.FingerPosition) == KaiCapabilities.FingerPosition)
				json.Add(Constants.FingerPositionalData, true);
			
			if ((capabilities & KaiCapabilities.PYRData) == KaiCapabilities.PYRData)
				json.Add(Constants.PYRData, true);
			
			if ((capabilities & KaiCapabilities.QuaternionData) == KaiCapabilities.QuaternionData)
				json.Add(Constants.QuaternionData, true);
			
			if ((capabilities & KaiCapabilities.RawData) == KaiCapabilities.RawData)
				json.Add(Constants.RawData, true);
			
			Send(json.ToString(Formatting.None));
			Capabilities = capabilities;
		}

		partial static void SetupConnections();
		partial static void Send(string data);

		private static Task SendAsync(string data)
		{
			return Task.Run(() => Send(data));
		}

		private static void Handle(string data)
		{
			if (!initialised)
			{
				// TODO Log.Warning($"Received {data} before the listener was initialised. Ignoring...");
				return;
			}

			JObject input;
			try
			{
				input = JObject.Parse(data);
			}
			catch (JsonReaderException)
			{
				// Ignore if the data is not formatted properly
				// TODO Log.Warning($"SDK data not formatted properly. Received: {data}");
				return;
			}

			var successKey = input[Constants.Success];
			if (successKey == null || successKey.Type != JTokenType.Boolean)
			{
				// TODO Log.Warning($"SDK data not formatted properly. Received: {data}");
				return;
			}

			var success = successKey.ToObject<bool>();
			if (!success)
			{
				ParseSDKError(input);
				return;
			}

			var typeKey = input[Constants.Type];
			if (typeKey == null || typeKey.Type == JTokenType.String)
			{
				// TODO Log.Error($"SDK data not formatted properly. Received: {data}");
				return;
			}

			var type = input[Constants.Type].ToObject<string>();
			switch (type)
			{
				case Constants.Gesture:
				{
					ParseGestureData(input);
					break;
				}
				default:
				{
					UnknownData.Invoke(input);
					break;
				}
			}
		}

		private static void ParseSDKError(JObject input)
		{
			var errorCodeKey = input[Constants.Type];
			if (errorCodeKey == null || errorCodeKey.Type == JTokenType.Integer)
			{
				// TODO Log.Error($"SDK data not formatted properly. Received: {data}");
				return;
			}

			var errorCode = errorCodeKey.ToObject<int>();

			var errorKey = input[Constants.Type];
			if (errorKey == null || errorKey.Type == JTokenType.Integer)
			{
				// TODO Log.Error($"SDK data not formatted properly. Received: {data}");
				return;
			}

			var error = errorKey.ToObject<string>();

			var messageKey = input[Constants.Type];
			if (messageKey == null || messageKey.Type == JTokenType.Integer)
			{
				// TODO Log.Error($"SDK data not formatted properly. Received: {data}");
				return;
			}

			var message = messageKey.ToObject<string>();

			Error?.Invoke(new ErrorEventArgs(errorCode, error, message));
		}

		private static void ParseGestureData(JObject input)
		{
			var gestureType = input[Constants.Gesture];
			if (gestureType == null || gestureType.Type == JTokenType.String)
			{
				// TODO Log.Error($"SDK data not formatted properly. Received: {data}");
				return;
			}

			var gesture = gestureType.ToObject<string>();

			if (Enum.TryParse(gesture, true, out Gesture knownGesture))
				Gesture?.Invoke(knownGesture);
			else
				UnknownGesture?.Invoke(gesture);
		}
	}
}