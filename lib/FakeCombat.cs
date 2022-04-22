using System;
using System.Collections.Generic;
using DiskCardGame;
using UnityEngine;
using System.Collections;
using Pixelplacement;

namespace voidSigils
{
	public static class FakeCombat
	{

		public static int DamageDealtThisPhase { get; private set; }

		public static IEnumerator FakeCombatPhase(bool playerIsAttacker, SpecialBattleSequencer specialSequencer, List<CardSlot> attacker)
		{
			var holder = Singleton<CombatPhaseManager>.Instance;
            DamageDealtThisPhase = 0;
			if (Singleton<TurnManager>.Instance.SpecialSequencer != null)
			{
				specialSequencer = Singleton<TurnManager>.Instance.SpecialSequencer;
			}
			List<CardSlot> attackingSlots = attacker;
			attackingSlots.RemoveAll((CardSlot x) => x.Card == null || x.Card.Attack < 1);
			if (attackingSlots.Count > 0)
            {
				///break out of this if there are no cards with greater than 0 attack
				yield break;
			}
			bool atLeastOneAttacker = attackingSlots.Count > 0;
			yield return holder.InitializePhase(attackingSlots, playerIsAttacker);
			if (specialSequencer != null)
			{
				if (playerIsAttacker)
				{
					yield return specialSequencer.PlayerCombatStart();
				}
				else
				{
					yield return specialSequencer.OpponentCombatStart();
				}
			}
			if (atLeastOneAttacker)
			{
				bool attackedWithSquirrel = false;
				foreach (CardSlot cardSlot in attackingSlots)
				{
					cardSlot.Card.AttackedThisTurn = false;
					if (cardSlot.Card.Info.IsOfTribe(Tribe.Squirrel))
					{
						attackedWithSquirrel = true;
					}
				}
				foreach (CardSlot cardSlot2 in attackingSlots)
				{
					if (cardSlot2.Card != null && !cardSlot2.Card.AttackedThisTurn)
					{
						cardSlot2.Card.AttackedThisTurn = true;
						yield return SlotAttackSequence(cardSlot2);
					}
				}
				List<CardSlot>.Enumerator enumerator2 = default(List<CardSlot>.Enumerator);
				if (specialSequencer != null && playerIsAttacker)
				{
					yield return specialSequencer.PlayerCombatPostAttacks();
				}
				if (DamageDealtThisPhase > 0)
				{
					yield return new WaitForSeconds(0.4f);
					yield return holder.VisualizeDamageMovingToScales(playerIsAttacker);
					int excessDamage = 0;
					if (playerIsAttacker)
					{
						excessDamage = Singleton<LifeManager>.Instance.Balance + DamageDealtThisPhase - 5;
						if (attackedWithSquirrel && excessDamage >= 0)
						{
							AchievementManager.Unlock(Achievement.PART1_SPECIAL1);
						}
						excessDamage = Mathf.Max(0, excessDamage);
					}
					int damage = DamageDealtThisPhase - excessDamage;
					AscensionStatsData.TryIncreaseStat(AscensionStat.Type.MostDamageDealt, DamageDealtThisPhase);
					if (DamageDealtThisPhase >= 666)
					{
						AchievementManager.Unlock(Achievement.PART2_SPECIAL2);
					}
					if (!(specialSequencer != null) || !specialSequencer.PreventDamageAddedToScales)
					{
						yield return Singleton<LifeManager>.Instance.ShowDamageSequence(damage, damage, !playerIsAttacker, 0f, null, 0f, true);
					}
					if (specialSequencer != null)
					{
						yield return specialSequencer.DamageAddedToScale(damage + excessDamage, playerIsAttacker);
					}
					if ((!(specialSequencer != null) || !specialSequencer.PreventDamageAddedToScales) && excessDamage > 0 && Singleton<TurnManager>.Instance.Opponent.NumLives == 1 && Singleton<TurnManager>.Instance.Opponent.GiveCurrencyOnDefeat)
					{
						yield return Singleton<TurnManager>.Instance.Opponent.TryRevokeSurrender();
						RunState.Run.currency += excessDamage;
						yield return holder.VisualizeExcessLethalDamage(excessDamage, specialSequencer);
					}
				}
				yield return new WaitForSeconds(0.15f);
				foreach (CardSlot cardSlot3 in attackingSlots)
				{
					if (cardSlot3.Card != null && cardSlot3.Card.TriggerHandler.RespondsToTrigger(Trigger.AttackEnded, Array.Empty<object>()))
					{
						yield return cardSlot3.Card.TriggerHandler.OnTrigger(Trigger.AttackEnded, Array.Empty<object>());
					}
				}
				enumerator2 = default(List<CardSlot>.Enumerator);
			}
			if (specialSequencer != null)
			{
				if (playerIsAttacker)
				{
					yield return specialSequencer.PlayerCombatEnd();
				}
				else
				{
					yield return specialSequencer.OpponentCombatEnd();
				}
			}
			Singleton<ViewManager>.Instance.SwitchToView(Singleton<BoardManager>.Instance.defaultView, false, false);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			DamageDealtThisPhase = 0;
			if (atLeastOneAttacker)
			{
				yield return new WaitForSeconds(0.15f);
			}
			yield break;
		}

