using HarmonyLib;
using UnityEngine.XR;

public static class SnapTurningMenu
{
    private static Harmony harmony;
    private static int tabIndex;

    public static void Patch()
    {
        if (XRSettings.enabled)
        {
            harmony = new Harmony("com.whotnt.subnautica.vrenhancements.mod");
            harmony.Patch(AccessTools.Method(typeof(uGUI_OptionsPanel), "AddGeneralTab", null, null), null, new HarmonyMethod(typeof(SnapTurningMenu).GetMethod("AddGeneralTab_Postfix")), null);
            harmony.Patch(AccessTools.Method(typeof(uGUI_TabbedControlsPanel), "AddTab", null, null), null, new HarmonyMethod(typeof(SnapTurningMenu).GetMethod("AddTab_Postfix")), null);
            harmony.Patch(AccessTools.Method(typeof(GameSettings), "SerializeVRSettings", null, null), null, new HarmonyMethod(typeof(SnapTurningMenu).GetMethod("SerializeVRSettings_Postfix")), null);
        }
    }

    public static void AddGeneralTab_Postfix(uGUI_OptionsPanel __instance)
    {
        __instance.AddHeading(tabIndex, "Snap Turning");//add new heading under the General Tab
        __instance.AddToggleOption(tabIndex, "Enable Snap Turning", SnapTurningOptions.EnableSnapTurning, (bool v) => SnapTurningOptions.EnableSnapTurning = v);
        __instance.AddChoiceOption(tabIndex, "Snap Angle", SnapTurningOptions.SnapAngleChoices, SnapTurningOptions.SnapAngleChoiceIndex, (int index) => SnapTurningOptions.SnapAngleChoiceIndex = index);
    }

    public static void AddTab_Postfix(int __result, string label)
    {
        //get the tabIndex of the general tab to be able to use it in  AddGeneralTab_Postfix
        if (label.Equals("General"))
            tabIndex = __result;
    }

    public static void SerializeVRSettings_Postfix(GameSettings.ISerializer serializer)
    {
        SnapTurningOptions.EnableSnapTurning = serializer.Serialize("VR/EnableSnapTurning", SnapTurningOptions.EnableSnapTurning);
        SnapTurningOptions.SnapAngleChoiceIndex = serializer.Serialize("VR/SnapAngleChoiceIndex", SnapTurningOptions.SnapAngleChoiceIndex);
    }
}

