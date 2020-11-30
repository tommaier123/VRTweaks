using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace VRTweaks.SnapTurn
{
    public static class Config
    {
        public static bool EnableSnapTurning = true;
        public static int SnapAngleChoiceIndex = 0;
        public static float[] SnapAngles = { 45, 90, 22.5f };
        public static KeyCode KeybindKeyLeft;
        public static KeyCode KeybindKeyRight;

        public static void Load()
        {
            EnableSnapTurning = PlayerPrefsExtra.GetBool(Options.PLAYER_PREF_KEY_TOGGLE_SNAP_TURNING, true);
            SnapAngleChoiceIndex = GetSnapAngleChoiceIndex(SnapType.Default);
            KeybindKeyLeft = PlayerPrefsExtra.GetKeyCode("SMLHelperExampleModKeybindLeft", KeyCode.LeftArrow);
            KeybindKeyRight = PlayerPrefsExtra.GetKeyCode("SMLHelperExampleModKeybindRight", KeyCode.RightArrow);
        }

        private static int GetSnapAngleChoiceIndex(SnapType snapType)
        {
            int result = GetChoiceIndexForSnapType(snapType);
            if (result > SnapAngles.Length)
            {
                result = 0;
            }

            return result;
        }

        private static int GetChoiceIndexForSnapType(SnapType snapType)
        {
            int result = 0;
            if (snapType == SnapType.Default)
            {
                result = PlayerPrefs.GetInt(Options.PLAYER_PREF_KEY_SNAP_ANGLE, 0);
            }

            return result;
        }
    }

    public enum SnapType
    {
        Default,
        Seamoth,
        Prawn
    }
}
