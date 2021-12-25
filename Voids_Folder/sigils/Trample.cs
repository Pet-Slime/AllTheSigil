using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using HarmonyLib;
using Artwork = voidSigils.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Eri
		private NewAbility AddTrample()
		{
			// setup ability
			const string rulebookName = "Trample";
			const string rulebookDescription = "[creature] will deal overkill damage to the owner of the creature.";
			const string LearnDialogue = "A stampede can not be stopped.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 7, true);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_trample);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_trample), tex, abIds);

			// set ability to behaviour class
			void_trample.ability = newAbility.ability;

			return newAbility;
		}
	}


	// Code donated by julian

	[HarmonyPatch(typeof(CombatPhaseManager), nameof(CombatPhaseManager.DealOverkillDamage))]
	public class DealOverkillDamageTramplePatch
	{
		[HarmonyPrefix]
		public static void CheckCardForBrimstoneAbilityPatch(ref int damage, CardSlot attackingSlot, CardSlot opposingSlot)
		{
			bool attackingSlotIsPlayerCard = attackingSlot.Card is not null && attackingSlot.IsPlayerSlot;
			bool attackingSlotHasBrimstone =
				attackingSlotIsPlayerCard && attackingSlot.Card.Info.HasAbility(void_trample.ability);
			if (attackingSlotHasBrimstone)
			{
				Plugin.Log.LogDebug($"{SigilUtils.GetLogOfCardInSlot(attackingSlot.Card)} - Setting damage to 1 for Brimstone");
				damage = 1;
			}
		}
	}

	public class void_trample : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private bool willDealDamageToOpponent;

		int damage = 0;

		public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat,
			PlayableCard killer)
		{
			return killer == base.Card && damage > 0;
		}

		public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat,
			PlayableCard killer)
		{
			yield return base.PreSuccessfulTriggerSequence();
			if (willDealDamageToOpponent)
			{
				yield return base.LearnAbility(0.25f);
				yield return Singleton<LifeManager>.Instance.ShowDamageSequence(damage, damage, deathSlot.IsPlayerSlot);
			}

			yield break;
		}

		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			return attacker == Card;
		}

		public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			// Plugin.Log.LogDebug($"[OnSlotTargetedForAttack] Setting {SigilUtils.GetLogOfCardInSlot(attacker)} startedAttack to true");
			// card exists in opposing slot
			// AND, no card exists in queued slot BEHIND slot that was targeted
			if (slot.Card != null)
			{
				damage = attacker.Attack - slot.Card.Info.Health;
			}
			this.willDealDamageToOpponent = slot.Card && !Singleton<BoardManager>.Instance.GetCardQueuedForSlot(slot);
			yield break;
		}
	}

}