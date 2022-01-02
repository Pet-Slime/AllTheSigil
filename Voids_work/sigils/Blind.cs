using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using APIPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using Random = UnityEngine.Random;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddBlind()
		{
			// setup ability
			const string rulebookName = "Blind";
			const string rulebookDescription = "[creature] is blind, and will attack randomly.";
			const string LearnDialogue = "A protector, till the very end.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 0);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Blind);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Blind), tex, abIds);

			// set ability to behaviour class
			void_Blind.ability = newAbility.ability;

			

			return newAbility;
		}
	}

	public class void_Blind : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			return attacker = base.Card;
		}


		public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			List<CardSlot> slots = new List<CardSlot>();
			if (attacker.slot.IsPlayerSlot)
            {
				slots = Singleton<BoardManager>.Instance.opponentSlots;
			} else
            {
				slots = Singleton<BoardManager>.Instance.playerSlots;
			}


			List<PlayableCard> targets = new List<PlayableCard>();
			for (int index = 0; index < slots.Count; index++)
			{
				if (slots[index].Card != null)
				{
					targets.Add(slots[index].Card);
				}
			}

			PlayableCard target = targets[Random.Range(0, (targets.Count))];

			
			if (attacker.HasAbility(Ability.SplitStrike))
            {
				yield return new WaitForSeconds(0.25f);
				target = targets[Random.Range(0, (targets.Count))];
				yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(attacker.slot, target.slot);

				yield return new WaitForSeconds(0.25f);
				target = targets[Random.Range(0, (targets.Count))];
				yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(attacker.slot, target.slot);

			} else if (attacker.HasAbility(Ability.TriStrike) || attacker.HasAbility(Ability.CellTriStrike))
			{
				yield return new WaitForSeconds(0.25f);
				target = targets[Random.Range(0, (targets.Count))];
				yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(attacker.slot, target.slot);
				yield return new WaitForSeconds(0.25f);
				target = targets[Random.Range(0, (targets.Count))];
				yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(attacker.slot, target.slot);
				yield return new WaitForSeconds(0.25f);
				target = targets[Random.Range(0, (targets.Count))];
				yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(attacker.slot, target.slot);
			} else
            {
				yield return new WaitForSeconds(0.25f);
				yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(attacker.slot, target.slot);
			}
			yield return new WaitForSeconds(0.25f);
			yield return base.LearnAbility(0.25f);
			yield break;
		}
	}

	[HarmonyPatch(typeof(PlayableCard), "AttackIsBlocked", MethodType.Normal)]
	public class AttackIsBlocked_Blind_Patch
	{

		[HarmonyPostfix]
		public static void Postfix(ref CardSlot opposingSlot, ref bool __result, PlayableCard __instance)
		{
			if (__instance.HasAbility(void_Blind.ability) && !__instance.HasAbility(Ability.AllStrike))
			{
				__result = true;
			}
		}
	}
}