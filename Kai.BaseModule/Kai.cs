using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kai.Module
{
	public class Kai
	{
		/// <summary>
		/// Contains a identifier of a Kai from all the other connected Kai's
		/// </summary>
		public int KaiID { get; internal set; }

		/// <summary>
		/// Contains the capabilities of the Kai
		/// </summary>
		public KaiCapabilities Capabilities { get; internal set; }
		
		/// <summary>
		/// Mentions the Hand that this Kai is worn in
		/// </summary>
		public Hand Hand { get; internal set; }
		
		/// <summary>
		/// Occurs when a gesture is performed
		/// </summary>
		public EventHandler<GestureEventArgs> Gesture;
		
		/// <summary>
		/// Occurs when an unknown gesture is performed
		/// </summary>
		public EventHandler<UnknownGestureEventArgs> UnknownGesture;

		/// <summary>
		/// Occurs when a finger shortcut is performed
		/// </summary>
		public EventHandler<FingerShortcutEventArgs> FingerShortcut;

		/// <summary>
		/// Occurs when PYR Data is received
		/// </summary>
		public EventHandler<PYREventArgs> PYRData;

		/// <summary>
		/// Occurs when  Quaternion Data is received
		/// </summary>
		public EventHandler<QuaternionEventArgs> QuaternionData;
		
		/// <summary>
		/// Occurs when  LinearFlick Data is received
		/// </summary>
		public EventHandler<LinearFlickEventArgs> LinearFlick;
		
		/// <summary>
		/// Occurs when  Finger Positional Data is received
		/// </summary>
		public EventHandler<FingerPositionalEventArgs> FingerPositionalData;
		
		/// <summary>
		/// Occurs when  Accelerometer Data is received
		/// </summary>
		public EventHandler<AccelerometerEventArgs> AccelerometerData;
		
		/// <summary>
		/// Occurs when  Gyroscope Data is received
		/// </summary>
		public EventHandler<GyroscopeEventArgs> GyroscopeData;
		
		/// <summary>
		/// Occurs when  Magnetometer Data is received
		/// </summary>
		public EventHandler<MagnetometerEventArgs> MagnetometerData;

		/// <summary>
		/// Sets the Kai's capabilities and subscribes to that data
		/// </summary>
		/// <param name="capabilities">The capabilities to set the Kai to</param>
		public void SetCapabilities(KaiCapabilities capabilities)
		{
			KaiSDK.SetCapabilities(this, capabilities);
		}
		
		public static bool operator ==(Kai first, Kai second)
		{
			return first?.KaiID == second?.KaiID;
		}

		public static bool operator !=(Kai first, Kai second)
		{
			return first?.KaiID != second?.KaiID;
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			if (!(obj is Kai))
				return false;
			return ((Kai) obj) == this;
		}

		public override int GetHashCode()
		{
			return KaiID.GetHashCode();
		}
	}
}