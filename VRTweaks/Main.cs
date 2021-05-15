using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using QModManager.API.ModLoading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.XR;
using VRTweaks.SnapTurn;
using System.Reflection;
using UWE;
using System.Collections;

namespace VRTweaks
{
    [QModCore]
    public static class Loader
    {
        [QModPatch]
        public static void Initialize()
        {
            if(!XRSettings.enabled)
            {
                return;
            }
            File.AppendAllText("VRTweaksLog.txt", "Initializing" + Environment.NewLine);

            new GameObject("_VRTweaks").AddComponent<VRTweaks>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "VRTweaks");

            SnapTurningMenu.Patch();

            VRTweaks.InitializeMainDevice();

            File.AppendAllText("VRTweaksLog.txt", "Done Initializing" + Environment.NewLine);
        }
    }

    public class VRTweaks : MonoBehaviour
    {
        //private static VRTweaks s_instance;
        public static List<InputDevice> vrDevices = new List<InputDevice>();
        public static bool usingOculus;
        public static bool usingVive;
        public static bool usingIndex;
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
            if (usingOculus)
            {
                File.AppendAllText("VRTweaksLog.txt", "Recentering Oculus" + Environment.NewLine);
                OVRManager.display.RecenterPose();
                return;
            }

            if (usingVive || usingIndex)
            {
                File.AppendAllText("VRTweaksLog.txt", "Recentering OpenVR" + Environment.NewLine);
                Valve.VR.OpenVR.System.ResetSeatedZeroPose();
                Valve.VR.OpenVR.Compositor.SetTrackingSpace(Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);
                return;
            }
        }
        //Auto detect device so i can choose controller layout
        public static void InitializeMainDevice()
        {
            InputDevices.GetDevices(vrDevices);
            foreach (var inputDevice in vrDevices)
            {
                if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
                {
                    if (inputDevice.manufacturer.Contains("Oculus") || inputDevice.manufacturer.Contains("Quest") && inputDevice.isValid)
                    {
                        usingOculus = true;
                    }
                    else if (inputDevice.manufacturer.Contains("Vive") && inputDevice.isValid)
                    {
                        usingVive = true;
                    }
                    else if (inputDevice.manufacturer.Contains("Index") && inputDevice.isValid)
                    {
                        usingIndex = true;
                    }
                    else
                    {
                        usingOculus = false;
                        usingVive = false;
                        usingIndex = false;
                    }
                }

                /*File.AppendAllText("Logs/VrInput.txt", "Name: " + inputDevice.name + Environment.NewLine);
                File.AppendAllText("Logs/VrInput.txt", "Characteristics: " + inputDevice.characteristics + Environment.NewLine);
                File.AppendAllText("Logs/VrInput.txt", "Manufacturer: " + inputDevice.manufacturer + Environment.NewLine);
                File.AppendAllText("Logs/VrInput.txt", "Subsystem: " + inputDevice.subsystem + Environment.NewLine);
                File.AppendAllText("Logs/VrInput.txt", "SerialNumber: " + inputDevice.serialNumber + Environment.NewLine);
                File.AppendAllText("Logs/VrInput.txt", "isValid: " + inputDevice.isValid + Environment.NewLine);
                File.AppendAllText("Logs/VrInput.txt", "        " + Environment.NewLine);*/
            }

        }
    }
}