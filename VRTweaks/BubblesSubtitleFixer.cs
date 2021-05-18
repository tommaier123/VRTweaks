using HarmonyLib;
using UnityEngine;
using UWE;

namespace VRTweaks
{
    [HarmonyPatch(typeof(PlayerBreathBubbles), "Start")]
    public static class PlayerBreathBubbles_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(PlayerBreathBubbles __instance)
        {
            //Place the bubbles right at about neck level but does not rotate with view
            __instance.anchor.position = new Vector3(0.0f, 1.6f, 0.0f);
            return true;
        }
    }

    //Dosen't seem to work in anything other then Update, need to find a way so I don't have to use hardcoded values
    [HarmonyPatch(typeof(Subtitles), "Update")]
    public static class SubtitleFixer
    {
        [HarmonyPrefix]
        public static bool Prefix(Subtitles __instance)
        {
            __instance.transform.parent.localPosition = new Vector3(-457.6f, -432.5f, 0.0f);
            return true;
        }
    }
}
