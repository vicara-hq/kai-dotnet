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
		/// Occurs when a finger shortcut is performed
		/// </summary>
		public static FingerShortcutDataHandler FingerShortcut;

		/// <summary>
		/// Occurs when PYR Data is recieved
		/// </summary>
		public static PYRDataHandler PYRData;

		/// <summary>
		/// Occurs when  Quaternion Data is recieved
		/// </summary>
		public static QuaternionDataHandler QuaternionData;

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

			if (capabilities.HasFlag(KaiCapabilities.GestureData))
				json.Add(Constants.GestureData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.LinearFlicks))
				json.Add(Constants.LinearFlick, true);
			
			if (capabilities.HasFlag(KaiCapabilities.FingerShortcut))
				json.Add(Constants.FingerShortcutData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.FingerPosition))
				json.Add(Constants.FingerPositionalData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.PYRData))
				json.Add(Constants.PYRData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.QuaternionData))
				json.Add(Constants.QuaternionData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.RawData))
				json.Add(Constants.RawData, true);
			
			Send(json.ToString(Formatting.None));
			Capabilities = capabilities;
		}

		static partial void SetupConnections();
		static partial void Send(string data);

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

			var type = typeKey.ToObject<string>();
			switch (type)
			{
				case Constants.Gesture:
					ParseGestureData(input);
					break;
				case Constants.FingerShortcut:
					ParseFingerShortcutData(input);
					break;
				case Constants.PYRData:
					ParsePYRData(input);
					break;
				case Constants.QuaternionData:
					ParseQuaternionData(input);
					break;
				default:
					UnknownData.Invoke(input);
					break;
			}
		}

		private static void ParseSDKError(JObject input)
		{
			var errorCodeKey = input[Constants.ErrorCode];
			if (errorCodeKey == null || errorCodeKey.Type == JTokenType.Integer)
			{
				// TODO Log.Error($"SDK data not formatted properly. Received: {data}");
				return;
			}

			var errorCode = errorCodeKey.ToObject<int>();

			var errorKey = input[Constants.Error];
			if (errorKey == null || errorKey.Type == JTokenType.Integer)
			{
				// TODO Log.Error($"SDK data not formatted properly. Received: {data}");
				return;
			}

			var error = errorKey.ToObject<string>();

			var messageKey = input[Constants.Message];
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

		private static void ParseFingerShortcutData(JObject input)
		{
			bool[] array = new bool[4];
			var jArray = input[Constants.Fingers]?.ToObject<JArray>();
			for (int i = 0; i < jArray.Count; i++)
			{
				array[i] = jArray[i].ToObject<bool>();
			}
			FingerShortcut?.Invoke(array);
		}

		private static void ParseQuaternionData(JObject input)
		{
			var quaterionObject = input[Constants.Quaternion].ToObject<JObject>();

			if (quaterionObject == null || quaterionObject.Type == JTokenType.String)
			{
				// TODO Log.Error($"SDK data not formatted properly. Received: {data}");
				return;
			}

			Quaternion quaternion = new Quaternion
			{
				w = quaterionObject[Constants.W].ToObject<float>(),
				x = quaterionObject[Constants.X].ToObject<float>(),
				y = quaterionObject[Constants.Y].ToObject<float>(),
				z = quaterionObject[Constants.Z].ToObject<float>()
			};
			QuaternionData?.Invoke(quaternion);
		}

		private static void ParsePYRData(JObject input)
		{
			var accelerometerObject = input[Constants.Accelerometer];
			if (accelerometerObject == null || accelerometerObject.Type == JTokenType.String)
			{
				// TODO Log.Error($"SDK data not formatted properly. Received: {data}");
				return;
			}
			var gyroscopeObject = input[Constants.Gyroscope];
			if (gyroscopeObject == null || gyroscopeObject.Type == JTokenType.String)
			{
				// TODO Log.Error($"SDK data not formatted properly. Received: {data}");
				return;
			}

			Vector3 accelerometer = new Vector3
			{
				x = accelerometerObject[Constants.X].ToObject<int>(),
				y = accelerometerObject[Constants.Y].ToObject<int>(),
				z = accelerometerObject[Constants.Z].ToObject<int>()
			};

			Vector3 gyroscope = new Vector3
			{
				x = gyroscopeObject[Constants.X].ToObject<int>(),
				y = gyroscopeObject[Constants.Y].ToObject<int>(),
				z = gyroscopeObject[Constants.Z].ToObject<int>()
			};

			Vector6 vector6 = new Vector6
			{
				Accelerometer = accelerometer,
				Gyroscope = gyroscope
			};
			PYRData?.Invoke(vector6);
		}
	}
}