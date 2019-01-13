using System;

namespace Kai.Module
{
	[Flags]
	public enum KaiCapabilities
	{
		GestureData		= 0b1000_0000,
		LinearFlicks	= 0b0100_0000,
		FingerShortcut	= 0b0010_0000,
		FingerPosition	= 0b0001_0000,
		PYRData			= 0b0000_1000,
		QuaternionData	= 0b0000_0100,
		RawData			= 0b0000_0010
	}

	public enum Gesture
	{
		SwipeUp			= 0b0000_0001,
		SwipeDown		= 0b0000_0010,
		SwipeLeft		= 0b0000_0011,
		SwipeRight		= 0b0000_0100,
		SideSwipeUp		= 0b0000_0101,
		SideSwipeDown	= 0b0000_0110,
		SideSwipeLeft	= 0b0000_0111,
		SideSwipeRight	= 0b0000_1000,
		Pinch2Begin		= 0b0000_1001,
		Pinch2End		= 0b0000_1010,
		GrabBegin		= 0b0000_1011,
		GrabEnd			= 0b0000_1100,
		Pinch3Begin		= 0b0000_1101,
		Pinch3End		= 0b0000_1110,
		DialBegin		= 0b0000_1111,
		DialEnd			= 0b0001_0000
	}

	public enum Hand
	{
		Left,
		Right
	}

	// ReSharper disable InconsistentNaming
	public struct Vector3
	{
		public int x, y, z;
	}

	public struct Quaternion
	{
		public float w, x, y, z;
	}

	public struct Accelerometer
	{
		public float x, y, z;
	}

	public struct Gyroscope
	{
		public float x, y, z;
	}
	
	public struct Magnetometer
	{
		public float x, y, z;
	}
	// ReSharper restore InconsistentNaming
}