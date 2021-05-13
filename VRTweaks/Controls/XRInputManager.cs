using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using HarmonyLib;
using System;

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

        public bool GetXRInput(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.JoystickButton0:
                    // ControllerButtonA
                    return Get(Controller.Right, CommonUsages.primaryButton);
                case KeyCode.JoystickButton1:
                    // ControllerButtonB
                    return Get(Controller.Right, CommonUsages.secondaryButton);
                case KeyCode.JoystickButton2:
                    // ControllerButtonX
                    return Get(Controller.Left, CommonUsages.primaryButton);
                case KeyCode.JoystickButton3:
                    // ControllerButtonY
                    return Get(Controller.Left, CommonUsages.secondaryButton);
                case KeyCode.JoystickButton4:
                    // ControllerButtonLeftBumper
                    return Get(Controller.Left, CommonUsages.gripButton);
                case KeyCode.JoystickButton5:
                    // ControllerButtonRightBumper
                    return Get(Controller.Right, CommonUsages.gripButton);
                case KeyCode.JoystickButton6:
                    // ControllerButtonBack - reservered by "oculus" button
                    return false;
                case KeyCode.JoystickButton7:
                    // ControllerButtonHome
                    return Get(Controller.Left, CommonUsages.menuButton);
                case KeyCode.JoystickButton8:
                    // ControllerButtonLeftStick
                    return Get(Controller.Left, CommonUsages.primary2DAxisClick);
                case KeyCode.JoystickButton9:
                    // ControllerButtonRightStick
                    return Get(Controller.Right, CommonUsages.primary2DAxisClick);
                default:
                    return false;
            }
        }

        [HarmonyPatch(typeof(GameInput), "UpdateAxisValues")]
        internal class UpdateAxisValuesPatch
        {
            public static bool Prefix(bool useKeyboard, bool useController, GameInput ___instance)
            {
                float[] axisValues = Traverse.Create(___instance).Field("axisValues").GetValue() as float[];
                float[] lastAxisValues = Traverse.Create(___instance).Field("lastAxisValues").GetValue() as float[];
                GameInput.Device lastDevice = (GameInput.Device)Traverse.Create(___instance).Field("lastDevice").GetValue();


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
                    Vector2 vector = xrInput.Get(Controller.Left, CommonUsages.primary2DAxis);
                    axisValues[2] = vector.x;
                    axisValues[3] = -vector.y;
                    Vector2 vector2 = xrInput.Get(Controller.Right, CommonUsages.primary2DAxis);
                    axisValues[0] = vector2.x;
                    axisValues[1] = -vector2.y;
                    // TODO: Use deadzone?
                    axisValues[4] = xrInput.Get(Controller.Left, CommonUsages.trigger).CompareTo(0.3f);
                    axisValues[5] = xrInput.Get(Controller.Right, CommonUsages.trigger).CompareTo(0.3f);
                    // Debug.Log("AxisValues6: " + axisValues[6]);
                    // Debug.Log("AxisValues7: " + axisValues[7]);
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
        public static void InitializeMainDevice()
        {
            List<InputDevice> allDevices = new List<InputDevice>();
            InputDevices.GetDevices(allDevices);

            foreach (var inputDevice in allDevices)
            {
                if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
                {
                    Debug.Log("found device: " + inputDevice.name);
                    if (inputDevice.name.Contains("Oculus") || inputDevice.name.Contains("Quest"))
                    {
                        Debug.Log("it is Oculus!");
                    }
                    else if (inputDevice.name.Contains("Vive"))
                    {
                        Debug.Log("it is Vive!");
                    }
                    else if (inputDevice.name.Contains("Index"))
                    {
                        Debug.Log("it is Index!");
                    }
                }
                // Debug.Log("-----NAME: " + inputDevice.name);
                // Debug.Log("-characteristics: " + inputDevice.characteristics);
                // Debug.Log("-manufacturer: " + inputDevice.manufacturer);
                // Debug.Log("-subsystem: " + inputDevice.subsystem);
                // Debug.Log("-serialNumber: " + inputDevice.serialNumber);
                // Debug.Log("-isValid: " + inputDevice.isValid);
            }
        }
        //Need to find out when this is enabled why Joystick axis do not work correctly.
        [HarmonyPatch(typeof(GameInput), "UpdateKeyInputs")]
        internal class UpdateKeyInputsPatch
        {
            public static bool Prefix(bool useKeyboard, bool useController, GameInput ___instance)
            {
                if (XRInputManager.GetXRInputManager().leftController.TryGetFeatureValue(CommonUsages.menuButton, out var left) || XRInputManager.GetXRInputManager().rightController.TryGetFeatureValue(CommonUsages.menuButton, out var right))
                {
                    Debug.Log("This is the start button maybe");
                }
                return true;

            }
        }
        /*GameInput.InputState[] inputStates = Traverse.Create(___instance).Field("inputStates").GetValue() as GameInput.InputState[];
                List<GameInput.Input> inputs = Traverse.Create(___instance).Field("inputs").GetValue() as List<GameInput.Input>;
                bool controllerEnabled = (bool)Traverse.Create(___instance).Field("controllerEnabled").GetValue();
                GameInput.Device lastDevice = (GameInput.Device)Traverse.Create(___instance).Field("lastDevice").GetValue();
                float[] axisValues = Traverse.Create(___instance).Field("axisValues").GetValue() as float[];
                int[] lastInputPressed = Traverse.Create(___instance).Field("lastInputPressed").GetValue() as int[];

                XRInputManager xrInput = GetXRInputManager();
                if (!xrInput.hasControllers())
                {
                    return true;
                }

                float unscaledTime = Time.unscaledTime;
                for (int i = 0; i < inputs.Count; i++)
                {
                    GameInput.InputState inputState = default;
                    GameInput.InputState prevInputState = inputStates[i];
                    inputState.timeDown = prevInputState.timeDown;
                    bool wasHeld = (prevInputState.flags & GameInput.InputStateFlags.Held) > 0U;

                    GameInput.Input currentInput = inputs[i];
                    GameInput.Device device = currentInput.device;
                    KeyCode key = currentInput.keyCode;

                    if (key != KeyCode.None)
                    {
                        bool pressed = xrInput.GetXRInput(key);
                        GameInput.InputStateFlags prevState = inputStates[i].flags;
                        if (pressed && (prevState == GameInput.InputStateFlags.Held && prevState == GameInput.InputStateFlags.Down))
                        {
                            inputState.flags |= GameInput.InputStateFlags.Held;
                        }
                        if (pressed && prevState == GameInput.InputStateFlags.Up)
                        {
                            inputState.flags |= GameInput.InputStateFlags.Down;
                        }
                        if (!pressed)
                        {
                            inputState.flags |= GameInput.InputStateFlags.Up;
                        }
                        if (inputState.flags != 0U && !PlatformUtils.isConsolePlatform && (controllerEnabled || device != GameInput.Device.Controller))
                        {
                            lastDevice = device;
                        }
                    }
                  /*  else
                    {
                        float axisValue = axisValues[(int)currentInput.axis];
                        bool isPressed;
                        if (inputs[i].axisPositive)
                        {
                            isPressed = (axisValue > currentInput.axisDeadZone);
                        }
                        else
                        {
                            isPressed = (axisValue < -currentInput.axisDeadZone);
                        }
                        if (isPressed)
                        {
                            inputState.flags |= GameInput.InputStateFlags.Held;
                        }
                        if (isPressed && !wasHeld)
                        {
                            inputState.flags |= GameInput.InputStateFlags.Down;
                        }
                        if (!isPressed && wasHeld)
                        {
                            inputState.flags |= GameInput.InputStateFlags.Up;
                        }
                    }

                    if ((inputState.flags & GameInput.InputStateFlags.Down) != 0U)
                    {
                        int lastIndex = lastInputPressed[(int)device];
                        int newIndex = i;
                        inputState.timeDown = unscaledTime;
                        if (lastIndex > -1)
                        {
                            GameInput.Input lastInput = inputs[lastIndex];
                            bool isSameTime = inputState.timeDown == inputStates[lastIndex].timeDown;
                            bool lastAxisIsGreater = Mathf.Abs(axisValues[(int)lastInput.axis]) > Mathf.Abs(axisValues[(int)currentInput.axis]);
                            if (isSameTime && lastAxisIsGreater)
                            {
                                newIndex = lastIndex;
                            }
                        }
                        lastInputPressed[(int)device] = newIndex;
                    }

                    if ((device == GameInput.Device.Controller && !useController) || (device == GameInput.Device.Keyboard && !useKeyboard))
                    {
                        inputState.flags = 0U;
                        if (wasHeld)
                        {
                            inputState.flags |= GameInput.InputStateFlags.Up;
                        }
                    }
                    inputStates[i] = inputState;
                }

                return false;
            }
        }*/
    }
}