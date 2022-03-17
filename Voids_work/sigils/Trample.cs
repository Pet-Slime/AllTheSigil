using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using HarmonyLib;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Eri
		private void AddTrample()
		{
			// setup ability
			const string rulebookName = "Trample";
			const string rulebookDescription = "When [creature] deals overkill damage to a card, the overkill damage will be sent to the owner.";
			const string LearnDialogue = "A stampede can not be stopped.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Trample);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Trample_a2);
			int powerlevel = 6;
			bool LeshyUsable = Plugin.configTrample.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Trample.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Trample), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
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
				attackingSlotIsPlayerCard && attackingSlot.Card.Info.HasAbility(void_Trample.ability);
			if (attackingSlotHasBrimstone)
			{
				Plugin.Log.LogDebug($"{SigilUtils.GetLogOfCardInSlot(attackingSlot.Card)} - Setting damage to 1 for Brimstone");
				damage = 1;
			}
		}
	}

	public class void_Trample : AbilityBehaviour
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
				yield return ShowDamageSequence(damage, damage, deathSlot.IsPlayerSlot);
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

		[HarmonyPatch(typeof(CombatPhaseManager), nameof(CombatPhaseManager.DealOverkillDamage))]
		public class TramplePatch
		{
			[HarmonyPrefix]
			public static void Prefix(ref int damage, ref CardSlot attackingSlot, ref CardSlot opposingSlot)
			{
				if (attackingSlot.Card != null && damage > 0 && attackingSlot.Card.HasAbility(void_Trample.ability))
                {
					damage = 0;
				}
			}
		}

		//Port KCM damage formula to fix sigils that deal damage to leshy
		public IEnumerator ShowDamageSequence(int damage, int numWeights, bool toPlayer, float waitAfter = 0.125f, GameObject alternateWeightPrefab = null, float waitBeforeCalcDamage = 0f, bool changeView = true)
		{
			bool flag = damage > 1 && Singleton<OpponentAnimationController>.Instance != null;
			if (flag)
			{
				bool flag2 = P03AnimationController.Instance != null && P03AnimationController.Instance.CurrentFace == P03AnimationController.Face.Default;
				if (flag2)
				{
					P03AnimationController.Instance.SwitchToFace(toPlayer ? P03AnimationController.Face.Happy : P03AnimationController.Face.Angry, false, true);
				}
				else
				{
					bool flag3 = Singleton<LifeManager>.Instance.scales != null;
					if (flag3)
					{
						Singleton<OpponentAnimationController>.Instance.SetLookTarget(Singleton<LifeManager>.Instance.scales.transform, Vector3.up * 2f);
					}
				}
			}
			bool flag4 = Singleton<LifeManager>.Instance.scales != null;
			if (flag4)
			{
				if (changeView)
				{
					Singleton<ViewManager>.Instance.SwitchToView(Singleton<LifeManager>.Instance.scalesView, false, false);
					yield return new WaitForSeconds(0.1f);
				}
				yield return Singleton<LifeManager>.Instance.scales.AddDamage(damage, numWeights, toPlayer, alternateWeightPrefab);
				bool flag5 = waitBeforeCalcDamage > 0f;
				if (flag5)
				{
					yield return new WaitForSeconds(waitBeforeCalcDamage);
				}
				if (toPlayer)
				{
					Singleton<LifeManager>.Instance.PlayerDamage += damage;
				}
				else
				{
					Singleton<LifeManager>.Instance.OpponentDamage += damage;
				}
				yield return new WaitForSeconds(waitAfter);
			}
			bool flag6 = Singleton<OpponentAnimationController>.Instance != null;
			if (flag6)
			{
				bool flag7 = P03AnimationController.Instance != null && (P03AnimationController.Instance.CurrentFace == P03AnimationController.Face.Angry || P03AnimationController.Instance.CurrentFace == P03AnimationController.Face.Happy);
				if (flag7)
				{
					P03AnimationController.Instance.PlayFaceStatic();
					P03AnimationController.Instance.SwitchToFace(P03AnimationController.Face.Default, false, false);
				}
				else
				{
					Singleton<OpponentAnimationController>.Instance.ClearLookTarget();
				}
			}
			yield break;
		}
	}

}