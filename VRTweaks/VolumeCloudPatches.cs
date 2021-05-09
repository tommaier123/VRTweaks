using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using Yangrc.VolumeCloud;

/*namespace VRTweaks
{

    // Call the Stereoscopic versions of these GetProjectionExtents functions instead.
    [HarmonyPatch(typeof(CameraExtension), nameof(CameraExtension.GetProjectionExtents),
        new Type[] { typeof(Camera), typeof(float), typeof(float) })]
    class GetProjectionExtentsPatch0
    {
        static bool Prefix(Camera camera, float texelOffsetX, float texelOffsetY, ref Vector4 __result)
        {
            if (camera == null)
            {
                return true;
            }
            Camera.StereoscopicEye activeEye = camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left ?
                Camera.StereoscopicEye.Left : Camera.StereoscopicEye.Right;
            __result = CameraExtension.GetProjectionExtents(camera, activeEye, texelOffsetX, texelOffsetY);
            return false;
        }
    }

    [HarmonyPatch(typeof(CameraExtension), nameof(CameraExtension.GetProjectionExtents),
    new Type[] { typeof(Camera) })]
    class GetProjectionExtentsPatch1
    {
        static bool Prefix(Camera camera, ref Vector4 __result)
        {
            if (camera == null)
            {
                return true;
            }
            Camera.StereoscopicEye activeEye = camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left ?
                Camera.StereoscopicEye.Left : Camera.StereoscopicEye.Right;
            __result = CameraExtension.GetProjectionExtents(camera, activeEye);
            return false;
        }
    }

    /*
    [HarmonyPatch]
    class VolumeCloudRenderer_EnsureMaterial_ReversePatch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(VolumeCloudRenderer), nameof(VolumeCloudRenderer.EnsureMaterial), new Type[] { typeof(bool) })]
        public static void Run(object instance, bool force)
        {
            throw new NotImplementedException("Stub for reverse patch.");
        }
    }

    [HarmonyPatch]
    class VolumeCloudRenderer_GetWidth_ReversePatch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(VolumeCloudRenderer), nameof(VolumeCloudRenderer.GetWidth))]
        public static int Run(object instance)
        {
            throw new NotImplementedException("Stub for reverse patch.");
        }
    }

    [HarmonyPatch]
    class VolumeCloudRenderer_GetHeight_ReversePatch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(VolumeCloudRenderer), nameof(VolumeCloudRenderer.GetHeight))]
        public static int Run(object instance)
        {
            throw new NotImplementedException("Stub for reverse patch.");
        }
    }///////////////////////////////////////////

    [HarmonyPatch(typeof(VolumeCloudRenderer), nameof(VolumeCloudRenderer.RenderFrame))]
    class RenderFrameReplacement
    {

        private static MethodInfo getProjectionMatrix = AccessTools.Method(typeof(Camera), "get_projectionMatrix");
        private static FieldInfo cameraField = AccessTools.Field(typeof(VolumeCloudRenderer), "mcam");

        private static Matrix4x4 GetPatchedCameraMatrixProjection(Camera camera)
        {
            // identity seems to produce a working image.
            // I originally tried Camera.GetStereoCameraMatrixProjection does not work.
            // It's possible there is something more approrpriate to use here.
            return Matrix4x4.identity;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var original = new List<CodeInstruction>(instructions);
            var patched = new List<CodeInstruction>();
            for (int i = 0; i < original.Count; i++)
            {
                var instruction = original[i];
                // If the current instruction is the call to Camera.projectionField, replace it patched function
                if (instruction.Calls(getProjectionMatrix))
                {
                    patched.Add(CodeInstruction.Call(typeof(RenderFrameReplacement), nameof(GetPatchedCameraMatrixProjection)));
                }
                else
                {
                    patched.Add(instruction);
                }
            }
            return patched;
        }

        // Temporarily keeping this block of code for debug. It is a full replacement of the original source.
        /*
        static bool Prefix(VolumeCloudRenderer __instance, RenderTexture source, RenderTexture destination, ref float ___updateTimer, ref int ___forceOutOfBound, ref bool ___qualityChanged,
            ref Material ___blitMat, ref RenderTexture[] ___fullBuffer, ref int ___fullBufferIndex, ref RenderTexture ___lowresBuffer, ref VolumeCloudRenderer.HaltonSequence ___haltonSequence,
            ref int ___frameIndex, ref int[,] ___offset, ref Camera ___mcam, ref Matrix4x4 ___prevV
            )
        {
            ___updateTimer = 0f;
            Camera.StereoscopicEye activeEye = ___mcam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left ?
                Camera.StereoscopicEye.Left : Camera.StereoscopicEye.Right;
            if (___qualityChanged)
            {
                ___forceOutOfBound = 1;
            }
            if (!(__instance.configuration != null) || !(___blitMat != null))
            {
                Graphics.Blit(source, destination);
            }
            else
            {
                VolumeCloudRenderer_EnsureMaterial_ReversePatch.Run(__instance, ___qualityChanged);
                if (!__instance.renderToSpheremap)
                {
                    ___blitMat.DisableKeyword("CLOUDS_TO_SPHEREMAP");
                }
                else
                {
                    ___blitMat.EnableKeyword("CLOUDS_TO_SPHEREMAP");
                }
                int width = VolumeCloudRenderer_GetWidth_ReversePatch.Run(__instance);
                int height = VolumeCloudRenderer_GetHeight_ReversePatch.Run(__instance);
                __instance.EnsureArray<RenderTexture>(ref ___fullBuffer, 2, null);
                __instance.EnsureRenderTarget(ref ___fullBuffer[0], width, height, RenderTextureFormat.ARGBHalf, FilterMode.Bilinear, 0, 1);
                __instance.EnsureRenderTarget(ref ___fullBuffer[1], width, height, RenderTextureFormat.ARGBHalf, FilterMode.Bilinear, 0, 1);
                __instance.EnsureRenderTarget(ref ___lowresBuffer, width / 4, height / 4, RenderTextureFormat.ARGBHalf, FilterMode.Bilinear, 0, 1);
                ___frameIndex = (___frameIndex + 1) % 16;
                ___fullBufferIndex = (___fullBufferIndex + 1) % 2;
                float single = (float)___offset[___frameIndex, 0];
                float single1 = (float)___offset[___frameIndex, 1];
                __instance.configuration.ApplyToMaterial(___blitMat);
                if (__instance.heroCloudTransform == null)
                {
                    ___blitMat.DisableKeyword("HERO_CLOUD");
                }
                else
                {
                    Vector4 vector4 = new Vector4(__instance.heroCloudTransform.position.x, __instance.heroCloudTransform.position.z, __instance.heroCloudTransform.localScale.x, __instance.heroCloudTransform.localScale.z);
                    ___blitMat.SetVector(ShaderPropertyID._HeroCloudPos, vector4);
                    ___blitMat.SetTexture(ShaderPropertyID._HeroCloudMask, __instance.heroCloudMask);
                    ___blitMat.SetFloat(ShaderPropertyID._HeroCloudIntensity, __instance.herocloudIntensity);
                    ___blitMat.EnableKeyword("HERO_CLOUD");
                }
                ___blitMat.SetInt(ShaderPropertyID._ForceOutOfBound, ___forceOutOfBound);
                ___forceOutOfBound = 0;
                if (!__instance.renderToSpheremap)
                {
                    ___blitMat.SetVector(ShaderPropertyID._CloudsProjectionExtents, ___mcam.GetProjectionExtents(activeEye, single * (float)(1 << (0 & 31)), single1 * (float)(1 << (0 & 31))));
                }
                else
                {
                    ___blitMat.SetVector(ShaderPropertyID._CloudsProjectionExtents, new Vector4(1f, 1f, single / (float)___lowresBuffer.width, single1 / (float)___lowresBuffer.width));
                }
                if (___haltonSequence == null)
                {
                    ___haltonSequence = new VolumeCloudRenderer.HaltonSequence()
                    {
                        radix = 3
                    };
                }
                ___blitMat.SetFloat(ShaderPropertyID._CloudsRaymarchOffset, ___haltonSequence.Get());
                ___blitMat.SetVector("_TexelSize", ___lowresBuffer.texelSize);
                Graphics.Blit(null, ___lowresBuffer, ___blitMat, 0);
                ___blitMat.SetVector(ShaderPropertyID._CloudsJitter, new Vector2(single, single1));
                ___blitMat.SetTexture(ShaderPropertyID._LowresCloudTex, ___lowresBuffer);
                if (!__instance.renderToSpheremap)
                {
                    //___blitMat.SetMatrix(ShaderPropertyID._CloudsPrevVP, GL.GetGPUProjectionMatrix(___mcam.GetStereoProjectionMatrix(activeEye), false) * ___prevV);
                    ___blitMat.SetVector(ShaderPropertyID._CloudsProjectionExtents, ___mcam.GetProjectionExtents(activeEye));
                }
                else
                {
                    ___blitMat.SetMatrix(ShaderPropertyID._CloudsPrevVP, GL.GetGPUProjectionMatrix(Matrix4x4.identity, false) * ___prevV);
                    ___blitMat.SetVector(ShaderPropertyID._CloudsProjectionExtents, new Vector4(1f, 1f, single / (float)___lowresBuffer.width, single1 / (float)___lowresBuffer.width));
                }
                Graphics.Blit(___fullBuffer[___fullBufferIndex], ___fullBuffer[___fullBufferIndex ^ 1], ___blitMat, 1);
                if (!__instance.renderToSpheremap)
                {
                    Shader.SetGlobalTexture(ShaderPropertyID._CloudTex, ___fullBuffer[___fullBufferIndex ^ 1]);
                }
                else
                {
                    if (uSkyManager.main)
                    {
                        uSkyManager.main.SetConstantMaterialProperties(___blitMat);
                        uSkyManager.main.SetVaryingMaterialProperties(___blitMat);
                    }
                    ___blitMat.SetVector(ShaderPropertyID._CloudsProjectionExtents, new Vector4(1f, 1f, 0f, 0f));
                    ___blitMat.SetTexture(ShaderPropertyID._CloudTex, ___fullBuffer[___fullBufferIndex ^ 1]);
                }
                Graphics.Blit(source, destination, ___blitMat, 2);
                if (!__instance.renderToSpheremap)
                {
                    ___prevV = ___mcam.worldToCameraMatrix;
                }
                else
                {
                    ___prevV = Matrix4x4.identity;
                    Shader.SetGlobalTexture(ShaderPropertyID._SkyMap, destination);
                }
            }
            ___qualityChanged = false;
            return false;
        }

    }
}*/