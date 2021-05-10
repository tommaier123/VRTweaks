using HarmonyLib;
using UnityEngine;

namespace VRTweaks
{
    [HarmonyPatch(typeof(uGUI_CraftingMenu), "Open")]
    public static class CraftingMenuFixer
    {
        [HarmonyPrefix]
        public static bool Prefix(uGUI_CraftingMenu __instance)
        {
            //Fixes a bug where the crafting menu is weirdly rotated and icons are not visible
            __instance.gameObject.GetComponent<uGUI_CanvasScaler>().vrMode = uGUI_CanvasScaler.Mode.Parented;
            return true;
        }
    }
}
