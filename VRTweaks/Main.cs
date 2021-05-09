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
            File.AppendAllText("VRTweaksLog.txt", "Initializing" + Environment.NewLine);

            new GameObject("_VRTweaks").AddComponent<VRTweaks>();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly() ,"VRTweaks");

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
            RemoveComponents();
            yield break;
        }

        internal void Update()
        {

            if (Input.GetKeyDown(KeyCode.T))
            {
                Recenter();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                RemoveComponents();
            }
        }

        public static void Recenter()
        {
            if (XRSettings.loadedDeviceName == "Oculus")
            {
                File.AppendAllText("VRTweaksLog.txt", "Recentering Oculus" + Environment.NewLine);
                InputTracking.Recenter();
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

        public static void RemoveComponents()
        {

            FindObjectsOfType<WBOIT>()?.ForEach((w) => w.enabled = false);

            FindObjectsOfType<PlayerMask>()?.ForEach((m) =>
            {
                m.enabled = false;
                m.gameObject.SetActive(false);
                m.GetAllComponentsInChildren<MeshFilter>()?.ForEach((f) => f.mesh = null);
            });

            /*
            foreach (GameObject m in FindObjectsOfType(typeof(GameObject)) as GameObject[])
            {
                if (m.name.Equals("airsack_fish_geo"))
                {
                    foreach (SkinnedMeshRenderer r in m.GetAllComponentsInChildren<SkinnedMeshRenderer>())
                    {
                        foreach (Material mat in r.materials)
                        {
                            if (mat.shaderKeywords.Where(x => x.Equals("WBOIT")).Count() > 0)
                            {
                                mat.DisableKeyword("WBOIT");
                                File.AppendAllText("VRTweaksLog.txt", "Shader Keyword Disabled" + Environment.NewLine);
                            }
                        }
                    }
                }
            }
            
            foreach (Material m in FindObjectsOfType(typeof(Material)) as Material[])
            {
                m.DisableKeyword("WBOIT");
                File.AppendAllText("VRTweaksLog.txt", m.name + " " + String.Join(", ", m.shaderKeywords) + Environment.NewLine);
            }
            
            Shader.DisableKeyword("WBOIT");
            */
        }

    }
}
