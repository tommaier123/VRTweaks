using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace VRTweaks
{
    [HarmonyPatch(typeof(VROptions))]
    [HarmonyPatch("GetUseGazeBasedCursor")]
    public static class GazeCursorEnabler
    {
        static GazeCursorEnabler()
        {
            if(XRSettings.isDeviceActive)
            {
                VROptions.gazeBasedCursor = true;
            }
        }

        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if(XRSettings.isDeviceActive)
            {
                __result = VROptions.gazeBasedCursor;
            }
        }
    }


    [HarmonyPatch(typeof(FPSInputModule))]
    [HarmonyPatch("GetCursorScreenPosition")]
    public static class GazeCursorPositioner
    {
        [HarmonyPostfix]
        public static void Postfix(ref Vector2 __result)
        {
            if(XRSettings.isDeviceActive)
            {
                __result = new Vector2((float)XRSettings.eyeTextureWidth, (float)XRSettings.eyeTextureHeight) * 0.5f;
            }
        }
    }
}
