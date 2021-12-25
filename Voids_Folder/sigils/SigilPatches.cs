using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using Artwork = voidSigils.Resources.Resources;


namespace voidSigils
{


    internal class SigilPatches
    {

		[HarmonyPatch(typeof(CardInfo), "Sacrificable", MethodType.Getter)]
		public class CardInfo_Sacrificable
		{
			[HarmonyPostfix]
			public static void Postfix(ref CardInfo __instance, ref bool __result)
			{

				if (__instance.abilities.Contains(void_Pathetic.ability) || __instance.ModAbilities.Contains(void_Pathetic.ability))
                {
					__result = false;
				}
			}
		}

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


		[HarmonyPatch(typeof(PlayableCard), "GetPassiveAttackBuffs")]
		public class MakeZapperWorkTry3
		{
			[HarmonyPostfix]
			public static void Postfix(ref int __result, ref PlayableCard __instance)
			{
				if (__instance.OnBoard)
				{
					foreach (CardSlot slotState in Singleton<BoardManager>.Instance.GetAdjacentSlots(__instance.slot))
					{
						if (slotState.Card != null && slotState.Card.Info.HasAbility(void_zapper.ability))
						{
							__result = __result + 2;
						}
					}
				}
			}
		}




		////	[HarmonyPatch(typeof(BeesOnHit), "RespondsToTakeDamage", MethodType.Normal)]
		////	public class BeesOnHit_postfix_RespondsToTakeDamage
		////	{
		////
		////        [HarmonyPostfix]
		////		public static void Postfix(ref PlayableCard source, ref bool __result)
		////		{
		////			PlayableCard Patch = source;
		////			Plugin.Log.LogMessage("Bee on Hit Patch: Card name" + Patch.name);
		////			Plugin.Log.LogMessage("Bee on Hit Patch: Card slot" + Patch.slot);
		////			Plugin.Log.LogMessage("Bee on Hit Patch: Card bool" + Patch.Info.HasAbility(Ability.BeesOnHit));
		////			if (Patch.HasAbility(Ability.BeesOnHit))
		////            {
		////
		////				Plugin.Log.LogMessage("Bee on Hit Patch: Allowing Bee spawning");
		////				__result = true;
		////            } else
		////			{
		////				Plugin.Log.LogMessage("Bee on Hit Patch: Blocking bee spawning");
		////				__result = false;
		////
		////            }
		////		}
		////	}
	}









}
