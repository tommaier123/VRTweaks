using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using HarmonyLib;
using System;
using Platform.Utils;
using FMODUnity;
using UnityEngine.Events;

namespace VRTweaks
{
    enum Controller
    {
        Left,
        Right
    }

    internal class XRInputManager
    {
        private static readonly XRInputManager _instance = new XRInputManager();
        private readonly List<InputDevice> xrDevices = new List<InputDevice>();
        public InputDevice leftController;
        public InputDevice rightController;


        private XRInputManager()
        {
            GetDevices();
        }

        public static XRInputManager GetXRInputManager()
        {
            return _instance;
        }

        void GetDevices()
        {
            InputDevices.GetDevices(xrDevices);

            foreach (InputDevice device in xrDevices)
            {
                if (device.name.Contains("Left"))
                {
                    leftController = device;
                }
                if (device.name.Contains("Right"))
                {
                    rightController = device;
                }
            }
        }

        InputDevice GetDevice(Controller name)
        {
            switch (name)
            {
                case Controller.Left:
                    return leftController;
                case Controller.Right:
                    return rightController;
                default: throw new Exception();
            }
        }

        public Vector2 Get(Controller controller, InputFeatureUsage<Vector2> usage)
        {
            InputDevice device = GetDevice(controller);
            Vector2 value = Vector2.zero;
            if (device != null && device.isValid)
            {
                device.TryGetFeatureValue(usage, out value);
            }
            else
            {
                GetDevices();
            }
            return value;
        }

        public Vector3 Get(Controller controller, InputFeatureUsage<Vector3> usage)
        {
            InputDevice device = GetDevice(controller);
            Vector3 value = Vector3.zero;
            if (device != null && device.isValid)
            {
                device.TryGetFeatureValue(usage, out value);
            }
            else
            {
                GetDevices();
            }
            return value;
        }

        public Quaternion Get(Controller controller, InputFeatureUsage<Quaternion> usage)
        {
            InputDevice device = GetDevice(controller);
            Quaternion value = Quaternion.identity;
            if (device != null && device.isValid)
            {
                device.TryGetFeatureValue(usage, out value);
            }
            else
            {
                GetDevices();
            }
            return value;
        }

        public float Get(Controller controller, InputFeatureUsage<float> usage)
        {
            InputDevice device = GetDevice(controller);
            float value = 0f;
            if (device != null && device.isValid)
            {
                device.TryGetFeatureValue(usage, out value);
            }
            else
            {
                GetDevices();
            }
            return value;
        }

        public bool Get(Controller controller, InputFeatureUsage<bool> usage)
        {
            InputDevice device = GetDevice(controller);
            bool value = false;
            if (device != null && device.isValid)
            {
                device.TryGetFeatureValue(usage, out value);
            }
            else
            {
                GetDevices();
            }
            return value;
        }

        public bool hasControllers()
        {
            bool hasController = false;
            if (leftController != null && leftController.isValid)
            {
                hasController = true;
            }
            if (rightController != null && rightController.isValid)
            {
                hasController = true;
            }
            return hasController;
        }
        
