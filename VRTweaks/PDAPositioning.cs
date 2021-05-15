using HarmonyLib;
using UnityEngine;
using RootMotion.FinalIK;

namespace VRTweaks
{
    public class PDAPositioning
    {

        static float pdaXOffset = -0.21f;
        static float pdaXRot = 223f;
        static float pdaYRot = 21f;
        static float pdaZRot = 76.5f;

        static GameObject leftHandTarget;
        static float lastVrOptionsPdaDistance = 0f;

        // Centers the target transform on the player and moves the
        // arm distance to the passed position
        static void updateTargetPosition(float distance)
        {
            if (!leftHandTarget)
            {
                return;
            }
            if (Player.main.motorMode != Player.MotorMode.Vehicle)
            {
                leftHandTarget.transform.localPosition =
                    leftHandTarget.transform.parent.transform.InverseTransformPoint(Player.main.playerController.forwardReference.position +
                                                                                    Player.main.armsController.transform.right * pdaXOffset +
                                                                                    Vector3.up * -0.15f +
                                                                                    new Vector3(Player.main.armsController.transform.forward.x,
                                                                                                0f,
                                                                                                Player.main.armsController.transform.forward.z).normalized * distance);
            }
            else
            {
                leftHandTarget.transform.localPosition =
                    leftHandTarget.transform.parent.transform.InverseTransformPoint(leftHandTarget.transform.parent.transform.position +
                                                                                    leftHandTarget.transform.parent.transform.right * pdaXOffset +
                                                                                    leftHandTarget.transform.parent.transform.forward * distance +
                                                                                    leftHandTarget.transform.parent.transform.up * -0.15f);
            }
            leftHandTarget.transform.rotation = Player.main.armsController.transform.rotation * Quaternion.Euler(pdaXRot, pdaYRot, pdaZRot);
            lastVrOptionsPdaDistance = VROptions.pdaDistance;
        }

        // Linear distance calcualtion between an appropriate min and max
        // that ranges between full scale of VROptions.pdaDistance
        static float calculatePdaDistance()
        {
            return 0.35f * VROptions.pdaDistance + 0.205f;
        }

        // Create target game object when PDA is opened and parent it to camera transform
        // Initialize it's position
        [HarmonyPatch(typeof(PDA), nameof(PDA.Open))]
        public class PDA_Open_Patch
        {

            static void Postfix(PDA __instance, bool __result)
            {
                if (__result)
                {
                    if (!leftHandTarget)
                    {
                        leftHandTarget = new GameObject();
                    }
                    leftHandTarget.transform.parent = Player.main.camRoot.transform;
                    updateTargetPosition(calculatePdaDistance());

                }
            }
        }

        // Destroy target game object when PDA is closed
        [HarmonyPatch(typeof(PDA), nameof(PDA.Close))]
        class PDA_Close_Patch
        {
            static void Postfix()
            {
                if (leftHandTarget)
                {
                    GameObject.Destroy(leftHandTarget);
                }
            }
        }

        // Update the target position if the Options menu has been updated
        // and apply the target to the IK
        [HarmonyPatch(typeof(PDA), nameof(PDA.ManagedUpdate))]
        class PDA_Update_Patch
        {
            static void Prefix()
            {
                if (leftHandTarget)
                {
                    if (VROptions.pdaDistance != lastVrOptionsPdaDistance)
                    {
                        updateTargetPosition(calculatePdaDistance());
                    }
                    var armsControllerIk = getArmsControllerIK();
                    if (armsControllerIk)
                    {
                        armsControllerIk.solver.leftHandEffector.target = leftHandTarget.transform;
                    }
                }
            }

            static FullBodyBipedIK getArmsControllerIK()
            {
                if (Player.main && Player.main.armsController)
                {
                    var armsController = Player.main.armsController;
                    return AccessTools.FieldRefAccess<ArmsController, FullBodyBipedIK>(armsController, nameof(ArmsController.ik));
                }
                return null;
            }
        }

    }
}
