using UXR=UnityEngine.XR;

namespace UWEXR
{
	public static class XRDevice
	{
		public static bool isPresent
		{
			get
			{
				return UXR.XRDevice.isPresent;
			}
		}
	}
}
