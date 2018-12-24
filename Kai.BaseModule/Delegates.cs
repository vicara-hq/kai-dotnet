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
		public Vector3 Accelerometer { get; }
		public Vector3 Gyroscope { get; }

		public PYREventArgs(Vector3 accelerometer, Vector3 gyroscope)
		{
			Accelerometer = accelerometer;
			Gyroscope = gyroscope;
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
			return jObject.GetObjectAs<T>(key, out var _, defaultValue, unknownType);
		}

		public static T GetObjectAs<T>(this JObject jObject, string key, out bool success, T defaultValue = default, JTokenType unknownType = default)
		{
			if (jObject == null)
				return defaultValue;
			
			success = true;
			
			switch (defaultValue)
			{
				case bool _ when jObject[key]?.Type == JTokenType.Boolean:
					return (T) jObject[key].ToObject(typeof(bool));
				
				case int _ when jObject[key]?.Type == JTokenType.Integer:
					return (T) jObject[key].ToObject(typeof(int));
				
				case float _ when jObject[key]?.Type == JTokenType.Float:
					return (T) jObject[key].ToObject(typeof(float));
				
				case string _ when jObject[key]?.Type == JTokenType.String:
					return (T) jObject[key].ToObject(typeof(string));
				
				case JArray _ when jObject[key]?.Type == JTokenType.Array:
					return (T) jObject[key].ToObject(typeof(JArray));
				
				case JObject _ when jObject[key]?.Type == JTokenType.Object:
					return (T) jObject[key].ToObject(typeof(JObject));
				
				default:
					success = false;
					return jObject.ToObject<T>();
			}
		}
	}
}