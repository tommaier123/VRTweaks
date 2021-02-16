using UnityEngine;
using UXR = UnityEngine.XR;

namespace UWEXR
{
    public class XRSettings
	{
		public static bool enabled
		{
			get
			{
				return UXR.XRSettings.enabled;
			}
			set
			{
				UXR.XRSettings.enabled = value;
			}
		}

		public static bool isDeviceActive
		{
			get
			{
				return UXR.XRSettings.isDeviceActive;
			}
		}

		public static bool showDeviceView
		{
			get
			{
				return UXR.XRSettings.showDeviceView;
			}
			set
			{
				UXR.XRSettings.showDeviceView = value;
			}
		}

		public static float renderScale
		{
			get
			{
				return UXR.XRSettings.renderScale;
			}
			set
			{
				UXR.XRSettings.renderScale = value;
			}
		}

		public static float eyeTextureResolutionScale
		{
			get
			{
				return UXR.XRSettings.eyeTextureResolutionScale;
			}
			set
			{
				UXR.XRSettings.eyeTextureResolutionScale = value;
			}
		}

		public static int eyeTextureWidth
		{
			get
			{
				return UXR.XRSettings.eyeTextureWidth;
			}
		}

		public static int eyeTextureHeight
		{
			get
			{
				return UXR.XRSettings.eyeTextureHeight;
			}
		}

		public static RenderTextureDescriptor eyeTextureDesc
		{
			get
			{
				return UXR.XRSettings.eyeTextureDesc;
			}
		}

		public static float renderViewportScale
		{
			get
			{
				return UXR.XRSettings.renderViewportScale;
			}
			set
			{
				UXR.XRSettings.renderViewportScale = value;
			}
		}

		public static float occlusionMaskScale
		{
			get
			{
				return UXR.XRSettings.occlusionMaskScale;
			}
			set
			{
				UXR.XRSettings.occlusionMaskScale = value;
			}
		}

		public static bool useOcclusionMesh
		{
			get
			{
				return UXR.XRSettings.useOcclusionMesh;
			}
			set
			{
				UXR.XRSettings.useOcclusionMesh = value;
			}
		}

		public static string loadedDeviceName
		{
			get
			{
				return UXR.XRSettings.loadedDeviceName;
			}
		}
	}
}
