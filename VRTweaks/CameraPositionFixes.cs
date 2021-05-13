using UnityEngine;
using HarmonyLib;

namespace VRTweaks
{
    class CameraPositionFixes
    {

        // In the base game, the camera position is shifted backwards behind the neck while
        // the PDA is not out, and when the PDA is out, it is shifted forwards to the "proper" location.
        // This patch keeps the camera position aligned on the center of the player body always.
        // A side effect is that it also no longer causes the player body to be clipped at the value
        // it was by default, so looking down you can see the body without clipping.
        [HarmonyPatch(typeof(MainCameraControl), nameof(MainCameraControl.LateUpdate))]
        class CameraForwardPosition_Patch
        {
            static bool Prefix(MainCameraControl __instance)
            {
                __instance.cameraUPTransform.localPosition
                    = new Vector3(__instance.cameraUPTransform.localPosition.x, __instance.cameraUPTransform.localPosition.y, 0f);
                return false;
            }

        }

    }
}
