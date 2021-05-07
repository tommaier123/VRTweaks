using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UXR=UnityEngine.XR;

namespace UWEXR
{
	public class InputTracking
	{
		public static Vector3 GetLocalPosition(UXR.XRNode node)
		{
			return UXR.InputTracking.GetLocalPosition(GetUNode(node));
		}

		public static Quaternion GetLocalRotation(UXR.XRNode node)
		{
			return UXR.InputTracking.GetLocalRotation(GetUNode(node));
		}

		public static void Recenter()
		{
			UXR.InputTracking.Recenter();
		}

		public static string GetNodeName(ulong uniqueID)
		{
			return UXR.InputTracking.GetNodeName(uniqueID);
		}

		public static void GetNodeStates(List<UXR.XRNodeState> nodeStates)
		{
			List<UXR.XRNodeState> list = new List<UXR.XRNodeState>();
			foreach (UXR.XRNodeState xrnodeState in nodeStates)
			{
				Vector3 position;
				xrnodeState.TryGetPosition(out position);
				Quaternion rotation;
				xrnodeState.TryGetRotation(out rotation);
				Vector3 velocity;
				xrnodeState.TryGetVelocity(out velocity);
				Vector3 angularVelocity;
				xrnodeState.TryGetAngularVelocity(out angularVelocity);
				Vector3 acceleration;
				xrnodeState.TryGetAcceleration(out acceleration);
				Vector3 angularAcceleration;
				xrnodeState.TryGetAngularAcceleration(out angularAcceleration);
				UXR.XRNodeState item = default(UXR.XRNodeState);
				item.uniqueID = xrnodeState.uniqueID;
				item.nodeType = InputTracking.GetUNode(xrnodeState.nodeType);
				item.tracked = xrnodeState.tracked;
				item.position = position;
				item.rotation = rotation;
				item.velocity = velocity;
				item.angularVelocity = angularVelocity;
				item.acceleration = acceleration;
				item.angularAcceleration = angularAcceleration;
				list.Add(item);
			}
			UXR.InputTracking.GetNodeStates(list);
		}

		public static bool disablePositionalTracking
		{
			get
			{
				return UXR.InputTracking.disablePositionalTracking;
			}
		}

		private static UXR.XRNode GetUNode(UXR.XRNode node)
		{
			return (UXR.XRNode)Enum.Parse(typeof(UXR.XRNode), node.ToString());
		}
	}
}