		public static IEnumerator SlotAttackSlot(CardSlot attackingSlot, CardSlot opposingSlot, float waitAfter = 0f)
		{
			var holder = Singleton<CombatPhaseManager>.Instance;
			yield return Singleton<GlobalTriggerHandler>.Instance.TriggerCardsOnBoard(Trigger.SlotTargetedForAttack, false, new object[]
			{
				opposingSlot,
				attackingSlot.Card
			});
			yield return new WaitForSeconds(0.025f);
			if (attackingSlot.Card != null)
			{
				if (attackingSlot.Card.Anim.DoingAttackAnimation)
				{
					yield return new WaitUntil(() => !attackingSlot.Card.Anim.DoingAttackAnimation);
					yield return new WaitForSeconds(0.25f);
				}
				if (opposingSlot.Card != null && attackingSlot.Card.AttackIsBlocked(opposingSlot))
				{
					ProgressionData.SetAbilityLearned(Ability.PreventAttack);
					yield return holder.ShowCardBlocked(attackingSlot.Card);
				}
				else if (attackingSlot.Card.CanAttackDirectly(opposingSlot))
				{
					DamageDealtThisPhase += attackingSlot.Card.Attack;
					yield return holder.VisualizeCardAttackingDirectly(attackingSlot, opposingSlot, attackingSlot.Card.Attack);
					if (attackingSlot.Card.TriggerHandler.RespondsToTrigger(Trigger.DealDamageDirectly, new object[]
					{
						attackingSlot.Card.Attack
					}))
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
					if (heightOffset > 0f)
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
					if (attackingCard != null && attackingCard.Slot != null)
					{
						attackingSlot = attackingCard.Slot;
						if (attackingSlot.Card.IsFlyingAttackingReach())
						{
							opposingSlot.Card.Anim.PlayJumpAnimation();
							yield return new WaitForSeconds(0.3f);
							attackingSlot.Card.Anim.PlayAttackInAirAnimation();
						}
						attackingSlot.Card.Anim.SetAnimationPaused(false);
						yield return new WaitForSeconds(0.05f);
						int overkillDamage = attackingSlot.Card.Attack - opposingSlot.Card.Health;
						yield return opposingSlot.Card.TakeDamage(attackingSlot.Card.Attack, attackingSlot.Card);
						yield return DealOverkillDamage(overkillDamage, attackingSlot, opposingSlot);
						if (attackingSlot.Card != null && heightOffset > 0f)
						{
							yield return Singleton<BoardManager>.Instance.AssignCardToSlot(attackingSlot.Card, attackingSlot.Card.Slot, 0.1f, null, false);
						}
					}
					attackingCard = null;
				}
				yield return new WaitForSeconds(waitAfter);
			}
			yield break;
		}

		public static IEnumerator SlotAttackSequence(CardSlot slot)
		{
			List<CardSlot> opposingSlots = slot.Card.GetOpposingSlots();
			Singleton<ViewManager>.Instance.SwitchToView(Singleton<BoardManager>.Instance.CombatView, false, false);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Locked;
			foreach (CardSlot opposingSlot in opposingSlots)
			{
				Singleton<ViewManager>.Instance.SwitchToView(Singleton<BoardManager>.Instance.CombatView, false, false);
				yield return SlotAttackSlot(slot, opposingSlot, (opposingSlots.Count > 1) ? 0.1f : 0f);
			}
			List<CardSlot>.Enumerator enumerator = default(List<CardSlot>.Enumerator);
			yield break;
		}

		public static IEnumerator DealOverkillDamage(int damage, CardSlot attackingSlot, CardSlot opposingSlot)
		{
			var holder = Singleton<CombatPhaseManager>.Instance;
			if (attackingSlot.Card != null && attackingSlot.IsPlayerSlot && damage > 0)
			{
				PlayableCard queuedCard = Singleton<BoardManager>.Instance.GetCardQueuedForSlot(opposingSlot);
				if (queuedCard != null)
				{
					yield return new WaitForSeconds(0.1f);
					Singleton<ViewManager>.Instance.SwitchToView(Singleton<BoardManager>.Instance.QueueView, false, false);
					yield return new WaitForSeconds(0.3f);
					if (queuedCard.HasAbility(Ability.PreventAttack))
					{
						yield return holder.ShowCardBlocked(attackingSlot.Card);
					}
					else
					{
						yield return queuedCard.TakeDamage(damage, attackingSlot.Card);
					}
				}
				queuedCard = null;
			}
			yield break;
		}
	}
}