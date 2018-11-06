namespace Kai.Module
{
	public class KaiDevice
	{
		// TODO handle multiple KAI data
		public byte[] KaiIDRaw { get; }
		public long KaiID { get; }
		public KaiCapabilities Capabilities { get; internal set; }
	}
}