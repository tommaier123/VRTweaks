using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace VRTweaks
{
    [HarmonyPatch]
    public static class GazeCursorPatches
    {
        [HarmonyPatch(typeof(VROptions), nameof(VROptions.GetUseGazeBasedCursor))]
        [HarmonyPostfix]
        public static void GetUseGazeBasedCursor_Postfix(ref bool __result)
        {
            if(XRSettings.isDeviceActive)
                __result = true;
        }


        [HarmonyPatch(typeof(FPSInputModule), nameof(FPSInputModule.GetCursorScreenPosition))]
        [HarmonyPostfix]
        public static void GetCursorScreenPosition_Postfix(ref Vector2 __result)
        {
            if (XRSettings.isDeviceActive)
            {
                __result = new Vector2((float)XRSettings.eyeTextureWidth, (float)XRSettings.eyeTextureHeight) * 0.5f;
            }
        }
    }

}
