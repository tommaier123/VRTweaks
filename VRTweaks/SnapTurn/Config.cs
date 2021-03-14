using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using UnityEngine;

namespace VRTweaks.SnapTurn
{
    [Menu("Snap Turning", SaveOn = MenuAttribute.SaveEvents.ChangeValue|MenuAttribute.SaveEvents.SaveGame|MenuAttribute.SaveEvents.QuitGame, LoadOn = MenuAttribute.LoadEvents.MenuRegistered|MenuAttribute.LoadEvents.MenuOpened)]
    public class Config: ConfigFile
    {
        public static Config instance;
        internal static float[] SnapAngles = { 22.5f, 45f, 90f };

        public Config()
        {
            instance = this;
        }

        [Toggle("Enabled")]
        public bool EnableSnapTurning = true;

        [Toggle("Unlock Controller Virtical Look")]
        public bool EnableVirticalLook = false;

        [Choice("Angle", options: new string[] {  "22.5", "45", "90" })]
        public int SnapAngleChoiceIndex = 1;

        [Keybind("Keyboard Left")]
        public KeyCode KeybindKeyLeft = KeyCode.LeftArrow;

        [Keybind("Keyboard Right")]
        public KeyCode KeybindKeyRight = KeyCode.RightArrow;

    }
}
