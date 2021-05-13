using UnityEngine;
using HarmonyLib;
using FMODUnity;

namespace VRTweaks
{
    public class AudioFixes
    {

        [HarmonyPatch(typeof(SNCameraRoot), nameof(SNCameraRoot.Awake))]
        public class AudioSpatializationFixes
        {
            static void Postfix(SNCameraRoot __instance, Camera ___mainCamera)
            {
                if (___mainCamera != null)
                {
                    Object.DestroyImmediate(__instance.gameObject.GetComponent<AudioListener>());
                    Object.DestroyImmediate(__instance.gameObject.GetComponent<StudioListener>());
                    ___mainCamera.gameObject.AddComponent<StudioListener>();
                }
            }
        }

    }
}
