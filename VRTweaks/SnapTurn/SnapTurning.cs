using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace VRTweaks.SnapTurn
{
    [HarmonyPatch(typeof(MainCameraControl), "Update")]
    public static class SnapTurning
    {
        private static float SnapAngle => SnapTurningOptions.SnapAngles[SnapTurningOptions.SnapAngleChoiceIndex];
        private static bool _didLookRight;
        private static bool _didLookLeft;
        private static bool _isLookingUpOrDown;
        private static bool _isLookingLeftOrRight;
        private static bool _shouldSnapTurn;


        [HarmonyPrefix]
        public static bool Prefix(MainCameraControl __instance, out Vector3 __state)
        {
            __state = __instance.viewModel.transform.localPosition;

            if (!SnapTurningOptions.EnableSnapTurning || Player.main.isPiloting)
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
            var lookDelta = GameInput.GetLookDelta();

            _isLookingUpOrDown = Mathf.Abs(lookDelta.y) > Mathf.Abs(lookDelta.x);
            _isLookingLeftOrRight = Mathf.Abs(lookDelta.x) > Mathf.Abs(lookDelta.y);

            _didLookRight = !_isLookingUpOrDown && (GameInput.GetButtonDown(GameInput.Button.LookRight) || GameInput.GetKeyDown(SnapTurningOptions.KeybindKeyRight));
            _didLookLeft = !_isLookingUpOrDown && (GameInput.GetButtonDown(GameInput.Button.LookLeft) || GameInput.GetKeyDown(SnapTurningOptions.KeybindKeyLeft));

            _shouldSnapTurn = XRSettings.enabled && _isLookingLeftOrRight;
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

        [HarmonyPostfix]
        public static void Postfix(MainCameraControl __instance, Vector3 __state)
        {
            __instance.viewModel.transform.localPosition = __state;
        }
    }
}
