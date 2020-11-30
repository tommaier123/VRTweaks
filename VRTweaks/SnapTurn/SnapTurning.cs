using HarmonyLib;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using UnityEngine;
using UnityEngine.XR;

namespace VRTweaks.SnapTurn
{
    [HarmonyPatch(typeof(MainCameraControl))]
    [HarmonyPatch("Update")]
    public static class SnapTurning
    {
        private static float SnapAngle => Config.SnapAngles[Config.SnapAngleChoiceIndex];
        private static bool _didLookRight;
        private static bool _didLookLeft;
        private static bool _isLookingLeft;
        private static bool _isLookingRight;
        private static bool _isLookingUpOrDown;
        private static bool _isLookingLeftOrRight;
        private static bool _shouldSnapTurn;

        public static void Initialize()
        {
            Config.Load();
            OptionsPanelHandler.RegisterModOptions(new OptionsMenu());
        }

        [HarmonyPrefix]
        public static bool Prefix()
        {
            if (!Config.EnableSnapTurning)
            {
                return true; //Enter vanilla method
            }

            UpdateFields();

            if (_isLookingUpOrDown)
            {
                return false; //Disable looking up or down with the joystick
            }

            if (_shouldSnapTurn)
            {
                UpdatePlayerRotation();
                return false; //Don't enter vanilla method if we snap turn
            }

            return true;
        }

        private static void UpdateFields()
        {
            _didLookRight = GameInput.GetButtonDown(GameInput.Button.LookRight) || KeyCodeUtils.GetKeyDown(Config.KeybindKeyRight);
            _didLookLeft = GameInput.GetButtonDown(GameInput.Button.LookLeft) || KeyCodeUtils.GetKeyDown(Config.KeybindKeyLeft);
            _isLookingRight = GameInput.GetButtonHeld(GameInput.Button.LookRight) || KeyCodeUtils.GetKeyHeld(Config.KeybindKeyRight);
            _isLookingLeft = GameInput.GetButtonHeld(GameInput.Button.LookLeft) || KeyCodeUtils.GetKeyHeld(Config.KeybindKeyLeft);
            _isLookingLeftOrRight = _didLookLeft || _didLookRight || _isLookingLeft || _isLookingRight;
            _shouldSnapTurn = XRSettings.enabled && _isLookingLeftOrRight;
            _isLookingUpOrDown = GameInput.GetButtonDown(GameInput.Button.LookUp)
                || GameInput.GetButtonDown(GameInput.Button.LookDown)
                || GameInput.GetButtonHeld(GameInput.Button.LookUp)
                || GameInput.GetButtonHeld(GameInput.Button.LookDown);
        }

        private static void UpdatePlayerRotation()
        {
            Player.main.transform.localRotation = Quaternion.Euler(GetNewEulerAngles());
        }

        private static Vector3 GetNewEulerAngles()
        {
            var newEulerAngles = Player.main.transform.localRotation.eulerAngles;

            if (_didLookRight)
            {
                newEulerAngles.y += SnapAngle;
            }
            else if (_didLookLeft)
            {
                newEulerAngles.y -= SnapAngle;
            }

            return newEulerAngles;
        }
    }
}
