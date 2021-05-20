using System;
using System.IO;
using HarmonyLib;
using QModManager.API.ModLoading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.XR;
using System.Reflection;
using UWE;
using System.Collections;
using System.Collections.Generic;

namespace VRTweaks
{

    [QModCore]
    public static class Loader
    {
        [QModPatch]
        public static void Initialize()
        {
            //I guess if they don't want to play in vr they don't have to.
            if(!XRSettings.enabled)
            {
                return;
            }
            File.AppendAllText("VRTweaksLog.txt", "Initializing" + Environment.NewLine);

            new GameObject("_VRTweaks").AddComponent<VRTweaks>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "VRTweaks");

            SnapTurningMenu.Patch();

            File.AppendAllText("VRTweaksLog.txt", "Done Initializing" + Environment.NewLine);
        }
    }

    public class VRTweaks : MonoBehaviour
    {
        //private static VRTweaks s_instance;

        public VRTweaks()
        {
            DontDestroyOnLoad(gameObject);
        }

        internal void Awake()
        {
            File.AppendAllText("VRTweaksLog.txt", "Mono Behaviour Started" + Environment.NewLine);
            SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CoroutineHost.StartCoroutine(RemoveNRecenter());
        }

        private static IEnumerator RemoveNRecenter()
        {
            yield return new WaitForSeconds(1);
            Recenter();
            yield break;
        }

        internal void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Recenter();
            }
        }

        public static void Recenter()
        {
            if (XRSettings.loadedDeviceName == "Oculus")
            {
                File.AppendAllText("VRTweaksLog.txt", "Recentering Oculus" + Environment.NewLine);
                OVRManager.display.RecenterPose();
                return;
            }

            if (XRSettings.loadedDeviceName == "OpenVR")
            {
                File.AppendAllText("VRTweaksLog.txt", "Recentering OpenVR" + Environment.NewLine);
                Valve.VR.OpenVR.System.ResetSeatedZeroPose();
                Valve.VR.OpenVR.Compositor.SetTrackingSpace(Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);
                return;
            }
        }
    }
}