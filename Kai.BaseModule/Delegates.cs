using System;
using Newtonsoft.Json.Linq;

namespace Kai.Module
{
	public delegate void GestureDataHandler(Gesture gesture);
	public delegate void UnknownGestureDataHandler(string gesture);

	public delegate void FingerShortcutDataHandler(bool[] array);

	public delegate void PYRDataHandler(Vector6 vector6);

	public delegate void QuaternionDataHandler(Quaternion quaternion);

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