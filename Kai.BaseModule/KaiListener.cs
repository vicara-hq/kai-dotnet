using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Kai.Module.JObjectUtils;

namespace Kai.Module
{
	public static partial class KaiListener
	{
		private static Kai[] connectedKais = new Kai[8];
		private static bool initialised;

		/// <summary>
		/// Represents a boolean value whether the module is authenticated or not
		/// </summary>
		public static bool Authenticated { get; private set; }
		
		/// <summary>
		/// Represents the <code>ProcessName</code> of the process that's in focus
		/// </summary>
		/// <see cref="System.Diagnostics.Process.ProcessName"/>
		public static string ForegroundProcess { get; private set; }
		
		/// <summary>
		/// Represents the module ID
		/// </summary>
		public static string ModuleID { get; private set; }
		
		/// <summary>
		/// Represents the secret data of the Module
		/// </summary>
		public static string ModuleSecret { get; private set; }

		/// <summary>
		/// Occurs when an unrecognised data is received by the module
		/// </summary>
		public static UnknownDataHandler UnknownData;

		/// <summary>
		/// Occurs when an error is reported by the SDK
		/// </summary>
		public static EventHandler<ErrorEventArgs> Error;

		/// <summary>
		/// Contains the value of the default Kai that is connected to the SDK
		/// </summary>
		public static Kai DefaultKai { get; private set; }
		
		/// <summary>
		/// Throws data when any Kai receives data
		/// </summary>
		public static Kai AnyKai { get; } = new Kai();
		
		// TODO LeftKai and RightKai

		/// <summary>
		/// Initialises the SDK. This function *has* to be called before receiving data from the Kai
		/// </summary>
		public static void Initialise(string moduleId, string moduleSecret)
		{
			ModuleID = moduleId;
			ModuleSecret = moduleSecret;
			
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
			
			SendAuth(ModuleID, ModuleSecret);
			
			// TODO check compatibility with SDK
			// Test compatibility
			// Send capabilities
		}
		
		/// <summary>
		/// Sets the Kai's capabilities and subscribes to that data
		/// </summary>
		/// <param name="capabilities">The capabilities to set the Kai to</param>
		/// <param name="kai">The kai to set the capabilities to</param>
		public static void SetCapabilities(KaiCapabilities capabilities, Kai kai)
		{
			// TODO Make SetCapabilities call send / sendAsync based on parameter
			if(!Authenticated)
				throw new ApplicationException("module not authenticated");

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
			kai.Capabilities = capabilities;
		}
		
		static partial void SetupConnections();
		static partial void Send(string data);

		private static Task SendAsync(string data)
		{
			return Task.Run(() => Send(data));
		}
		
		private static void SendAuth(string moduleId, string moduleSecret)
		{
			var json = new JObject
			{
				[Constants.Type] = Constants.Authentication,
				[Constants.ModuleId] = moduleId,
				[Constants.ModuleSecret] = moduleSecret
			};

			Send(json.ToString(Formatting.None));
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

			var success = input.GetObjectAs<bool>(Constants.Success);
			if (!success)
			{
				ParseSDKError(input);
				return;
			}

			var type = input.GetObjectAs<string>(Constants.Type);

			switch (type)
			{
				case Constants.IncomingData:
					ParseIncomingData(input);
					break;
				case Constants.Authentication:
					ParseAuthentication(input);
					break;
				case Constants.ConnectedKais:
					ParseConnectedKais(input);
					break;
				default:
					UnknownData?.Invoke(input);
					break;
			}
		}
		
		private static void ParseSDKError(JObject input)
		{
			var errorCode = input.GetObjectAs<int>(Constants.ErrorCode);
			var error = input.GetObjectAs<string>(Constants.Error);
			var message = input.GetObjectAs<string>(Constants.Message);

			Error?.Invoke(null, new ErrorEventArgs(errorCode, error, message));
		}

