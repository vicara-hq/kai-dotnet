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

	public class LinearFlickEventArgs : EventArgs
	{
		public string Flick { get; }

		public LinearFlickEventArgs(string flick)
		{
			Flick = flick;
		}
	}

	public class FingerPositionalEventArgs : EventArgs
	{
		public int[] Fingers { get; }

		public int LittleFinger => Fingers[0];
		public int RingFinger=> Fingers[1];
		public int MiddleFinger  => Fingers[2];
		public int IndexFinger => Fingers[3];

		public FingerPositionalEventArgs(int[] fingers)
		{
			Fingers = fingers;
		}
	}

	public class AccelerometerEventArgs : EventArgs
	{
		public Vector3 Accelerometer { get; }

		public AccelerometerEventArgs(Vector3 accelerometer)
		{
			Accelerometer = accelerometer;
		}
	}

	public class GyroscopeEventArgs : EventArgs
	{
		public Vector3 Gyroscope { get; }

		public GyroscopeEventArgs(Vector3 gyroscope)
		{
			Gyroscope = gyroscope;
		}
		
	}
	
	public class MagnetometerEventArgs : EventArgs
	{
		public Vector3 Magnetometer { get; }

		public MagnetometerEventArgs(Vector3 magnetometer)
		{
			Magnetometer = magnetometer;
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
		public float Yaw { get; }
		
		public float Pitch { get; }
		
		public float Roll { get; }

		public PYREventArgs(float yaw, float pitch, float roll)
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
	public delegate void ModuleLogStream(string data);
}