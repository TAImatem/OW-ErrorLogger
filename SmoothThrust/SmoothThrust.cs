using OWML.Common;
using OWML.Events;
using System;
using UnityEngine;
using Harmony;
using System.Reflection;

namespace SmoothThrustContainer
{

	//[HarmonyPatch(typeof(ThrusterModel), "AddTranslationalInput")]
	class ThrusterModel_AddTranslationalInput
	{
		public static void Prefix(ThrusterModel __instance, ref Vector3 input)
		{
			if (OWInput.IsNewlyPressed(InputLibrary.interact, InputMode.All))
			{
				SmoothThrustContainer._prevTranslation = input;
				SmoothThrustContainer._buildupVelocity = Vector3.zero;
			}
			if (input.magnitude > 0.0001f && OWInput.IsHeld(InputLibrary.interact, 0.05f, InputMode.All))
			{
				SmoothThrustContainer._prevTranslation.x = Mathf.SmoothDamp(SmoothThrustContainer._prevTranslation.x, input.x, ref SmoothThrustContainer._buildupVelocity.x, SmoothThrustContainer._analogSmoothTime);
				SmoothThrustContainer._prevTranslation.z = Mathf.SmoothDamp(SmoothThrustContainer._prevTranslation.z, input.z, ref SmoothThrustContainer._buildupVelocity.z, SmoothThrustContainer._analogSmoothTime);
				SmoothThrustContainer._prevTranslation.y = Mathf.SmoothDamp(SmoothThrustContainer._prevTranslation.y, input.y, ref SmoothThrustContainer._buildupVelocity.y, SmoothThrustContainer._analogSmoothTime);
				input = SmoothThrustContainer._prevTranslation;
			}
			SmoothThrustContainer._prevTranslation = input;
		}
	}
	public class SmoothThrustContainer:ModBehaviour
	{
		private void Start()
		{
			ModHelper.HarmonyHelper.AddPrefix<ThrusterModel>("AddTranslationalInput", typeof(ThrusterModel_AddTranslationalInput), "Prefix");
			ModHelper.Console.WriteLine("Smooth Thrust Ready!");
		}
		public static Vector3 _prevTranslation;
		public static Vector3 _buildupVelocity;
		public static float _analogSmoothTime = 0.3f;
	}
}
