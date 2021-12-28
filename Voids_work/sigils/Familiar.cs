using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private NewAbility AddFamiliar()
		{
			// setup ability
			const string rulebookName = "Familiar";
			const string rulebookDescription = "A familiar will help with attacking when it's adjacent allies attack.";
			const string LearnDialogue = "A familiar helps those in need.";
			// const string TextureFile = "Artwork/void_vicious.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3, true);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_familair);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Familiar), tex, abIds);

			// set ability to behaviour class
			void_Familiar.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Familiar : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private PlayableCard LeftSlot = null;

		private PlayableCard RightSlot = null;


		private PlayableCard Previous_target = null;


		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			return true;
		}

		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{
			//On upkeep, no worry about softlocks so clear previous target
			Previous_target = null;
			yield break;
		}



		public override bool RespondsToOtherCardDealtDamage(PlayableCard attacker, int amount, PlayableCard target)
		{
			CardSlot slotSaved = base.Card.slot;

			//If target is equal to previous target, we somehow entered a softlock loop. Return false to break it and clear previous target
			if (Previous_target == target)
            {
				Previous_target = null;
				return false;
            } else
            {
				return true;
            }
		}

		public override IEnumerator OnOtherCardDealtDamage(PlayableCard attacker, int amount, PlayableCard target)
		{
			CardSlot slotSaved = base.Card.slot;
			yield return new WaitForSeconds(0.1f);
			List<CardSlot> nextTo = Singleton<BoardManager>.Instance.GetAdjacentSlots(slotSaved);
			List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(slotSaved.opposingSlot);
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < slotSaved.Index)
			{
				if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead && adjacentSlots[0].Card == target)
				{
					if (nextTo.Count > 0 && nextTo[0].Index < slotSaved.Index)
					{
						if (nextTo[0].Card != null && !nextTo[0].Card.Dead && nextTo[0].Card == attacker)
						{
							LeftSlot = target;
							RightSlot = attacker;
							yield return new WaitForSeconds(0.1f);
							yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(slotSaved, target.slot);
							yield return new WaitForSeconds(0.1f);
						}
						
					}
				}
				adjacentSlots.RemoveAt(0);
				nextTo.RemoveAt(0);
			}
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead && adjacentSlots[0].Card == target)
			{
				if (nextTo.Count > 0 && nextTo[0].Card != null && !nextTo[0].Card.Dead && nextTo[0].Card == attacker)
				{
					LeftSlot = target;
					RightSlot = attacker;
					yield return new WaitForSeconds(0.1f);
					yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(slotSaved, target.slot);
					yield return new WaitForSeconds(0.1f);
				}
			}

			Plugin.Log.LogMessage("Familiar attacker data: " + RightSlot);
			Plugin.Log.LogMessage("Familiar target data: " + RightSlot);
			yield return new WaitForSeconds(0.1f);
			Previous_target = target;
			LeftSlot = null;
			RightSlot = null;
			yield return new WaitForSeconds(0.1f);
			yield break;
		}
	}
}