using System;
using Newtonsoft.Json.Linq;

namespace Kai.Module
{
	public class GestureEventArgs : EventArgs
	{
		public Gesture Gesture { get; }

		public GestureEventArgs(Gesture gesture)
		{
			Gesture = gesture;
		}
	}
	
	public class UnknownGestureEventArgs : EventArgs
	{
		public string Gesture { get; }

		public UnknownGestureEventArgs(string gesture)
		{
			Gesture = gesture;
		}
	}
	
	public class FingerShortcutEventArgs : EventArgs
	{
		public bool[] Fingers { get; }

		public bool LittleFinger => Fingers[0];
		public bool RingFinger=> Fingers[1];
		public bool MiddleFinger  => Fingers[2];
		public bool IndexFinger => Fingers[3];

		public FingerShortcutEventArgs(bool[] fingers)
		{
			Fingers = fingers;
		}
	}

	public class PYREventArgs : EventArgs
	{
		public float Yaw { get; set; }
		
		public float Pitch { get; set; }
		
		public float Roll { get; set; }

		public PYREventArgs(float yaw,float pitch, float roll)
		{
			Yaw = yaw;
			Pitch = pitch;
			Roll = roll;
		}
	}

	public class QuaternionEventArgs : EventArgs
	{
		public Quaternion Quaternion { get; }

		public QuaternionEventArgs(Quaternion quaternion)
		{
			Quaternion = quaternion;
		}
	}

	public delegate void UnknownDataHandler(JObject data);

	public class ErrorEventArgs : EventArgs
	{
		public int Code { get; }
		public string Error { get; }
		public string Message { get; }

		public ErrorEventArgs(int code, string error, string message)
		{
			Code = code;
			Error = error;
			Message = message;
		}
	}

	public static class JObjectUtils
	{
		public static T GetObjectAs<T>(this JObject jObject, string key, T defaultValue = default, JTokenType unknownType = default)
		{
			return jObject.GetObjectAs(key, out _, defaultValue);
		}

		public static T GetObjectAs<T>(this JObject jObject, string key, out bool success, T defaultValue = default)
		{
            if (jObject == null)
            {
                success = false;
                return defaultValue;
            }
			
			success = true;
			
			switch (defaultValue)
			{
				case bool _ when jObject[key]?.Type == JTokenType.Boolean:
					return jObject[key].ToObject<T>();
				
				case int _ when jObject[key]?.Type == JTokenType.Integer:
					return jObject[key].ToObject<T>();
				
				case float _ when jObject[key]?.Type == JTokenType.Float:
					return jObject[key].ToObject<T>();
				
				case string _ when jObject[key]?.Type == JTokenType.String:
					return jObject[key].ToObject<T>();
				
				case JArray _ when jObject[key]?.Type == JTokenType.Array:
					return jObject[key].ToObject<T>();
				
				case JObject _ when jObject[key]?.Type == JTokenType.Object:
					return jObject[key].ToObject<T>();
				
				default:
					success = false;
					return defaultValue;
			}
		}
	}
}