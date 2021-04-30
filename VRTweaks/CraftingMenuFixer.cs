using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;
using UWE;

namespace VRTweaks
{
    public static class ActualGazeCursor
    {
        public static bool actualGazedBasedCursor;
    }

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
    [HarmonyPatch(typeof(FPSInputModule), "UpdateCursor")]
    public static class UpdateCursorPrefix
    {

        [HarmonyPrefix]
        public static void Prefix(FPSInputModule __instance)
        {
            //save the original value so we can set it back in the postfix
            ActualGazeCursor.actualGazedBasedCursor = VROptions.gazeBasedCursor;
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
            VROptions.gazeBasedCursor = ActualGazeCursor.actualGazedBasedCursor;
            //Fix the problem with the cursor rendering behind UI elements.
            Canvas cursorCanvas = __instance._cursor.GetComponentInChildren<Graphic>().canvas;
            RaycastResult lastRaycastResult = Traverse.Create(__instance).Field("lastRaycastResult").GetValue<RaycastResult>();
            if (cursorCanvas && lastRaycastResult.isValid)
            {
                cursorCanvas.sortingLayerID = lastRaycastResult.sortingLayer;//put the cursor on the same layer as whatever was hit by the cursor raycast.
            }
        }
    }
    [HarmonyPatch(typeof(HandReticle), "LateUpdate")]
    public static class HandRectilePostifix
    {
        
        [HarmonyPostfix]
        public static void HandRLateUpdate_Postfix(HandReticle __instance)
        {
            float x = InputTracking.GetLocalPosition(XRNode.RightHand).x;
            float y = InputTracking.GetLocalPosition(XRNode.RightHand).y;
            float z = InputTracking.GetLocalPosition(XRNode.RightHand).z;

            if (__instance.transform.parent != null && VRHandsController.rightController.transform != null)
            {
                IPrefabRequest request = UWE.PrefabDatabase.GetPrefabForFilenameAsync("H:/Steam Games SSD/steamapps/common/SubnauticaZero/QMods/VRTweaks/controller.prefab");
               // yield return request;
                request.TryGetPrefab(out GameObject originalPrefab);
                GameObject resultPrefab = Object.Instantiate(originalPrefab);
                //  Debug.Log("Parent:" + __instance.transform.parent);
                // __instance.transform.SetParent(VRHandsController.rightController.transform);

                // Debug.Log("__instance:" + __instance.transform.parent.transform);
                //  __instance.transform.SetParent(VRHandsController.rightController.transform);
                //   Debug.Log("__instance:" + __instance.hand.position);
                //__instance.GetComponent<Canvas>().transform.SetParent(VRHandsController.rightController.transform);
                //__instance.transform.position = new Vector3(x, y, z+0.5f);
                resultPrefab.transform.position = new Vector3(x, y, z + 0.5f);
                __instance.transform.position = new Vector3(x, y, z + 0.5f);
                //   __instance.transform.position = new Vector3(InputTracking.GetLocalPosition(XRNode.RightHand).x, InputTracking.GetLocalPosition(XRNode.RightHand).y,InputTracking.GetLocalPosition(XRNode.RightHand).z).normalized;
            }
        }
    }
}