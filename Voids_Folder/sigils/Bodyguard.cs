using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using APIPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddBodyguard()
		{
			// setup ability
			const string rulebookName = "Bodyguard";
			const string rulebookDescription = "[creature] will take attacks for it's adjacent allies.";
			const string LearnDialogue = "A protector, till the very end.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 8);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_bodyguard);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_bodyguard), tex, abIds);

			// set ability to behaviour class
			void_bodyguard.ability = newAbility.ability;

			

			return newAbility;
		}
	}

	public class void_bodyguard : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			return true;
		}


		public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			PlayableCard protector = base.Card;
			List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(protector.slot);
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < protector.slot.Index)
			{
				if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
				{
					if (adjacentSlots[0] == slot && !adjacentSlots[0].Card.Info.HasAbility(void_bodyguard.ability))
					{
						yield return new WaitForSeconds(0.25f);
						yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(attacker.slot, protector.slot);
						yield return new WaitForSeconds(0.25f);
					}
				}
				adjacentSlots.RemoveAt(0);
			}
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
			{
				if (adjacentSlots[0] == slot && !adjacentSlots[0].Card.Info.HasAbility(void_bodyguard.ability))
				{
					yield return new WaitForSeconds(0.25f);
					yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(attacker.slot, protector.slot);
					yield return new WaitForSeconds(0.25f);
				}
			}
			yield break;
		}


	}

	[HarmonyPatch(typeof(PlayableCard), "AttackIsBlocked", MethodType.Normal)]
	public class AttackIsBlocked_Bodyguard_Patch
	{

        [HarmonyPostfix]
		public static void Postfix(ref CardSlot opposingSlot, ref bool __result)
		{
			
			if (opposingSlot.Card != null)
            {
				Plugin.Log.LogMessage("bodyguard test succesfully patched");
				List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(opposingSlot);

				if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < opposingSlot.Index)
				{
					if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
					{
						Plugin.Log.LogMessage("Bodyguard test: card: " + adjacentSlots[0].Card);
						Plugin.Log.LogMessage("Bodyguard test: ability: " + adjacentSlots[0].Card.Info.HasAbility(void_bodyguard.ability));
						if (adjacentSlots[0].Card.Info.HasAbility(void_bodyguard.ability))
                        {
							__result = true;
                        }
					}
					adjacentSlots.RemoveAt(0);
				}
				if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
				{
					Plugin.Log.LogMessage("bodyguard test: card: " + adjacentSlots[0].Card);
					Plugin.Log.LogMessage("bodyguard test: ability: " + adjacentSlots[0].Card.Info.HasAbility(void_bodyguard.ability));
					if (adjacentSlots[0].Card.Info.HasAbility(void_bodyguard.ability))
					{
						__result = true;
					}
				}
				Plugin.Log.LogMessage(__result);
			}



		}
	}
}