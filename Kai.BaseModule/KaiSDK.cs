using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kai.Module
{
	public static partial class KaiSDK
	{
		public static Kai[] connectedKais { get; private set; } = new Kai[8];
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
		public static Kai DefaultKai { get; } = new Kai();

		/// <summary>
		/// Contains the value of the default left Kai that is connected to the SDK
		/// </summary>
		public static Kai DefaultLeftKai { get; } = new Kai();

		/// <summary>
		/// Contains the value of the default right Kai that is connected to the SDK
		/// </summary>
		public static Kai DefaultRightKai { get; } = new Kai();
		
		/// <summary>
		/// Throws data when any Kai receives data
		/// </summary>
		public static Kai AnyKai { get; } = new Kai();

		/// <summary>
		/// Initialises the SDK. This function *has* to be called before receiving data from the Kai
		/// </summary>
		public static void Initialise(string moduleId, string moduleSecret)
		{
			ModuleID = moduleId;
			ModuleSecret = moduleSecret;
			
			initialised = true;
			Log.Init(Log.Level.Verbose);
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
		}

		/// <summary>
		/// Gets the list of all connected Kais
		/// </summary>
		public static void GetConnectedKais()
		{
			Send(new JObject()
			{
				[Constants.Type] = Constants.ListConnectedKais
			}.ToString(Formatting.None));
		}
		
		/// <summary>
		/// Set the Kai's capabilities and subscribes to that data
		/// </summary>
		/// <param name="capabilities">The capabilities to set the Kai to</param>
		/// <param name="kai">The kai to set the capabilities to</param>
		public static void SetCapabilities(Kai kai, KaiCapabilities capabilities)
		{
			kai.Capabilities |= capabilities;
			if (!Authenticated)
				return;

			var json = new JObject
			{
				[Constants.Type] = Constants.SetCapabilities
			};

			if (ReferenceEquals(kai, DefaultKai))
			{
				json.Add(Constants.KaiID, Constants.Default);
			}
			else if (ReferenceEquals(kai, DefaultLeftKai))
			{
				json.Add(Constants.KaiID, Constants.DefaultLeft);
			}
			else if (ReferenceEquals(kai, DefaultRightKai))
			{
				json.Add(Constants.KaiID, Constants.DefaultRight);
			}
			else
			{
				json.Add(Constants.KaiID, kai.KaiID);
			}

			if (capabilities.HasFlag(KaiCapabilities.GestureData))
				json.Add(Constants.GestureData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.LinearFlickData))
				json.Add(Constants.LinearFlickData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.FingerShortcutData))
				json.Add(Constants.FingerShortcutData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.FingerPositionalData))
				json.Add(Constants.FingerPositionalData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.PYRData))
				json.Add(Constants.PYRData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.QuaternionData))
				json.Add(Constants.QuaternionData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.AccelerometerData))
				json.Add(Constants.AccelerometerData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.GyroscopeData))
				json.Add(Constants.GyroscopeData, true);
			
			if (capabilities.HasFlag(KaiCapabilities.MagnetometerData))
				json.Add(Constants.MagnetometerData, true);
			
			Send(json.ToString(Formatting.None));
		}
		
		/// <summary>
		/// Unset the Kai's capabilities and subscribes to that data
		/// </summary>
		/// <param name="capabilities">The capabilities to set the Kai to</param>
		/// <param name="kai">The kai to set the capabilities to</param>
		public static void UnsetCapabilities(Kai kai, KaiCapabilities capabilities)
		{
			kai.Capabilities &= ~capabilities; // value = value AND NOT parameter. This will unset the parameter from the value
			if (!Authenticated)
				return;

			var json = new JObject
			{
				[Constants.Type] = Constants.SetCapabilities
			};

			if (ReferenceEquals(kai, DefaultKai))
			{
				json.Add(Constants.KaiID, Constants.Default);
			}
			else if (ReferenceEquals(kai, DefaultLeftKai))
			{
				json.Add(Constants.KaiID, Constants.DefaultLeft);
			}
			else if (ReferenceEquals(kai, DefaultRightKai))
			{
				json.Add(Constants.KaiID, Constants.DefaultRight);
			}
			else
			{
				json.Add(Constants.KaiID, kai.KaiID);
			}

			if (capabilities.HasFlag(KaiCapabilities.GestureData))
				json.Add(Constants.GestureData, false);
			
			if (capabilities.HasFlag(KaiCapabilities.LinearFlickData))
				json.Add(Constants.LinearFlickData, false);
			
			if (capabilities.HasFlag(KaiCapabilities.FingerShortcutData))
				json.Add(Constants.FingerShortcutData, false);
			
			if (capabilities.HasFlag(KaiCapabilities.FingerPositionalData))
				json.Add(Constants.FingerPositionalData, false);
			
			if (capabilities.HasFlag(KaiCapabilities.PYRData))
				json.Add(Constants.PYRData, false);
			
			if (capabilities.HasFlag(KaiCapabilities.QuaternionData))
				json.Add(Constants.QuaternionData, false);
			
			if (capabilities.HasFlag(KaiCapabilities.AccelerometerData))
				json.Add(Constants.AccelerometerData, false);
			
			if (capabilities.HasFlag(KaiCapabilities.GyroscopeData))
				json.Add(Constants.GyroscopeData, false);
			
			if (capabilities.HasFlag(KaiCapabilities.MagnetometerData))
				json.Add(Constants.MagnetometerData, false);
			
			Send(json.ToString(Formatting.None));
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
				Log.Warn($"Received {data} before the listener was initialised. Ignoring...");
				return;
			}

			try
			{
				var input = JObject.Parse(data);

				var success = input[Constants.Success].ToObject<bool>();
				if (success != true)
				{
					DecodeSDKError(input);
					return;
				}

				var type = input[Constants.Type].ToObject<string>();

				switch (type)
				{
					case Constants.Authentication:
						DecodeAuthentication();
						break;
					case Constants.IncomingData:
						DecodeIncomingData(input);
						break;
					case Constants.ListConnectedKais:
						DecodeConnectedKais(input);
						break;
					case Constants.KaiConnected:
						DecodeKaiConnected(input);
						break;
					default:
						UnknownData?.Invoke(input);
						break;
				}
			}
			catch (Exception e)
			{
				// Ignore if the data is not formatted properly
				Log.Warn($"Error parsing JSON. Received: {data}. Error: {e.GetType().Name} - {e.Message}: {e.StackTrace}");
			}
		}

		private static void DecodeSDKError(JObject input)
		{
			var errorCode = input[Constants.ErrorCode].ToObject<int>();
			var error = input[Constants.Error].ToObject<string>();
			var message = input[Constants.Message].ToObject<string>();

			Error?.Invoke(null, new ErrorEventArgs(errorCode, error, message));
		}

		private static void DecodeIncomingData(JObject input)
		{
			ForegroundProcess = input[Constants.ForegroundProcess].ToObject<string>();
			var kaiId = input[Constants.KaiID].ToObject<int>();
			var kai = connectedKais[kaiId];
			var defaultKai = input[Constants.DefaultKai]?.ToObject<bool>();
			var defaultLeftKai = input[Constants.DefaultLeftKai]?.ToObject<bool>();
			var defaultRightKai = input[Constants.DefaultRightKai]?.ToObject<bool>();

			var dataList = input[Constants.Data].ToObject<JArray>();

			if (dataList == null)
			{
				Log.Warn($"Data list is null. Received: {input}");
				return;
			}

			foreach (var data in dataList)
			{
				if (data.Type != JTokenType.Object)
				{
					Log.Warn($"Data is not an object. Received: {data}");
					continue;
				}

				var dataObject = data.ToObject<JObject>();
				var type = dataObject[Constants.Type].ToObject<string>();

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
				var gesture = data[Constants.Gesture].ToObject<string>();

				if (Enum.TryParse(gesture, true, out Gesture knownGesture))
				{
					FireGestureEvent(new GestureEventArgs(knownGesture));
				}
				else
				{
					FireUnknownGestureEvent(new UnknownGestureEventArgs(gesture));
				}
			}
	
			void ParseFingerShortcutData(JObject data)
			{
				var dataArray = data[Constants.Fingers].ToObject<JArray>();
				var array = new bool[4];
				
				for (var i = 0; i < dataArray.Count; i++)
				{
					array[i] = dataArray[i].ToObject<bool>();
				}
				
				FireFingerShortcutEvent(new FingerShortcutEventArgs(array));
			}
	
			void ParseQuaternionData(JObject data)
			{
				var json = data[Constants.Quaternion].ToObject<JObject>();
				
				var quaternion = new Quaternion
				{
					w = json[Constants.W].ToObject<float>(),
					x = json[Constants.X].ToObject<float>(),
					y = json[Constants.Y].ToObject<float>(),
					z = json[Constants.Z].ToObject<float>()
				};
				
				FireQuaternionEvent(new QuaternionEventArgs(quaternion));
			}
	
			void ParsePYRData(JObject json)
			{
				var pitch = json[Constants.Pitch].ToObject<float>();
				var yaw = json[Constants.Yaw].ToObject<float>();
				var roll = json[Constants.Roll].ToObject<float>();
				
				FirePYREvent(new PYREventArgs(pitch, yaw, roll));
			}
			
			void ParseLinearFlickData(JObject data)
			{
				var flick = data[Constants.Flick].ToObject<string>();
				
				FireLinearFlickEvent(new LinearFlickEventArgs(flick));
			}

			void ParseFingerPositionalData(JObject data)
			{
				var json = data[Constants.Fingers].ToObject<JObject>();
				var array = new int[4];
				
				for (var i = 0; i < json.Count; i++)
				{
					array[i] = json[i].ToObject<int>();
				}
				
				FireFingerPositionalEvent(new FingerPositionalEventArgs(array));
			}

			void ParseAccelerometerData(JObject data)
			{
				var json = data[Constants.Accelerometer].ToObject<JObject>();
				
				var accelerometer = new Vector3
				{
					x = json[Constants.X].ToObject<float>(),
					y = json[Constants.Y].ToObject<float>(),
					z = json[Constants.Z].ToObject<float>()
				};
				
				FireAccelerometerEvent(new AccelerometerEventArgs(accelerometer));
			}
			
			void ParseGyroscopeData(JObject data)
			{
				var json = data[Constants.Gyroscope].ToObject<JObject>();
				
				var gyroscope = new Vector3
				{
					x = json[Constants.X].ToObject<float>(),
					y = json[Constants.Y].ToObject<float>(),
					z = json[Constants.Z].ToObject<float>()
				};
				
				FireGyroscopeEvent(new GyroscopeEventArgs(gyroscope));
			}
			
			void ParseMagnetometerData(JObject data)
			{
				var json = data[Constants.Magnetometer].ToObject<JObject>();
				
				var magnetometer = new Vector3
				{
					x = json[Constants.X].ToObject<float>(),
					y = json[Constants.Y].ToObject<float>(),
					z = json[Constants.Z].ToObject<float>()
				};
				
				FireMagnetometerEvent(new MagnetometerEventArgs(magnetometer));
			}
			
			void FireGestureEvent(GestureEventArgs args)
			{
				if (defaultKai == true)
					DefaultKai.Gesture?.Invoke(DefaultKai, args);
				if (defaultLeftKai == true)
					DefaultLeftKai.Gesture?.Invoke(DefaultLeftKai, args);
				if (defaultRightKai == true)
					DefaultRightKai.Gesture?.Invoke(DefaultRightKai, args);
					
				kai.Gesture?.Invoke(kai, args);
				AnyKai.Gesture?.Invoke(kai, args);
			}
			
			void FireUnknownGestureEvent(UnknownGestureEventArgs args)
			{
				if (defaultKai == true)
					DefaultKai.UnknownGesture?.Invoke(DefaultKai, args);
				if (defaultLeftKai == true)
					DefaultLeftKai.UnknownGesture?.Invoke(DefaultLeftKai, args);
				if (defaultRightKai == true)
					DefaultRightKai.UnknownGesture?.Invoke(DefaultRightKai, args);
					
				kai.UnknownGesture?.Invoke(kai, args);
				AnyKai.UnknownGesture?.Invoke(kai, args);
			}
			
			void FireLinearFlickEvent(LinearFlickEventArgs args)
			{
				if (defaultKai == true)
					DefaultKai.LinearFlick?.Invoke(DefaultKai, args);
				if (defaultLeftKai == true)
					DefaultLeftKai.LinearFlick?.Invoke(DefaultLeftKai, args);
				if (defaultRightKai == true)
					DefaultRightKai.LinearFlick?.Invoke(DefaultRightKai, args);
					
				kai.LinearFlick?.Invoke(kai, args);
				AnyKai.LinearFlick?.Invoke(kai, args);
			}
			
			void FireFingerShortcutEvent(FingerShortcutEventArgs args)
			{
				if (defaultKai == true)
					DefaultKai.FingerShortcut?.Invoke(DefaultKai, args);
				if (defaultLeftKai == true)
					DefaultLeftKai.FingerShortcut?.Invoke(DefaultLeftKai, args);
				if (defaultRightKai == true)
					DefaultRightKai.FingerShortcut?.Invoke(DefaultRightKai, args);
					
				kai.FingerShortcut?.Invoke(kai, args);
				AnyKai.FingerShortcut?.Invoke(kai, args);
			}
			
			void FireFingerPositionalEvent(FingerPositionalEventArgs args)
			{
				if (defaultKai == true)
					DefaultKai.FingerPositionalData?.Invoke(DefaultKai, args);
				if (defaultLeftKai == true)
					DefaultLeftKai.FingerPositionalData?.Invoke(DefaultLeftKai, args);
				if (defaultRightKai == true)
					DefaultRightKai.FingerPositionalData?.Invoke(DefaultRightKai, args);
					
				kai.FingerPositionalData?.Invoke(kai, args);
				AnyKai.FingerPositionalData?.Invoke(kai, args);
			}
			
			void FirePYREvent(PYREventArgs args)
			{
				if (defaultKai == true)
					DefaultKai.PYRData?.Invoke(DefaultKai, args);
				if (defaultLeftKai == true)
					DefaultLeftKai.PYRData?.Invoke(DefaultLeftKai, args);
				if (defaultRightKai == true)
					DefaultRightKai.PYRData?.Invoke(DefaultRightKai, args);
					
				kai.PYRData?.Invoke(kai, args);
				AnyKai.PYRData?.Invoke(kai, args);
			}
			
			void FireQuaternionEvent(QuaternionEventArgs args)
			{
				if (defaultKai == true)
					DefaultKai.QuaternionData?.Invoke(DefaultKai, args);
				if (defaultLeftKai == true)
					DefaultLeftKai.QuaternionData?.Invoke(DefaultLeftKai, args);
				if (defaultRightKai == true)
					DefaultRightKai.QuaternionData?.Invoke(DefaultRightKai, args);
					
				kai.QuaternionData?.Invoke(kai, args);
				AnyKai.QuaternionData?.Invoke(kai, args);
			}
			
			void FireAccelerometerEvent(AccelerometerEventArgs args)
			{
				if (defaultKai == true)
					DefaultKai.AccelerometerData?.Invoke(DefaultKai, args);
				if (defaultLeftKai == true)
					DefaultLeftKai.AccelerometerData?.Invoke(DefaultLeftKai, args);
				if (defaultRightKai == true)
					DefaultRightKai.AccelerometerData?.Invoke(DefaultRightKai, args);
					
				kai.AccelerometerData?.Invoke(kai, args);
				AnyKai.AccelerometerData?.Invoke(kai, args);
			}
			
			void FireGyroscopeEvent(GyroscopeEventArgs args)
			{
				if (defaultKai == true)
					DefaultKai.GyroscopeData?.Invoke(DefaultKai, args);
				if (defaultLeftKai == true)
					DefaultLeftKai.GyroscopeData?.Invoke(DefaultLeftKai, args);
				if (defaultRightKai == true)
					DefaultRightKai.GyroscopeData?.Invoke(DefaultRightKai, args);
					
				kai.GyroscopeData?.Invoke(kai, args);
				AnyKai.GyroscopeData?.Invoke(kai, args);
			}
			
			void FireMagnetometerEvent(MagnetometerEventArgs args)
			{
				if (defaultKai == true)
					DefaultKai.MagnetometerData?.Invoke(DefaultKai, args);
				if (defaultLeftKai == true)
					DefaultLeftKai.MagnetometerData?.Invoke(DefaultLeftKai, args);
				if (defaultRightKai == true)
					DefaultRightKai.MagnetometerData?.Invoke(DefaultRightKai, args);
					
				kai.MagnetometerData?.Invoke(kai, args);
				AnyKai.MagnetometerData?.Invoke(kai, args);
			}
		}

		private static void DecodeAuthentication()
		{
			Authenticated = true;
			GetConnectedKais();
		}

		private static void DecodeConnectedKais(JObject input)
		{
			var kaiList = input[Constants.Kais].ToObject<JArray>();
			connectedKais = new Kai[8];
			foreach (var token in kaiList)
				DecodeKaiConnected((JObject)token);
			
			ResetDefaultCapabilities();
		}

		private static void DecodeKaiConnected(JObject input)
		{
			var kaiID = input[Constants.KaiID].ToObject<int>();
			var hand = input[Constants.Hand]?.ToObject<string>(); // will not be optional in future
			var defaultKai = input[Constants.Default]?.ToObject<bool>();
			var defaultLeftKai = input[Constants.DefaultLeft]?.ToObject<bool>();
			var defaultRightKai = input[Constants.DefaultRight]?.ToObject<bool>();
			var kaiSerialNumber = input[Constants.KaiSerialNumber]?.ToObject<bool>(); // will not be optional in future

			//var kaiParsed = KaiObjectParsed.Parse(input);
			if (!Enum.TryParse(hand, true, out Hand handEnum))
				handEnum = Hand.Left;
			
			if (defaultKai == true)
			{
				DefaultKai.KaiID = kaiID;
				DefaultKai.Hand = handEnum;
			}

			if (defaultLeftKai == true)
			{
				DefaultLeftKai.KaiID = kaiID;
				DefaultLeftKai.Hand = Hand.Left;
			}

			if (defaultRightKai == true)
			{
				DefaultRightKai.KaiID = kaiID;
				DefaultRightKai.Hand = Hand.Right;
			}
			
			connectedKais[kaiID] = new Kai
			{
				KaiID = kaiID,
				Hand = handEnum
			};
			
			if(defaultKai==true || defaultLeftKai==true || defaultRightKai==true)
				ResetDefaultCapabilities();
		}

		private static void ResetDefaultCapabilities(){
			DefaultKai.SetCapabilities(DefaultKai.Capabilities);
			DefaultLeftKai.SetCapabilities(DefaultLeftKai.Capabilities);
			DefaultRightKai.SetCapabilities(DefaultRightKai.Capabilities);
		}
	}
}