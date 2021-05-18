using HarmonyLib;
using UnityEngine;

namespace VRTweaks
{
    [HarmonyPatch(typeof(uGUI_HealthBar), "Awake")]
    public static class HUDFixer
    {
        [HarmonyPostfix]
        public static void Postfix(uGUI_HealthBar __instance)
        {
            //     //Shift the healthbar, oxygen meter, and temperature parent UI element to the right
            __instance.transform.parent.localPosition += new Vector3(300, 0, 0);
            MiscSettings.SetUIScale(0.7f);
        }
    }
}
