using HarmonyLib;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

namespace VRTweaks
{
    public enum ControllerLayout
    {
        // Token: 0x04003C52 RID: 15442
        Automatic,
        // Token: 0x04003C53 RID: 15443
        Xbox360,
        // Token: 0x04003C54 RID: 15444
        XboxOne,
        // Token: 0x04003C55 RID: 15445
        PS4,
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

            if (pda.isActiveAndEnabled)
            {

                ik.solver.leftHandEffector.target = leftController.transform;
                ik.solver.rightHandEffector.target = null;
            }

            if (heldItem != null)
            {
                if (heldItem.item.GetComponent<FlashLight>())
                {

                    ik.solver.leftHandEffector.target = null;
                    ik.solver.rightHandEffector.target = rightController.transform;
                }
            }
            /* if (XRInputManager.GetXRInputManager().rightController.TryGetFeatureUsages(inputFeatures))
             {
                 foreach (var feature in inputFeatures)
                 {
                     if (feature.type == typeof(bool))
                     {
                         bool featureValue;
                         if (XRInputManager.GetXRInputManager().rightController.TryGetFeatureValue(feature.As<bool>(), out featureValue))
                         {
                             if (!File.ReadAllText("Logs/Right.txt").Contains(feature.name))
                             {
                                 File.AppendAllText("Logs/Right.txt", "Bool Feature: " + feature.name + " , Value: " + feature.ToString() + " , Type" + feature.type + Environment.NewLine);
                                 //Debug.Log(string.Format("Right Bool feature {0}'s value is {1}", feature.name, featureValue.ToString()));
                             }
                         }
                     }
                 }
             }
             if (XRInputManager.GetXRInputManager().leftController.TryGetFeatureUsages(inputFeatures))
             {
                 foreach (var feature in inputFeatures)
                 {
                     if (feature.type == typeof(bool))
                     {
                         bool featureValue;
                         if (XRInputManager.GetXRInputManager().leftController.TryGetFeatureValue(feature.As<bool>(), out featureValue))
                         {
                             if (!File.ReadAllText("Logs/Left.txt").Contains(feature.name))
                             {
                                 File.AppendAllText("Logs/Left.txt", "Bool Feature: " + feature.name + " , Value: " + feature.ToString() + " , Type" + feature.type + Environment.NewLine);
                                 //Debug.Log(string.Format("Left Bool feature {0}'s value is {1}", feature.name, featureValue.ToString()));
                             }
                         }
                     }
                 }
             }*/
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

        /*        [HarmonyPatch(typeof(GameInput))]
                [HarmonyPatch("Reconfigure")]
                class SetupDefaultControllerBindings_Patch
                {
                    [HarmonyPrefix]
                    public static void Prefix(GameInput __instance)
                    {

                    }
                }*/
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


