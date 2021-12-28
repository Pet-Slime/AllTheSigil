using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System;
using System.Collections.Generic;
using Pixelplacement;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private NewAbility AddStampede()
		{
			// setup ability
			const string rulebookName = "Stampede";
			const string rulebookDescription = "[creature] will cause adjacent creatures to attack when played on the board.";
			const string LearnDialogue = "Power in Numbers";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_ignit);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_ignite), tex, abIds);

			// set ability to behaviour class
			void_ignite.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_ignite : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public int DamageDealtThisPhase { get; private set; }

		public override bool RespondsToResolveOnBoard()
		{
			return base.Card.HasAbility(void_ignite.ability);
		}


		public override IEnumerator OnResolveOnBoard()
		{

			CardSlot slot = base.Card.slot;
			List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(slot);
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < slot.Index)
			{
				if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
				{
					CardSlot attacker = adjacentSlots[0].Card.Slot;
					yield return new WaitForSeconds(0.1f);
					yield return this.FakeCombat(attacker.IsPlayerSlot, null, attacker);
					yield return new WaitForSeconds(0.1f);
				}
				adjacentSlots.RemoveAt(0);
			}
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
			{
				CardSlot attacker = adjacentSlots[0].Card.Slot;
				yield return new WaitForSeconds(0.1f);
				yield return this.FakeCombat(attacker.IsPlayerSlot, null, attacker);
				yield return new WaitForSeconds(0.1f);
			}
			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.1f);
			yield break;

		}

		public IEnumerator FakeCombat(bool playerIsAttacker, SpecialBattleSequencer specialSequencer, CardSlot attacker)
		{
			this.DamageDealtThisPhase = 0;
			yield return this.SlotAttackSequence(attacker);
			bool flag6 = this.DamageDealtThisPhase > 0;
			if (flag6)
			{
				yield return new WaitForSeconds(0.4f);
				yield return Singleton<CombatPhaseManager>.Instance.VisualizeDamageMovingToScales(playerIsAttacker);
				int excessDamage = 0;
				if (playerIsAttacker)
				{
					excessDamage = Singleton<LifeManager>.Instance.Balance + this.DamageDealtThisPhase - 5;
					excessDamage = Mathf.Max(0, excessDamage);
				}
				int damage = this.DamageDealtThisPhase - excessDamage;
				bool flag8 = this.DamageDealtThisPhase >= 666;
				if (flag8)
				{
					AchievementManager.Unlock(Achievement.PART2_SPECIAL2);
				}
				bool flag9 = !(specialSequencer != null) || !specialSequencer.PreventDamageAddedToScales;
				if (flag9)
				{
					yield return Singleton<LifeManager>.Instance.ShowDamageSequence(damage, damage, !playerIsAttacker, 0f, null, 0f);
				}
				bool flag10 = specialSequencer != null;
				if (flag10)
				{
					yield return specialSequencer.DamageAddedToScale(damage + excessDamage, playerIsAttacker);
				}
				bool flag11 = (!(specialSequencer != null) || !specialSequencer.PreventDamageAddedToScales) && excessDamage > 0 && Singleton<TurnManager>.Instance.Opponent.NumLives == 1 && Singleton<TurnManager>.Instance.Opponent.GiveCurrencyOnDefeat;
				if (flag11)
				{
					yield return Singleton<TurnManager>.Instance.Opponent.TryRevokeSurrender();
					RunState.Run.currency += excessDamage;
					yield return Singleton<CombatPhaseManager>.Instance.VisualizeExcessLethalDamage(excessDamage, specialSequencer);
				}
			}
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
		}

		public IEnumerator SlotAttackSlot(CardSlot attackingSlot, CardSlot opposingSlot, float waitAfter = 0f)
		{
			yield return Singleton<GlobalTriggerHandler>.Instance.TriggerCardsOnBoard(Trigger.SlotTargetedForAttack, false, new object[]
			{
				opposingSlot,
				attackingSlot.Card
			});
			yield return new WaitForSeconds(0.025f);
			bool flag = attackingSlot.Card != null;
			if (flag)
			{
				bool doingAttackAnimation = attackingSlot.Card.Anim.DoingAttackAnimation;
				if (doingAttackAnimation)
				{
					yield return new WaitUntil(() => !attackingSlot.Card.Anim.DoingAttackAnimation);
					yield return new WaitForSeconds(0.25f);
				}
				bool flag2 = opposingSlot.Card != null && attackingSlot.Card.AttackIsBlocked(opposingSlot);
				if (flag2)
				{
					ProgressionData.SetAbilityLearned(Ability.PreventAttack);
					yield return Singleton<CombatPhaseManager>.Instance.ShowCardBlocked(attackingSlot.Card);
				}
				else
				{
					bool flag3 = attackingSlot.Card.CanAttackDirectly(opposingSlot);
					if (flag3)
					{
						this.DamageDealtThisPhase += attackingSlot.Card.Attack;
						yield return Singleton<CombatPhaseManager>.Instance.VisualizeCardAttackingDirectly(attackingSlot, opposingSlot, attackingSlot.Card.Attack);
						bool flag4 = attackingSlot.Card.TriggerHandler.RespondsToTrigger(Trigger.DealDamageDirectly, new object[]
						{
							attackingSlot.Card.Attack
						});
						if (flag4)
						{
							yield return attackingSlot.Card.TriggerHandler.OnTrigger(Trigger.DealDamageDirectly, new object[]
							{
								attackingSlot.Card.Attack
							});
						}
					}
					else
					{
						float heightOffset = (opposingSlot.Card == null) ? 0f : opposingSlot.Card.SlotHeightOffset;
						bool flag5 = heightOffset > 0f;
						if (flag5)
						{
							Tween.Position(attackingSlot.Card.transform, attackingSlot.Card.transform.position + Vector3.up * heightOffset, 0.05f, 0f, Tween.EaseInOut, Tween.LoopType.None, null, null, true);
						}
						attackingSlot.Card.Anim.PlayAttackAnimation(attackingSlot.Card.IsFlyingAttackingReach(), opposingSlot, null);
						yield return new WaitForSeconds(0.07f);
						attackingSlot.Card.Anim.SetAnimationPaused(true);
						PlayableCard attackingCard = attackingSlot.Card;
						yield return Singleton<GlobalTriggerHandler>.Instance.TriggerCardsOnBoard(Trigger.CardGettingAttacked, false, new object[]
						{
							opposingSlot.Card
						});
						bool flag6 = attackingCard != null && attackingCard.Slot != null;
						if (flag6)
						{
							attackingSlot = attackingCard.Slot;
							bool flag7 = attackingSlot.Card.IsFlyingAttackingReach();
							if (flag7)
							{
								opposingSlot.Card.Anim.PlayJumpAnimation();
								yield return new WaitForSeconds(0.3f);
								attackingSlot.Card.Anim.PlayAttackInAirAnimation();
							}
							attackingSlot.Card.Anim.SetAnimationPaused(false);
							yield return new WaitForSeconds(0.05f);
							int overkillDamage = attackingSlot.Card.Attack - opposingSlot.Card.Health;
							yield return opposingSlot.Card.TakeDamage(attackingSlot.Card.Attack, attackingSlot.Card);
							yield return this.DealOverkillDamage(overkillDamage, attackingSlot, opposingSlot);
							bool flag8 = attackingSlot.Card != null && heightOffset > 0f;
							if (flag8)
							{
								yield return Singleton<BoardManager>.Instance.AssignCardToSlot(attackingSlot.Card, attackingSlot.Card.Slot, 0.1f, null, false);
							}
						}
						attackingCard = null;
					}
				}
				yield return new WaitForSeconds(waitAfter);
			}
			yield break;
		}

		private IEnumerator SlotAttackSequence(CardSlot slot)
		{
			List<CardSlot> opposingSlots = slot.Card.GetOpposingSlots();
			Singleton<ViewManager>.Instance.SwitchToView(Singleton<BoardManager>.Instance.CombatView, false, false);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Locked;
			foreach (CardSlot opposingSlot in opposingSlots)
			{
				Singleton<ViewManager>.Instance.SwitchToView(Singleton<BoardManager>.Instance.CombatView, false, false);
				yield return this.SlotAttackSlot(slot, opposingSlot, (opposingSlots.Count > 1) ? 0.1f : 0f);
			}
			List<CardSlot>.Enumerator enumerator = default(List<CardSlot>.Enumerator);
			Singleton<CombatPhaseManager>.Instance.VisualizeClearSniperAbility();
			yield break;
		}

		protected virtual IEnumerator DealOverkillDamage(int damage, CardSlot attackingSlot, CardSlot opposingSlot)
		{
			bool flag = attackingSlot.Card != null && attackingSlot.IsPlayerSlot && damage > 0;
			if (flag)
			{
				PlayableCard queuedCard = Singleton<BoardManager>.Instance.GetCardQueuedForSlot(opposingSlot);
				bool flag2 = queuedCard != null;
				if (flag2)
				{
					yield return new WaitForSeconds(0.1f);
					Singleton<ViewManager>.Instance.SwitchToView(Singleton<BoardManager>.Instance.QueueView, false, false);
					yield return new WaitForSeconds(0.3f);
					bool flag3 = queuedCard.HasAbility(Ability.PreventAttack);
					if (flag3)
					{
						yield return Singleton<CombatPhaseManager>.Instance.ShowCardBlocked(attackingSlot.Card);
					}
					else
					{

						yield return Singleton<CombatPhaseManager>.Instance.PreOverkillDamage(queuedCard);
						yield return queuedCard.TakeDamage(damage, attackingSlot.Card);
						yield return Singleton<CombatPhaseManager>.Instance.PostOverkillDamage(queuedCard);
					}
				}
				queuedCard = null;
			}
			yield break;
		}
	}
}