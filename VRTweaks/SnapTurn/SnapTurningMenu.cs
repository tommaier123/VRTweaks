using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

public static class SnapTurningMenu
{
    private static Harmony _harmony;
    private static int _tabIndex;

    public static void Patch()
    {
        if (XRSettings.enabled)
        {
            _harmony = new Harmony("com.whotnt.subnautica.vrenhancements.mod");
            _harmony.Patch(AccessTools.Method(typeof(uGUI_OptionsPanel), "AddGeneralTab", null, null), null, new HarmonyMethod(typeof(SnapTurningMenu).GetMethod("AddGeneralTab_Postfix")), null);
            _harmony.Patch(AccessTools.Method(typeof(uGUI_TabbedControlsPanel), "AddTab", null, null), null, new HarmonyMethod(typeof(SnapTurningMenu).GetMethod("AddTab_Postfix")), null);
            _harmony.Patch(AccessTools.Method(typeof(GameSettings), "SerializeVRSettings", null, null), null, new HarmonyMethod(typeof(SnapTurningMenu).GetMethod("SerializeVRSettings_Postfix")), null);
        }
    }

    public static void AddGeneralTab_Postfix(uGUI_OptionsPanel __instance)
    {
        __instance.AddHeading(_tabIndex, "Snap Turning");//add new heading under the General Tab
        __instance.AddToggleOption(_tabIndex, "Enable Snap Turning", SnapTurningOptions.EnableSnapTurning, (bool v) => SnapTurningOptions.EnableSnapTurning = v);
        __instance.AddChoiceOption(_tabIndex, "Snap Angle", SnapTurningOptions.SnapAngleChoices, SnapTurningOptions.SnapAngleChoiceIndex, (int index) => SnapTurningOptions.SnapAngleChoiceIndex = index);
        __instance.AddBindingOption(_tabIndex, "Keyboard Turn Left", GameInput.Device.Keyboard, GameInput.Button.LookLeft);
        __instance.AddBindingOption(_tabIndex, "Keyboard Turn Right", GameInput.Device.Keyboard, GameInput.Button.LookRight);
        __instance.AddToggleOption(_tabIndex, "Disable Mouse Look", SnapTurningOptions.DisableMouseLook, (bool v) => SnapTurningOptions.DisableMouseLook = v);
    }

    public static void AddTab_Postfix(int __result, string label)
    {
        //get the tabIndex of the general tab to be able to use it in  AddGeneralTab_Postfix
        if (label.Equals("General"))
            _tabIndex = __result;
    }

    public static void SerializeVRSettings_Postfix(GameSettings.ISerializer serializer)
    {
        SnapTurningOptions.EnableSnapTurning = serializer.Serialize("VR/EnableSnapTurning", SnapTurningOptions.EnableSnapTurning);
        SnapTurningOptions.SnapAngleChoiceIndex = serializer.Serialize("VR/SnapAngleChoiceIndex", SnapTurningOptions.SnapAngleChoiceIndex);
    }
}

