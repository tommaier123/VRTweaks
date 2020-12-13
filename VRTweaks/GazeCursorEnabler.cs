using HarmonyLib;

namespace VRTweaks
{
    [HarmonyPatch(typeof(VROptions))]
    [HarmonyPatch("GetUseGazeBasedCursor")]
    public static class GazeCursorEnabler
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }
}