		private static void ParseIncomingData(JObject input)
		{
			ForegroundProcess = input.GetObjectAs<string>(Constants.ForegroundProcess);
			var kaiId = input.GetObjectAs<int>(Constants.KaiId);
			var kai = connectedKais[kaiId];

			var dataList = input.GetObjectAs<JArray>(Constants.Data);

			if (dataList == null)
			{
				// TODO Log.Warning($"SDK data not formatted properly. Received: {data}");
				return;
			}

			foreach (var data in dataList)
			{
				if (data.Type != JTokenType.Object)
				{
					// TODO Log.Warning($"SDK data not formatted properly. Received: {data}");
					continue;
				}

				var dataObject = data.ToObject<JObject>();
				var type = dataObject.GetObjectAs<string>(Constants.Type);

				switch (type)
				{
					case Constants.GestureData:
						ParseGestureData(dataObject);
						break;
					case Constants.FingerShortcutData:
						ParseFingerShortcutData(dataObject);
						break;
					case Constants.PYRData:
						ParsePYRData(dataObject);
						break;
					case Constants.QuaternionData:
						ParseQuaternionData(dataObject);
						break;
					case Constants.LinearFlickData:
						ParseLinearFlickData(dataObject);
						break;
					case Constants.FingerPositionalData:
						ParseFingerPositionalData(dataObject);
						break;
					case Constants.AccelerometerData:
						ParseAccelerometerData(dataObject);
						break;
					case Constants.GyroscopeData:
						ParseGyroscopeData(dataObject);
						break;
					case Constants.MagnetometerData:
						ParseMagnetometerData(dataObject);
						break;
					default:
						UnknownData.Invoke(input);
						break;
				}
			}
			
			void ParseGestureData(JObject data)
			{
				var gesture = data.GetObjectAs<string>(Constants.Gesture);

				if (Enum.TryParse(gesture, true, out Gesture knownGesture))
				{
					kai.Gesture?.Invoke(kai, new GestureEventArgs(knownGesture));
					AnyKai.Gesture?.Invoke(kai, new GestureEventArgs(knownGesture));
				}
				else
				{
					kai.UnknownGesture?.Invoke(kai, new UnknownGestureEventArgs(gesture));
					AnyKai.UnknownGesture?.Invoke(kai, new UnknownGestureEventArgs(gesture));
				}
			}
	
			void ParseFingerShortcutData(JObject data)
			{
				var dataArray = data.GetObjectAs<JArray>(Constants.Fingers);
				var array = new bool[4];
				
				for (var i = 0; i < dataArray.Count; i++)
				{
					array[i] = dataArray[i].ToObject<bool>();
				}
	
				kai.FingerShortcut?.Invoke(kai, new FingerShortcutEventArgs(array));
				AnyKai.FingerShortcut?.Invoke(kai, new FingerShortcutEventArgs(array));
			}
	
			void ParseQuaternionData(JObject data)
			{
				var json = data.GetObjectAs<JObject>(Constants.Quaternion);
				
				var quaternion = new Quaternion
				{
					w = json.GetObjectAs<float>(Constants.W),
					x = json.GetObjectAs<float>(Constants.X),
					y = json.GetObjectAs<float>(Constants.Y),
					z = json.GetObjectAs<float>(Constants.Z)
				};
				
				kai.QuaternionData?.Invoke(kai, new QuaternionEventArgs(quaternion));
				AnyKai.QuaternionData?.Invoke(kai, new QuaternionEventArgs(quaternion));
			}
	
			void ParsePYRData(JObject json)
			{
				var yaw = json.GetObjectAs<float>(Constants.Yaw);
				var pitch = json.GetObjectAs<float>(Constants.Pitch);
				var roll = json.GetObjectAs<float>(Constants.Roll);
				
				kai.PYRData?.Invoke(kai, new PYREventArgs(yaw,pitch,roll));
				AnyKai.PYRData?.Invoke(kai, new PYREventArgs(yaw,pitch,roll));
			}
			
			void ParseLinearFlickData(JObject data)
			{
				var flick = data.GetObjectAs<string>(key: Constants.Flick);
				kai.LinearFlickData?.Invoke(kai,new LinearFlickEventArgs(flick));
				AnyKai.LinearFlickData?.Invoke(kai,new LinearFlickEventArgs(flick));
			}

			void ParseFingerPositionalData(JObject data)
			{
				var json = data.GetObjectAs<JArray>(Constants.Fingers);
				var array = new float[4];
				
				for (var i = 0; i < json.Count; i++)
				{
					array[i] = json[i].ToObject<float>();
				}
				
				kai.FingerPositionalData?.Invoke(kai,new FingerPositionalEventArgs(array));
				AnyKai.FingerPositionalData?.Invoke(kai,new FingerPositionalEventArgs(array));
			}

			void ParseAccelerometerData(JObject data)
			{
				var json = data.GetObjectAs<JObject>(Constants.Accelerometer);
				
				var accelerometer = new Accelerometer
				{
					x = json.GetObjectAs<float>(Constants.X),
					y = json.GetObjectAs<float>(Constants.Z),
					z = json.GetObjectAs<float>(Constants.Z),
				};
				
				kai.AccelerometerData?.Invoke(kai,new AccelerometerEventArgs(accelerometer));
				AnyKai.AccelerometerData?.Invoke(kai,new AccelerometerEventArgs(accelerometer));
			}
			
			void ParseGyroscopeData(JObject data)
			{
				var json = data.GetObjectAs<JObject>(Constants.Gyroscope);
				
				var gyroscope = new Gyroscope
				{
					x = json.GetObjectAs<float>(Constants.X),
					y = json.GetObjectAs<float>(Constants.Z),
					z = json.GetObjectAs<float>(Constants.Z),
				};
				
				kai.GyroscopeData?.Invoke(kai,new GyroscopeEventAgrs(gyroscope));
				AnyKai.GyroscopeData?.Invoke(kai,new GyroscopeEventAgrs(gyroscope));
			}
			
			void ParseMagnetometerData(JObject data)
			{
				var json = data.GetObjectAs<JObject>(Constants.Magnetometer);
				
				var magnetometer = new Magnetometer
				{
					x = json.GetObjectAs<float>(Constants.X),
					y = json.GetObjectAs<float>(Constants.Z),
					z = json.GetObjectAs<float>(Constants.Z),
				};
				
				kai.MagnetometerData?.Invoke(kai,new MagnetometerEventAgrs(magnetometer));
				AnyKai.MagnetometerData?.Invoke(kai,new MagnetometerEventAgrs(magnetometer));
			}
		}

		private static void ParseAuthentication(JObject input)
		{
			Authenticated = input.GetObjectAs<bool>(Constants.Authenticated);
		}

		private static void ParseConnectedKais(JObject input)
		{
			var kaiList = input.GetObjectAs<JArray>(Constants.Kais);
			connectedKais = new Kai[8];
			foreach (var token in kaiList)
			{
				var kai = token.ToObject<JObject>();
				var kaiID = kai.GetObjectAs<int>(Constants.KaiId);
				var hand = kai.GetObjectAs<string>(Constants.Hand);
				var def = kai.GetObjectAs<bool>(Constants.Default);

				if (!Enum.TryParse(hand, true, out Hand handEnum))
					handEnum = Hand.Left;
				
				connectedKais[kaiID] = new Kai
				{
					Hand = handEnum
				};

				if (def)
					DefaultKai = connectedKais[kaiID];
			}
		}
	}
}