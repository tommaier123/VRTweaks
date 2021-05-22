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
        public static bool Prefix(WBOIT __instance)
        {
            __instance.wboitTexture1 = new RenderTexture(__instance.camera.pixelWidth, __instance.camera.pixelHeight, 24, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            __instance.wboitTexture1.name = "WBOIT TexA";

            __instance.wboitTexture2 = new RenderTexture(__instance.camera.pixelWidth, __instance.camera.pixelHeight, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            __instance.wboitTexture2.name = "WBOIT TexB";

            __instance.compositeMaterial.SetTexture(__instance.texAPropertyID, __instance.wboitTexture1);
            __instance.compositeMaterial.SetTexture(__instance.texBPropertyID, __instance.wboitTexture2);
            __instance.colorBuffers = new RenderBuffer[]
            {
                __instance.wboitTexture1.colorBuffer,
                __instance.wboitTexture1.colorBuffer,
                __instance.wboitTexture2.colorBuffer
            };

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
