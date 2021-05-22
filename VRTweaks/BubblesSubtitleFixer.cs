using HarmonyLib;
using UnityEngine;


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

    [HarmonyPatch(typeof(Subtitles), "Awake")]
    public static class SubtitlesFixer
    {
        [HarmonyPostfix]
        public static void Postfix(Subtitles __instance)
        {
            __instance.transform.parent.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            __instance.transform.parent.localPosition = new Vector3(-237.6f, +225.0f, 0.0f);
        }
    }
}