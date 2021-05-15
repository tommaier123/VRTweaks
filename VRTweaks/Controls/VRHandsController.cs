using HarmonyLib;
using RootMotion.FinalIK;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

namespace VRTweaks
{

    public enum ControllerLayout
    {
        // Token: 0x04005E34 RID: 24116
        Automatic,
        // Token: 0x04005E35 RID: 24117
        Xbox360,
        // Token: 0x04005E36 RID: 24118
        XboxOne,
        // Token: 0x04005E37 RID: 24119
        PS4,
        // Token: 0x04005E38 RID: 24120
        Switch,
        // Token: 0x04006E2F RID: 28207
        OpenVR
    }
    public class VRHandsController : MonoBehaviour
    {
        public static GameObject rightController;
        public static GameObject leftController;
        public ArmsController armsController;
        public Player player;
        public static FullBodyBipedIK ik;
        public static PDA pda;
        private static VRHandsController _main;
        public static VRHandsController main
        {
            get
            {
                if (_main == null)
                {
                    _main = new VRHandsController();
                }
                return _main;
            }
        }

        public void Initialize(ArmsController controller)
        {
            armsController = controller;
            player = global::Utils.GetLocalPlayerComp();
            ik = controller.GetComponent<FullBodyBipedIK>();
            pda = player.GetPDA();

            rightController = new GameObject("rightController");
            rightController.transform.parent = player.camRoot.transform;

            leftController = new GameObject("leftController");
            leftController.transform.parent = player.camRoot.transform;

            Debug.Log("rightControllerInt: " + rightController);
            Debug.Log("leftControllerInt: " + leftController);
            Debug.Log("IKInt: " + ik);
        }

        public void UpdateHandPositions()
        {
            InventoryItem heldItem = Inventory.main.quickSlots.heldItem;

            if (XRInputManager.GetXRInputManager().rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPos) && XRInputManager.GetXRInputManager().rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rightRot))
            {
                rightController.transform.localPosition = rightPos + new Vector3(0f, -0.13f, -0.14f);
                rightController.transform.localRotation = rightRot * Quaternion.Euler(35f, 190f, 270f);
            }

            if (XRInputManager.GetXRInputManager().leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPos) && XRInputManager.GetXRInputManager().leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion leftRot))
            {
                leftController.transform.localPosition = leftPos + new Vector3(0f, -0.13f, -0.14f);
                leftController.transform.localRotation = leftRot * Quaternion.Euler(270f, 90f, 0f);
            }
            //is user has pda enabled enable motion support for left hand
            if (pda.isActiveAndEnabled)
            {
                ik.solver.leftHandEffector.target = leftController.transform;
                ik.solver.rightHandEffector.target = null;
            }

            if (heldItem != null)
            {
                //if player has a flash light in hand enable motion support for right hand
                if (heldItem.item.GetComponent<FlashLight>())
                {
                    ik.solver.leftHandEffector.target = null;
                    ik.solver.rightHandEffector.target = rightController.transform;
                }
            }
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

                main.Initialize(__instance);
            }
        }

        [HarmonyPatch(typeof(ArmsController))]
        [HarmonyPatch("Update")]
        class ArmsController_Update_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(ArmsController __instance)
            {
                if (!XRSettings.enabled)
                {
                    return;
                }

                PDA pda = VRHandsController.pda;
                Player player = Player.main;

                if ((/*(Player.main.motorMode != Player.MotorMode.Vehicle &&*/ !player.cinematicModeActive))
                {
                    main.UpdateHandPositions();
                }
            }
        }

        [HarmonyPatch(typeof(ArmsController))]
        [HarmonyPatch("Reconfigure")]
        class ArmsController_Reconfigure_Patch
        {
            [HarmonyPrefix]
            public static void Prefix(ArmsController __instance, PlayerTool tool)
            {
                FullBodyBipedIK ik = VRHandsController.ik;
                ik.solver.GetBendConstraint(FullBodyBipedChain.LeftArm).bendGoal = __instance.leftHandElbow;
                ik.solver.GetBendConstraint(FullBodyBipedChain.LeftArm).weight = 1f;

                Traverse tInstance = Traverse.Create(__instance);
                tInstance.Field("leftAim").Field("shouldAim").SetValue(false);
                tInstance.Field("rightAim").Field("shouldAim").SetValue(false);

                ik.solver.leftHandEffector.target = null;
                ik.solver.rightHandEffector.target = null;

                Transform leftWorldTarget = tInstance.Field<Transform>("leftWorldTarget").Value;

                if (leftWorldTarget)
                {
                    ik.solver.leftHandEffector.target = leftWorldTarget;
                    ik.solver.GetBendConstraint(FullBodyBipedChain.LeftArm).bendGoal = null;
                    ik.solver.GetBendConstraint(FullBodyBipedChain.LeftArm).weight = 0f;
                }

                Transform rightWorldTarget = tInstance.Field<Transform>("rightWorldTarget").Value;

                if (rightWorldTarget)
                {
                    ik.solver.rightHandEffector.target = rightWorldTarget;
                    return;
                }
            }
        }
    }
}


