using HarmonyLib;

namespace VRTweaks
{
    class MiscVrOptions
    {

        static int generalTabIndex = 0;

        [HarmonyPatch(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.AddGeneralTab))]
        class uGUI_OptionsPanel_VROptionsPatch
        {
            static void Postfix(uGUI_OptionsPanel __instance)
            {
                __instance.AddHeading(generalTabIndex, "Misc VR Options");
                __instance.AddToggleOption(generalTabIndex, "Disable Y-Axis Input", VROptions.disableInputPitch, (bool v) => VROptions.disableInputPitch = v);
                __instance.AddSliderOption(generalTabIndex, "Ground Speed Scale", VROptions.groundMoveScale, 0.6f, (float v) => {
                    if (v <= 0f)
                    {
                        // Never allow 0 scale speed (immobile)
                        v = .05f;
                    }
                    VROptions.groundMoveScale = v;

                }
                );
            }

        }

        [HarmonyPatch(typeof(uGUI_TabbedControlsPanel), nameof(uGUI_TabbedControlsPanel.AddTab))]
        class uGUI_TabbedControlsPanel_GetGeneralTabPatch
        {
            static void Postfix(int __result, string label)
            {
                if (label.Equals("General"))
                    generalTabIndex = __result;
            }
        }

        [HarmonyPatch(typeof(GameSettings), nameof(GameSettings.SerializeVRSettings))]
        class GameSettings_SerializeVRSettings_Patch
        {
            static void Postfix(GameSettings.ISerializer serializer)
            {
                VROptions.disableInputPitch = serializer.Serialize("VR/DisableInputPitch", VROptions.disableInputPitch);
                VROptions.groundMoveScale = serializer.Serialize("VR/GroundMoveScale", VROptions.groundMoveScale);
            }
        }

    }
}
