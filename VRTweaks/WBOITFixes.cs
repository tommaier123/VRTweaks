using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace VRTweaks
{
    [HarmonyPatch(typeof(WBOIT))]
    [HarmonyPatch("CreateRenderTargets")]
    internal class CreateRenderTargets_Patch
    {
        public static RenderTexture wboitTexture1;
        public static RenderTexture wboitTexture2;

        public static RenderBuffer[] colorBuffers;

        public static bool Prefix(WBOIT __instance)
        {
            Material compositeMaterial = Traverse.Create(__instance).Field("compositeMaterial").GetValue() as Material;
            Camera camera = Traverse.Create(__instance).Field("camera").GetValue() as Camera;
            int texAPropertyID = (int)Traverse.Create(__instance).Field("texAPropertyID").GetValue();
            int texBPropertyID = (int)Traverse.Create(__instance).Field("texBPropertyID").GetValue();

            wboitTexture1 = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 24, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            wboitTexture1.name = "WBOIT TexA";
            Traverse.Create(__instance).Field("wboitTexture1").SetValue(wboitTexture1);

            wboitTexture2 = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            wboitTexture2.name = "WBOIT TexB";
            Traverse.Create(__instance).Field("wboitTexture2").SetValue(wboitTexture2);

            compositeMaterial.SetTexture(texAPropertyID, wboitTexture1);
            compositeMaterial.SetTexture(texBPropertyID, wboitTexture2);
            colorBuffers = new RenderBuffer[]
            {
                wboitTexture1.colorBuffer,
                wboitTexture1.colorBuffer,
                wboitTexture2.colorBuffer
            };
            Traverse.Create(__instance).Field("colorBuffers").SetValue(colorBuffers);

            return false;
        }
    }

    [HarmonyPatch(typeof(WBOIT))]
    [HarmonyPatch(nameof(WBOIT.VerifyRenderTargets))]
    internal class VerifyRenderTargets_Patch
    {
        private static MethodInfo screenGetWidth = AccessTools.Method(typeof(Screen), "get_width");
        private static MethodInfo screenGetHeight = AccessTools.Method(typeof(Screen), "get_height");

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var original = new List<CodeInstruction>(instructions);
            var patched = new List<CodeInstruction>();
            for (int i = 0; i < original.Count; i++)
            {
                var instruction = original[i];
                if (instruction.Calls(screenGetHeight))
                {
                    patched.Add(new CodeInstruction(OpCodes.Ldarg_0));
                    patched.Add(CodeInstruction.LoadField(typeof(WBOIT), "camera"));
                    patched.Add(CodeInstruction.Call(typeof(Camera), "get_pixelHeight"));
                }
                else if (instruction.Calls(screenGetWidth))
                {
                    patched.Add(new CodeInstruction(OpCodes.Ldarg_0));
                    patched.Add(CodeInstruction.LoadField(typeof(WBOIT), "camera"));
                    patched.Add(CodeInstruction.Call(typeof(Camera), "get_pixelWidth"));
                }
                else
                {
                    patched.Add(instruction);
                }
            }
            return patched;

        }
    }
}
