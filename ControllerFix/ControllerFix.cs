using OWML.Common;
using OWML.Events;
using System;
using UnityEngine;
using Harmony;
using System.Reflection;
using PadEZ;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections;

namespace XBoxCOntrollerFix
{

	class GamePad_PC_XB1_Fix
	{
		public static void Postfix(GamePad_PC_XB1 __instance, ref uint ___m_xb1Index, ref XboxControllerFix.XINPUT_State ___m_padState)
		{
			XboxControllerFix.ModHelper.Console.WriteLine($"controller SetUp normal index: {___m_xb1Index}");
			for (uint num = 0u; num < 4u; num += 1u)
			{
				if (XboxControllerFix.XInputGetState(num, out ___m_padState) == 0u && !XboxControllerFix.occInds.Contains(num))
				{
					___m_xb1Index = num;
				}
			}
			XboxControllerFix.occInds.Add(___m_xb1Index);
			XboxControllerFix.ModHelper.Console.WriteLine($"controller SetUp patched index: {___m_xb1Index}");
		}
		
		public static void Destr_Prefix(GamePad_PC_XB1 __instance, ref uint ___m_xb1Index)
		{
			XboxControllerFix.ModHelper.Console.WriteLine("entered patched controller destroy");
			XboxControllerFix.occInds.Remove(___m_xb1Index);
		}
	}


	class PadManager_Fix
	{
		public static bool Prefix(ThrusterModel __instance, ref GamePad __result, int padIndex, string name)
		{
			GamePad result = null;
			if (name != null)
			{
				if (name == "Controller (Xbox One For Windows)" || name == "Xbox One Wireless Controller" || name == "Bluetooth XINPUT compatible input device")
				{
					result = new GamePad_PC_XB1(padIndex, GamePad.PadType.XB1);
				}
				else if (name.ToUpper().Contains("XBOX") || name.ToUpper().Contains("XINPUT"))
				{
					result = new GamePad_PC_XB1(padIndex, GamePad.PadType.X360);
				}

			}
			if (result != null)
			{
				XboxControllerFix.ModHelper.Console.WriteLine($"patched controller type: #{padIndex} \"{name}\"; resulting type = {result.GetPadType()}");
				__result = result;
				return false;
			}
			else
			{
				XboxControllerFix.ModHelper.Console.WriteLine($"normal controller type: #{padIndex} \"{name}\"");
				return true;
			}
		}
	}

	public class XboxControllerFix:ModBehaviour
	{
		private void Start()
		{
			ModHelper.HarmonyHelper.AddPostfix<PadEZ.GamePad_PC_XB1>("SetUp", typeof(GamePad_PC_XB1_Fix), "Postfix");
			ModHelper.HarmonyHelper.AddPrefix<PadEZ.GamePad_PC_XB1>("Destroy", typeof(GamePad_PC_XB1_Fix), "Destr_Prefix");
			ModHelper.HarmonyHelper.AddPrefix<PadEZ.PadManager>("CreateController", typeof(PadManager_Fix), "Prefix");
			ModHelper.Console.WriteLine("Controller Fix ready!");
		}
		public static HashSet<uint> occInds = new HashSet<uint>();
		public static bool looppreventer;

		[DllImport("XINPUT9_1_0.DLL")]
		public static extern uint XInputGetState(uint userIndex, out XboxControllerFix.XINPUT_State state);
		public struct XINPUT_State
		{
			// Token: 0x040000C3 RID: 195
			public uint PacketNumber;

			// Token: 0x040000C4 RID: 196
			public ushort Buttons;

			// Token: 0x040000C5 RID: 197
			public byte LeftTrigger;

			// Token: 0x040000C6 RID: 198
			public byte RightTrigger;

			// Token: 0x040000C7 RID: 199
			public short ThumbLX;

			// Token: 0x040000C8 RID: 200
			public short ThumbLY;

			// Token: 0x040000C9 RID: 201
			public short ThumbRX;

			// Token: 0x040000CA RID: 202
			public short ThumbRY;
		}
	}

}