        [HarmonyPatch(typeof(GameInput), "UpdateAxisValues")]
        internal class UpdateAxisValuesPatch
        {
            public static bool Prefix(bool useKeyboard, bool useController, GameInput ___instance)
            {
                float[] axisValues = Traverse.Create(___instance).Field("axisValues").GetValue() as float[];
                float[] lastAxisValues = Traverse.Create(___instance).Field("lastAxisValues").GetValue() as float[];
                GameInput.Device lastDevice = (GameInput.Device)Traverse.Create(___instance).Field("lastDevice").GetValue();

              //  bool GetUseOculusInputManager = (bool)Traverse.Create(___instance).Method("GetUseOculusInputManager").GetValue();
                GameInput.ControllerLayout GetControllerLayout = (GameInput.ControllerLayout)Traverse.Create(___instance).Method("GetControllerLayout").GetValue();

                XRInputManager xrInput = GetXRInputManager();
                if (!xrInput.hasControllers())
                {
                    return true;
                }

                for (int i = 0; i < axisValues.Length; i++)
                {
                    axisValues[i] = 0f;
                }
                if (useController)
                {
                    //Oculus Axis Values
                    if (XRSettings.loadedDeviceName == "Oculus")
                    {
                        Vector2 vector = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.Active);
                        axisValues[2] = vector.x;
                        axisValues[3] = -vector.y;
                        Vector2 vector2 = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.Active);
                        axisValues[0] = vector2.x;
                        axisValues[1] = -vector2.y;
                        // TODO: Use deadzone?
                        axisValues[4] = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger, OVRInput.Controller.Active);
                        axisValues[5] = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger, OVRInput.Controller.Active);
                    }
                    //OpenVR Asix values
                    else if (XRSettings.loadedDeviceName == "OpenVR")
                    {
                        Vector2 vector = xrInput.Get(Controller.Left, CommonUsages.primary2DAxis);
                        axisValues[2] = vector.x;
                        axisValues[3] = -vector.y;
                        Vector2 vector2 = xrInput.Get(Controller.Right, CommonUsages.primary2DAxis);
                        axisValues[0] = vector2.x;
                        axisValues[1] = -vector2.y;
                        // TODO: Use deadzone?
                        axisValues[4] = xrInput.Get(Controller.Left, CommonUsages.trigger).CompareTo(0.3f);
                        axisValues[5] = xrInput.Get(Controller.Right, CommonUsages.trigger).CompareTo(0.3f);
                    }
                    else
                    {
                        //Prolly should leave these here just in case.
                        GameInput.ControllerLayout controllerLayout = GetControllerLayout;
                        if (controllerLayout == GameInput.ControllerLayout.Xbox360 || controllerLayout == GameInput.ControllerLayout.XboxOne || Application.platform == RuntimePlatform.PS4)
                        {
                            axisValues[2] = Input.GetAxis("ControllerAxis1");
                            axisValues[3] = Input.GetAxis("ControllerAxis2");
                            axisValues[0] = Input.GetAxis("ControllerAxis4");
                            axisValues[1] = Input.GetAxis("ControllerAxis5");
                            if (Application.platform == RuntimePlatform.PS4)
                            {
                                axisValues[4] = Mathf.Max(Input.GetAxis("ControllerAxis8"), 0f);
                                axisValues[5] = Mathf.Max(-Input.GetAxis("ControllerAxis3"), 0f);
                            }
                            else if (Application.platform == RuntimePlatform.XboxOne)
                            {
                                axisValues[4] = Mathf.Max(Input.GetAxis("ControllerAxis3"), 0f);
                                axisValues[5] = Mathf.Max(-Input.GetAxis("ControllerAxis3"), 0f);
                            }
                            else
                            {
                                axisValues[4] = Mathf.Max(-Input.GetAxis("ControllerAxis3"), 0f);
                                axisValues[5] = Mathf.Max(Input.GetAxis("ControllerAxis3"), 0f);
                            }
                            axisValues[6] = Input.GetAxis("ControllerAxis6");
                            axisValues[7] = Input.GetAxis("ControllerAxis7");
                        }
                        else if (controllerLayout == GameInput.ControllerLayout.Switch)
                        {
                            axisValues[2] = InputUtils.GetAxis("ControllerAxis1");
                            axisValues[3] = InputUtils.GetAxis("ControllerAxis2");
                            axisValues[0] = InputUtils.GetAxis("ControllerAxis4");
                            axisValues[1] = InputUtils.GetAxis("ControllerAxis5");
                            axisValues[4] = Mathf.Max(InputUtils.GetAxis("ControllerAxis3"), 0f);
                            axisValues[5] = Mathf.Max(-InputUtils.GetAxis("ControllerAxis3"), 0f);
                            axisValues[6] = InputUtils.GetAxis("ControllerAxis6");
                            axisValues[7] = InputUtils.GetAxis("ControllerAxis7");
                        }
                        else if (controllerLayout == GameInput.ControllerLayout.PS4)
                        {
                            axisValues[2] = Input.GetAxis("ControllerAxis1");
                            axisValues[3] = Input.GetAxis("ControllerAxis2");
                            axisValues[0] = Input.GetAxis("ControllerAxis3");
                            axisValues[1] = Input.GetAxis("ControllerAxis6");
                            axisValues[4] = (Input.GetAxis("ControllerAxis4") + 1f) * 0.5f;
                            axisValues[5] = (Input.GetAxis("ControllerAxis5") + 1f) * 0.5f;
                            axisValues[6] = Input.GetAxis("ControllerAxis7");
                            axisValues[7] = Input.GetAxis("ControllerAxis8");
                        }
                    }
                }
                if (useKeyboard)
                {
                    axisValues[10] = Input.GetAxis("Mouse ScrollWheel");
                    axisValues[8] = Input.GetAxisRaw("Mouse X");
                    axisValues[9] = Input.GetAxisRaw("Mouse Y");
                }
                for (int j = 0; j < axisValues.Length; j++)
                {
                    GameInput.AnalogAxis axis = (GameInput.AnalogAxis)j;
                    GameInput.Device deviceForAxis = (GameInput.Device)Traverse.Create(___instance).Method("GetDeviceForAxis", axis).GetValue();// ___instance.GetDeviceForAxis(axis);
                    float f = lastAxisValues[j] - axisValues[j];
                    lastAxisValues[j] = axisValues[j];
                    if (deviceForAxis != lastDevice)
                    {
                        float num = 0.1f;
                        if (Mathf.Abs(f) > num)
                        {
                            if (!PlatformUtils.isConsolePlatform)
                            {
                                lastDevice = deviceForAxis;
                            }
                        }
                        else
                        {
                            axisValues[j] = 0f;
                        }
                    }
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(FPSInputModule))]
        [HarmonyPatch("EscapeMenu")]
        class ArmsController_Update_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(FPSInputModule __instance)
            {
                if (__instance.lockPauseMenu)
                {
                    return false;
                }
                //Press and hold pda to access escape menu with touch controllers (should be left controller menu button by default)
                if (GameInput.GetButtonHeldTime(GameInput.Button.PDA) > 1.0f && IngameMenu.main != null && !IngameMenu.main.selected)
                {
                    IngameMenu.main.Open();
                    GameInput.ClearInput();
                }
                return false;
            }
        }
    }
}
