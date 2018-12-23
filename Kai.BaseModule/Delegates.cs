using System;
using Newtonsoft.Json.Linq;

namespace Kai.Module
{
	public delegate void GestureDataHandler(Gesture gesture, string kaiId);
	public delegate void UnknownGestureDataHandler(string gesture);

	public delegate void FingerShortcutDataHandler(bool[] array, string kaiId);

	public delegate void PYRDataHandler(Vector3 accelerometer, Vector3 gyroscope, string kaiId);

	public delegate void QuaternionDataHandler(Quaternion quaternion, string kaiId);

	public delegate void KaiErrorHandler(ErrorEventArgs error);

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
}