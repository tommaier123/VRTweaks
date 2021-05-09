using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace VRTweaks
{
    [HarmonyPatch(typeof(WBOIT))]
    [HarmonyPatch("CreateRenderTargets")]
    internal class CreateRenderTargets_Patch
    {
        public static RenderTexture wboitTexture0;
        public static RenderTexture wboitTexture1;
        public static RenderTexture wboitTexture2;

        public static RenderBuffer[] colorBuffers;

        public static bool Prefix(WBOIT __instance)
        {
            Material compositeMaterial = Traverse.Create(__instance).Field("compositeMaterial").GetValue() as Material;
            Camera camera = Traverse.Create(__instance).Field("camera").GetValue() as Camera;
            int texAPropertyID = (int)Traverse.Create(__instance).Field("texAPropertyID").GetValue();
            int texBPropertyID = (int)Traverse.Create(__instance).Field("texBPropertyID").GetValue();

            wboitTexture0 = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 24, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
            wboitTexture0.name = "WBOIT Tex0";
            Traverse.Create(__instance).Field("wboitTexture0").SetValue(wboitTexture0);

            wboitTexture1 = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 24, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
            wboitTexture1.name = "WBOIT TexA";
            Traverse.Create(__instance).Field("wboitTexture1").SetValue(wboitTexture1);

            wboitTexture2 = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 0, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
            wboitTexture2.name = "WBOIT TexB";
            Traverse.Create(__instance).Field("wboitTexture2").SetValue(wboitTexture2);

            compositeMaterial.SetTexture(texAPropertyID, wboitTexture1);

            compositeMaterial.SetTexture(texBPropertyID, wboitTexture2);

            colorBuffers = new RenderBuffer[]
            {
                wboitTexture0.colorBuffer,
                wboitTexture1.colorBuffer,
                wboitTexture2.colorBuffer
            };
            Traverse.Create(__instance).Field("colorBuffers").SetValue(colorBuffers);

            return false;
        }
    }
}
