
using HarmonyLib;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

namespace VRTweaks
{
    [HarmonyPatch(typeof(GameOptions), "GetVrAnimationMode")]
    public static class VRAnimationsFixer
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            //Disables VRAnimations in order to fix cutscenes being skipped and seatruck controls not working
            __result = false;
        }
    }

  /*  [HarmonyPatch(typeof(MapRoomFunctionality), "Update")]
    public static class Test
    {
        [HarmonyPrefix]
        public static bool Prefix(MapRoomFunctionality __instance)
        {
            if (__instance.hologramRoot != null)
            {
                foreach (MeshRenderer test in __instance.hologramRoot.GetAllComponentsInChildren<MeshRenderer>())
                {
                    // File.AppendAllText("Logs/MeshRenderer.txt", test.name + Environment.NewLine);
                    foreach (Material mat in test.materials)
                    {
                        if (mat.name.Contains("wireframemap"))
                        {
                            if (mat.GetShaderPassEnabled("FX/WBOIT"))
                            {
                                mat.SetShaderPassEnabled("FX/WBOIT", false);
                               
                                 //File.AppendAllText("Logs/mats.txt", "Name: " + mat.name + " , ShaderName: " + mat.name + " , ShaderKeys: " + String.Join(", ", mat.shaderKeywords)  + ", WBOITEnabled: "  + String.Join(", ", mat.GetShaderPassEnabled("FX/WBOIT") + Environment.NewLine));
                            }
                        }
                    }
                    foreach(Material ma in test.sharedMaterials)
                    {
                        ma.DisableKeyword("WBOIT");
                       // File.AppendAllText("Logs/mats1.txt", "Name: " + ma.name + " , ShaderName: " + ma.name + " , ShaderKeys: " + String.Join(", ", ma.shaderKeywords) + ", WBOITEnabled: " + String.Join(", ", ma.GetShaderPassEnabled("FX/WBOIT") + Environment.NewLine));
                    }

                }
            }
           /* MeshRenderer[] holo = __instance.hologramRoot.GetAllComponentsInChildren<MeshRenderer>();
            Transform[] trans = __instance.hologramRoot.GetAllComponentsInChildren<Transform>();
            foreach (MeshRenderer h in holo)
            {
                foreach (Material ma in h.sharedMaterials)
                {
                    //File.AppendAllText("Logs/Meshes.txt", ma.name + " " + String.Join(", ", ma.shaderKeywords) + trans.Length + Environment.NewLine);
                    foreach (Transform t in trans)
                    {
                        foreach (MeshRenderer mesh in t.GetAllComponentsInChildren<MeshRenderer>())
                        {
                            foreach (Material mat in mesh.materials)
                            {
                                foreach (var key in mat.shaderKeywords)
                                {
                                    if (key.Contains("WBOIT"))
                                    {
                                        File.AppendAllText("Logs/MapRoom.txt", mat.name + " " + String.Join(", ", mat.shaderKeywords) + Environment.NewLine);
                                    }
                                }
                            }
                        }
                    }
                    /* foreach (Material mat in h.materials)
                     {
                         foreach (var key in mat.shaderKeywords)
                         {
                             if (key.Contains("WBOIT"))
                             {
                                 File.AppendAllText("Logs/MapRoom.txt", mat.name + " " + String.Join(", ", mat.shaderKeywords) + Environment.NewLine);
                             }
                         }
                     }
                }
            }
            return true;
        }
    }*/

}

