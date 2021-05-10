using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VRTweaks.Fixes
{
    //It fixes the layer issue on the menu but only works with mouse and keyboard while in game
    class gazeLayerFix
    {
        public static bool actualGazedBasedCursor;

        [HarmonyPatch(typeof(FPSInputModule), "UpdateCursor")]
        public static class UpdateCursorPrefix
        {
            [HarmonyPrefix]
            public static void Prefix(FPSInputModule __instance)
            {
                //save the original value so we can set it back in the postfix
                actualGazedBasedCursor = VROptions.gazeBasedCursor;
                //trying make flag in UpdateCursor be true if Cursor.lockState != CursorLockMode.Locked)
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    VROptions.gazeBasedCursor = true;
                }
            }
        }

        [HarmonyPatch(typeof(FPSInputModule), "UpdateCursor")]
        public static class UpdateCursorPostfix
        {
            [HarmonyPostfix]
            public static void UpdateCursor_Postfix(FPSInputModule __instance)
            {
                VROptions.gazeBasedCursor = actualGazedBasedCursor;
                //Fix the problem with the cursor rendering behind UI elements.
                Canvas cursorCanvas = __instance._cursor.GetComponentInChildren<Graphic>().canvas;
                RaycastResult lastRaycastResult = Traverse.Create(__instance).Field("lastRaycastResult").GetValue<RaycastResult>();
                if (cursorCanvas && lastRaycastResult.isValid)
                {
                    cursorCanvas.sortingLayerID = lastRaycastResult.sortingLayer;//put the cursor on the same layer as whatever was hit by the cursor raycast.
                }
            }
        }
    }
}
