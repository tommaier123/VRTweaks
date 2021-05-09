using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using HarmonyLib;
using System;
using QModManager.Utility;
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
        public static InputDevice leftController;
        public static InputDevice rightController;
        public static float thickness = 0.002f;
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
                if (device.role == InputDeviceRole.LeftHanded)
                {
                    leftController = device;
                    //Debug.Log("leftControllr: " + leftController);
                }
                if (device.role == InputDeviceRole.RightHanded)
                {
                    rightController = device;
                    //Debug.Log("rightController: " + rightController);
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
                    Debug.Log("Button0");
                    return Get(Controller.Right, CommonUsages.secondaryButton);

                case KeyCode.JoystickButton1:
                    Debug.Log("Button1");
                    // ControllerButtonB
                    return Get(Controller.Right, CommonUsages.primaryButton);

                case KeyCode.JoystickButton2:
                    Debug.Log("Button2");
                    // ControllerButtonX
                    return Get(Controller.Left, CommonUsages.secondaryButton);

                case KeyCode.JoystickButton3:
                    Debug.Log("Button3");
                    // ControllerButtonY
                    return Get(Controller.Left, CommonUsages.primaryButton);

                case KeyCode.JoystickButton4:
                    Debug.Log("Button4");
                    // ControllerButtonLeftBumper
                    return Get(Controller.Left, CommonUsages.gripButton);

                case KeyCode.JoystickButton5:
                    Debug.Log("Button5");
                    // ControllerButtonRightBumper
                    return Get(Controller.Right, CommonUsages.gripButton);

                case KeyCode.JoystickButton6:
                    //  return Get(Controller.Left, CommonUsages.);
                    Debug.Log("Button6");
                    return Get(Controller.Right, CommonUsages.menuButton);

                case KeyCode.JoystickButton7:
                    // ControllerButtonHome
                    Debug.Log("Button7");
                    return Get(Controller.Left, CommonUsages.menuButton);

                case KeyCode.JoystickButton8:
                    // ControllerButtonLeftStick
                    Debug.Log("Button8");
                    return Get(Controller.Left, CommonUsages.primary2DAxisClick);

                case KeyCode.JoystickButton9:
                    Debug.Log("Button9");
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


                for (int i = 0; i < axisValues.Length; i++)
                {
                    axisValues[i] = 0f;
                }
                if (useController)
                {
                    Vector2 vector22 = xrInput.Get(Controller.Right, CommonUsages.primary2DAxis);
                    axisValues[0] = vector22.x;
                    axisValues[1] = -vector22.y;
                    //Debug.Log("AxisValues[0]: " + axisValues[0] + " AxisValues[1]: " + axisValues[1]);

                    Vector2 vectorr = xrInput.Get(Controller.Left, CommonUsages.primary2DAxis);
                    axisValues[2] = vectorr.x;
                    axisValues[3] = -vectorr.y;
                    //Debug.Log("AxisValues[2]: " + axisValues[2] + " AxisValues[3]: " + axisValues[4]);

                    axisValues[4] = xrInput.Get(Controller.Left, CommonUsages.trigger).CompareTo(0.3f);
                    axisValues[5] = xrInput.Get(Controller.Right, CommonUsages.trigger).CompareTo(0.3f);

                    axisValues[6] = xrInput.Get(Controller.Left, CommonUsages.indexFinger).CompareTo(0.3f);
                    axisValues[7] = xrInput.Get(Controller.Right, CommonUsages.indexFinger).CompareTo(0.3f);
                    // Debug.Log("AxisValues[4]: " + axisValues[4] + " AxisValues[5]: " + axisValues[5]);
                    //  axisValues[6] = 0f;

                    //  axisValues[8] = InputTracking.GetLocalPosition(XRNode.RightHand).x;//Input.GetAxisRaw("Mouse X");
                    //  axisValues[9] = InputTracking.GetLocalPosition(XRNode.RightHand).y;//Input.GetAxisRaw("Mouse Y");
                }
                if (useKeyboard)
                {
                    axisValues[10] = Input.GetAxis("Mouse ScrollWheel");
                    axisValues[8] = Input.GetAxis("Mouse X"); //Input.GetAxisRaw("Mouse X");
                    axisValues[9] = Input.GetAxis("Mouse Y");//Input.GetAxisRaw("Mouse Y");
                }
                for (int j = 0; j < axisValues.Length; j++)
                {
                    GameInput.AnalogAxis axis = (GameInput.AnalogAxis)j;

                    GameInput.Device deviceForAxis = (GameInput.Device)Traverse.Create(___instance).Method("GetDeviceForAxis", axis).GetValue();
                    //   Debug.Log("deviceForAxis: " + deviceForAxis);
                    float f = lastAxisValues[j] - axisValues[j];
                    lastAxisValues[j] = axisValues[j];
                    if (deviceForAxis != lastDevice)
                    {
                        float num = 0.1f;
                        //  Debug.Log("0.1 F: " + f);
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

            /*  [HarmonyPatch(typeof(GameInput), "UpdateKeyInputs")]
              internal class UpdateKeyInputsPatch
              {
                  public static bool Prefix(bool useKeyboard, bool useController, GameInput ___instance)
                  {
                      GameInput.InputState[] InputStates = Traverse.Create(___instance).Field("inputStates").GetValue() as GameInput.InputState[];
                      List<GameInput.Input> inputs = Traverse.Create(___instance).Field("inputs").GetValue() as List<GameInput.Input>;
                      bool controllerEnabled = (bool)Traverse.Create(___instance).Field("controllerEnabled").GetValue();
                      GameInput.Device lastDevice = (GameInput.Device)Traverse.Create(___instance).Field("lastDevice").GetValue();
                      float[] axisValues = Traverse.Create(___instance).Field("axisValues").GetValue() as float[];
                      int[] lastInputPressed = Traverse.Create(___instance).Field("lastInputPressed").GetValue() as int[];
                      //  useKeyboard = false;

                      XRInputManager xrInput = GetXRInputManager();

                      if (!xrInput.hasControllers())
                      {
                          return true;
                      }

                      float unscaledTime = Time.unscaledTime;

                      for (int i = 0; i < inputs.Count; i++)
                      {
                          GameInput.InputState inputState = default;
                          GameInput.InputState prevInputState = InputStates[i];
                          inputState.timeDown = prevInputState.timeDown;
                          //GameInput.ControllerLayout controllerLayout = GameInput.GetControllerLayout();
                          bool wasHeld = (prevInputState.flags & GameInput.InputStateFlags.Held) > 0U;

                          GameInput.Input currentInput = inputs[i];
                          GameInput.Device device = currentInput.device;
                          KeyCode key =  currentInput.keyCode;


                          if (key != KeyCode.None)
                          {
                              bool pressed = xrInput.GetXRInput(key);

                              GameInput.InputStateFlags prevState = InputStates[i].flags;
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
                          else
                          {
                              float axisValue = axisValues[(int)currentInput.axis];
                              //Debug.Log("axisvalue: " + axisValue);
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
                          /*
                          if ((inputState.flags & GameInput.InputStateFlags.Down) != 0U)
                          {
                              int lastIndex = lastInputPressed[(int)device];
                              int newIndex = i;
                              inputState.timeDown = unscaledTime;
                              if (lastIndex > -1)
                              {
                                  GameInput.Input lastInput = inputs[lastIndex];
                                  bool isSameTime = inputState.timeDown == InputStates[lastIndex].timeDown;
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
                          InputStates[i] = inputState;
                      }

                      return false;
                  }
              }
          }*/
        }
    }
}
   

