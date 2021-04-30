using UnityEngine;
using RootMotion.FinalIK;
using HarmonyLib;
using UnityEngine.XR;
using System;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace VRTweaks
{
    /*class XRArmsController
    {
        private static readonly XRArmsController _instance = new XRArmsController();
        public ArmsController armsController;
        public Player player;
        public FullBodyBipedIK ik;
        public Avatar avatar;

        string editSide = "right";
        string editType = "rotation";
        string editCoord = "x";

        // 0f, -0.13f, -0.14f
        public float leftPosX = 0.006f;
        public float leftPosY = -0.08f;
        public float leftPosZ = -0.07f;

        // 270f, 90f, 0f
        public float leftRotX = 270f;
        public float leftRotY = 90f;
        public float leftRotZ = 0f;

        // 0f, -0.13f, -0.14f
        public float rightPosX = 0f;
        public float rightPosY = -0.13f;
        public float rightPosZ = -0.14f;

        // 35f, 190f, 270f
        public float rightRotX = 20f;
        public float rightRotY = 190f;
        public float rightRotZ = 270f;

        public Vector3 scale;

        private XRArmsController()
        {
        }

        static XRArmsController GetXRArmsController()
        {
            return _instance;
        }

        public void Initialize(ArmsController controller)
        {
         //   Debug.Log("InitializeStart");
            armsController = controller;
            player = Utils.GetLocalPlayerComp();
            ik = controller.GetComponent<FullBodyBipedIK>();
            scale = MainCameraControl.main.viewModel.localScale;
          //  Debug.Log("InitializeEnd");
        }

        public void setEdit()
        {
            //Debug.Log("setEditStart");
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKey(KeyCode.Z))
                {
                    editCoord = "x";
                }
                if (Input.GetKey(KeyCode.X))
                {
                    editCoord = "y";
                }
                if (Input.GetKey(KeyCode.C))
                {
                    editCoord = "z";
                }
                if (Input.GetKey(KeyCode.A))
                {
                    editType = "position";
                }
                if (Input.GetKey(KeyCode.S))
                {
                    editType = "rotation";
                }
                if (Input.GetKey(KeyCode.D))
                {
                    editType = "scale";
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    editSide = "left";
                }
                if (Input.GetKey(KeyCode.W))
                {
                    editSide = "right";
                }
               // Debug.Log("setEditEnd");
            }
        }

        public void editHandCoord()
        {
         //   Debug.Log("editHandCoordStart");
            if (Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    setEditCoord(0.1f * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    setEditCoord(-0.1f * Time.deltaTime);
                }
              //  Debug.Log("editHandCoordEnd");
            }
        }

        public void printHandCoord()
        {
           // Debug.Log("printHandCoordStart");
            Console.WriteLine("left position:");
            Console.WriteLine(leftPosX.ToString() + " " + leftPosY.ToString() + " " + leftPosZ.ToString());
            Console.WriteLine("left rotation:");
            Console.WriteLine(leftRotX.ToString() + " " + leftRotY.ToString() + " " + leftRotZ.ToString());
            Console.WriteLine("right position:");
            Console.WriteLine(rightPosX.ToString() + " " + rightPosY.ToString() + " " + rightPosZ.ToString());
            Console.WriteLine("right rotation:");
            Console.WriteLine(rightRotX.ToString() + " " + rightRotY.ToString() + " " + rightRotZ.ToString());
           // Debug.Log("printHandCoordEnd");
        }

        public void setEditCoord(float edit)
        {
            //Debug.Log("setEditCoordStart");
            if (editSide == "left")
            {
                if (editType == "position")
                {
                    if (editCoord == "x")
                    {
                        leftPosX += edit;
                    }
                    if (editCoord == "y")
                    {
                        leftPosY += edit;
                    }
                    if (editCoord == "z")
                    {
                        leftPosZ += edit;
                    }
                }
                if (editType == "rotation")
                {
                    if (editCoord == "x")
                    {
                        leftRotX += edit;
                    }
                    if (editCoord == "y")
                    {
                        leftRotY += edit;
                    }
                    if (editCoord == "z")
                    {
                        leftRotZ += edit;
                    }
                }
            }
            if (editSide == "right")
            {
                if (editType == "position")
                {
                    if (editCoord == "x")
                    {
                        rightPosX += edit;
                    }
                    if (editCoord == "y")
                    {
                        rightPosY += edit;
                    }
                    if (editCoord == "z")
                    {
                        rightPosZ += edit;
                    }
                }
                if (editType == "rotation")
                {
                    if (editCoord == "x")
                    {
                        rightRotX += edit;
                    }
                    if (editCoord == "y")
                    {
                        rightRotY += edit;
                    }
                    if (editCoord == "z")
                    {
                        rightRotZ += edit;
                    }
                }
            }
            if (editType == "scale")
            {
                scale += new Vector3(edit, edit, edit);
            }
          //  Debug.Log("setEditCoordEnd");
        }

        public void logAnimations()
        {
        //    Debug.Log("logAnimationsStart");
            AnimatorClipInfo[] anims = Player.main.playerAnimator.GetCurrentAnimatorClipInfo(0);
            foreach (AnimatorClipInfo info in anims)
            {
                QModManager.Utility.Logger.Log(0, info.clip.name, null, true);
                Debug.Log("Animation: " + info.clip.name);
                Debug.Log("logAnimationsEnd");
            }
        }

                public void UpdateHandPositions()
                {
                     //Debug.Log("UpdateHandPositionsStart");
                    setEdit();
                    editHandCoord();
                    printHandCoord();
                  //  logAnimations();

                    MainCameraControl.main.viewModel.localScale = scale;

                    XRInputManager xrInput = XRInputManager.GetXRInputManager();
                    Vector3 leftHandPos = xrInput.Get(Controller.Left, CommonUsages.devicePosition);
                    Quaternion leftHandRot = xrInput.Get(Controller.Left, CommonUsages.deviceRotation);
                    Vector3 rightHandPos = xrInput.Get(Controller.Right, CommonUsages.devicePosition);
                    Quaternion rightHandRot = xrInput.Get(Controller.Right, CommonUsages.deviceRotation);

            //  Transform rightHand = xrInput.GetGameTransform(Controller.Right);
                    Transform rightHand = armsController.rightHand.transform;
                  //  Debug.Log("RighHand: " + rightHand.position);
                    rightHand.localPosition += rightHandPos;//new Vector3(rightPosX, rightPosY, rightPosZ);
                    rightHand.localRotation *= rightHandRot;//Quaternion.Euler(rightRotX, rightRotY, rightRotZ);
                    ik.solver.rightHandEffector.target = rightHand;
                    ik.solver.rightHandEffector.positionWeight = 1f;

                    // Transform leftHand = xrInput.GetGameTransform(Controller.Left);
                    Transform leftHand = armsController.leftHand.transform;
                  //  Debug.Log("LeftHand: " + leftHand.position);
                    leftHand.localPosition += leftHandPos; //new Vector3(leftPosX, leftPosY, leftPosZ);
                    leftHand.localRotation *= leftHandRot;//Quaternion.Euler(leftRotX, leftRotY, leftRotZ);
                    ik.references.leftHand.localRotation = leftHand.localRotation;
                    ik.references.leftHand.localPosition = leftHand.localPosition;
                    ik.solver.leftHandEffector.target = leftHand;
                    ik.solver.leftHandEffector.positionWeight = 1f;
                   // Debug.Log("UpdateHandPositionsEnd");
                    //disableArmAnimations();
        }

        public void disableArmAnimations()
        {
          //  Debug.Log("disableArmAnimationsStart");
            Animator anim = player.playerAnimator;
            Transform leftLowerArm = ik.references.leftForearm;
            AvatarMask mask = new AvatarMask();
            mask.AddTransformPath(leftLowerArm);
            PlayableGraph graph = player.playerAnimator.playableGraph;
            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(graph, "LayerMixer", anim);
            AnimationMixerPlayable mixer = AnimationMixerPlayable.Create(graph, 2);
            playableOutput.SetSourcePlayable(mixer);
          //  Debug.Log("disableArmAnimationsEnd");
        }

        [HarmonyPatch(typeof(ArmsController))]
        [HarmonyPatch("Start")]
        class ArmsController_Start_Patch
        {
            [HarmonyPostfix]
            public static void PostFix(ArmsController __instance)
            {
                if (!XRSettings.enabled)
                {
                    return;
                }
               // GetXRArmsController().Initialize(__instance);
            }
        }

        [HarmonyPatch(typeof(ArmsController))]
        [HarmonyPatch("Update")]
        class ArmsController_Update_Patch
        {

            [HarmonyPostfix]
            public static void Postfix()
            {
                return;
                if (!XRSettings.enabled)
                {
                    return;
                }
                XRArmsController xrArms = GetXRArmsController();
                Debug.Log("xrArms: " + xrArms);
                Player player = xrArms.player;
                Debug.Log("player: " + player);
                if ((Player.main.motorMode != Player.MotorMode.Vehicle && !player.cinematicModeActive))
                {
                    //xrArms.UpdateHandPositions();
                }
            }
        }
        /*
                [HarmonyPatch(typeof(MainCameraControl))]
                [HarmonyPatch("Update")]
                class MainCameraContral_Update_Patch
                {

                    [HarmonyPostfix]
                    public static void Postfix(MainCameraControl __instance)
                    {
                        if (!XRSettings.enabled)
                        {
                            return;
                        }
                        __instance.viewModel.transform.localPosition = __instance.gameObject.FindAncestor<PlayerController>().forwardReference.position;
                    }
                }
    }*/
}