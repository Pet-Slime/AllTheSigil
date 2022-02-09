using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections;
using Pixelplacement;
using System.Linq;
using GBC;
using Artwork = voidSigils.Voids_work.Resources.Resources;


namespace voidSigils
{

    internal class SigilPatches
    {

		[HarmonyPatch(typeof(BoneDigger), nameof(BoneDigger.RespondsToTurnEnd))]
		public class BoneDiggerPatch
		{
			[HarmonyPrefix]
			public static bool Prefix(bool playerTurnEnd)
			{
				return playerTurnEnd;
			}
		}

		[HarmonyPatch(typeof(QuadrupleBones), nameof(QuadrupleBones.RespondsToDie))]
		public class QuadrupleBonesPatch
		{
			[HarmonyPostfix]
			public static void RespondsToDie(bool wasSacrifice, PlayableCard killer, AbilityBehaviour __instance, ref bool __result)
			{
				__result = __instance.Card.Slot.IsPlayerSlot;
			}
		}


		[HarmonyPatch(typeof(PackMule), nameof(PackMule.RespondsToResolveOnBoard))]
		public class PackMulePatch
		{
			[HarmonyPostfix]
			public static void Postfix(ref bool __result)
			{
				__result = true;
			}
		}

		[HarmonyPatch(typeof(CombatPhaseManager), "DoCombatPhase", MethodType.Normal)]
		public class Shove_Combatphase_Startpatch
		{
			[HarmonyPrefix]
			public static void DoCombatPhase()
			{
				Plugin.voidCombatPhase = true;
			}
		}

		[HarmonyPatch(typeof(CombatPhaseManager), "DoCombatPhase", MethodType.Normal)]
		public class Shove_Combatphase_Endpatch
		{
			[HarmonyPostfix]
			public static void DoCombatPhase()
			{
				Plugin.voidCombatPhase = false;
			}
		}
	}
}
