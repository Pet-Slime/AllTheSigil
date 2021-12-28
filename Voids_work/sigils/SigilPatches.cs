using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using System.Linq;
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
			[HarmonyPrefix]
			public static bool RespondsToDie(bool wasSacrifice, PlayableCard killer, AbilityBehaviour __instance)
			{
				return __instance.Card.Slot.IsPlayerSlot;
			}
		}



	}
}
