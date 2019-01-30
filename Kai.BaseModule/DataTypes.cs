using System;

namespace Kai.Module
{
	[Flags]
	public enum KaiCapabilities
	{
		GestureData,
		LinearFlickData,
		FingerShortcutData,
		FingerPositionalData,
		PYRData,
		QuaternionData,
		AccelerometerData,
		GyroscopeData,
		MagnetometerData
	}

	public enum Gesture
	{
		SwipeUp,
		SwipeDown,
		SwipeLeft,
		SwipeRight,
		SideSwipeUp,
		SideSwipeDown,
		SideSwipeLeft,
		SideSwipeRight,
		Pinch2Begin,
		Pinch2End,
		GrabBegin,
		GrabEnd,
		Pinch3Begin,
		Pinch3End,
		DialBegin,
		DialEnd			
	}

	public enum Hand
	{
		Left,
		Right
	}

	public struct Vector3
	{
		public float x, y, z;
	}

	public struct Quaternion
	{
		public float w, x, y, z;
	}
}