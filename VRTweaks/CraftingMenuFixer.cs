using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;
using UWE;

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

   /* [HarmonyPatch(typeof(HandReticle), "LateUpdate")]
    public static class HandRectilePostifix
    {
        [HarmonyPrefix]
        public static bool Prefix(HandReticle __instance)
        {
            if (__instance.transform.position != null)
            {
                float x = InputTracking.GetLocalPosition(XRNode.RightHand).x;
                float y = InputTracking.GetLocalPosition(XRNode.RightHand).y;
                float z = InputTracking.GetLocalPosition(XRNode.RightHand).z;

                //float X = VRHandsController.rightController.transform.position.x;
               // float Y = VRHandsController.rightController.transform.position.y;
               // float Z = VRHandsController.rightController.transform.position.z;
                //  __instance.transform.position = new Vector3(X, Y, Z + 0.5f);
                __instance.transform.position = new Vector3(x, y, z + 0.5f);
                //   __instance.transform.position = new Vector3(InputTracking.GetLocalPosition(XRNode.RightHand).x, InputTracking.GetLocalPosition(XRNode.RightHand).y,InputTracking.GetLocalPosition(XRNode.RightHand).z).normalized;
              
            }
            return true;
        }
    }*/
}