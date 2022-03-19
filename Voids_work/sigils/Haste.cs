using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using HarmonyLib;
using System.Collections.Generic;
using Pixelplacement;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private void AddHaste()
		{
			// setup ability
			const string rulebookName = "Haste";
			const string rulebookDescription = "[creature] will attack as soon as it gets played on the board.";
			const string LearnDialogue = "Speed";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Haste);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Haste_a2);
			int powerlevel = 4;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Haste.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Haste), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Haste : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


		public int DamageDealtThisPhase { get; private set; }

		public override bool RespondsToResolveOnBoard()
		{
			return base.Card.HasAbility(void_Haste.ability);
		}


		public override IEnumerator OnResolveOnBoard()
		{
			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.1f);
			Plugin.Log.LogWarning(Plugin.voidCombatPhase);
			if (base.Card.Attack > 0 && Plugin.voidCombatPhase == false)
			{
				yield return this.FakeCombat(base.Card.slot.IsPlayerSlot, null, base.Card.slot);
			} else
            {
				yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSequence(base.Card.slot);
			}
			yield return new WaitForSeconds(0.1f);
			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.1f);
			yield break;

		}

		public IEnumerator FakeCombat(bool playerIsAttacker, SpecialBattleSequencer specialSequencer, CardSlot attacker)
		{
			var holder = Singleton<CombatPhaseManager>.Instance;
			this.DamageDealtThisPhase = 0;
			List<CardSlot> attackingSlots = new List<CardSlot>();
			attackingSlots.Add(attacker);
			attackingSlots.RemoveAll((CardSlot x) => x.Card == null || x.Card.Attack == 0);
			bool atLeastOneAttacker = attackingSlots.Count > 0;
			yield return holder.InitializePhase(attackingSlots, playerIsAttacker);
			bool flag = specialSequencer != null;
			if (flag)
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
			bool flag2 = atLeastOneAttacker;
			if (flag2)
			{
				bool attackedWithSquirrel = false;
				foreach (CardSlot slot in attackingSlots)
				{
					slot.Card.AttackedThisTurn = false;
					bool flag3 = slot.Card.Info.IsOfTribe(Tribe.Squirrel);
					if (flag3)
					{
						attackedWithSquirrel = true;
					}
				}
#pragma warning disable CS0219 // Variable is assigned but its value is never used
				List<CardSlot>.Enumerator enumerator = default(List<CardSlot>.Enumerator);
#pragma warning restore CS0219 // Variable is assigned but its value is never used
				foreach (CardSlot slot2 in attackingSlots)
				{
					bool flag4 = slot2.Card != null && !slot2.Card.AttackedThisTurn;
					if (flag4)
					{
						slot2.Card.AttackedThisTurn = true;
						yield return this.SlotAttackSequence(slot2);
					}
				}
#pragma warning disable CS0219 // Variable is assigned but its value is never used
				List<CardSlot>.Enumerator enumerator2 = default(List<CardSlot>.Enumerator);
#pragma warning restore CS0219 // Variable is assigned but its value is never used
				bool flag5 = specialSequencer != null && playerIsAttacker;
				if (flag5)
				{
					yield return specialSequencer.PlayerCombatPostAttacks();
				}
				bool flag6 = this.DamageDealtThisPhase > 0;
				if (flag6)
				{
					yield return new WaitForSeconds(0.4f);
					yield return holder.VisualizeDamageMovingToScales(playerIsAttacker);
					int excessDamage = 0;
					if (playerIsAttacker)
					{
						excessDamage = Singleton<LifeManager>.Instance.Balance + this.DamageDealtThisPhase - 5;
						bool flag7 = attackedWithSquirrel && excessDamage >= 0;
						if (flag7)
						{
							AchievementManager.Unlock(Achievement.PART1_SPECIAL1);
						}
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
						yield return ShowDamageSequence(damage, damage, !playerIsAttacker, 0f, null, 0f, true);
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
						yield return holder.VisualizeExcessLethalDamage(excessDamage, specialSequencer);
					}
				}
				yield return new WaitForSeconds(0.15f);
			}
			bool flag13 = specialSequencer != null;
			if (flag13)
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
			bool flag14 = atLeastOneAttacker;
			if (flag14)
			{
				yield return new WaitForSeconds(0.15f);
			}
			yield break;
		}

		public IEnumerator SlotAttackSlot(CardSlot attackingSlot, CardSlot opposingSlot, float waitAfter = 0f)
		{
			var holder = Singleton<CombatPhaseManager>.Instance;
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
					yield return holder.ShowCardBlocked(attackingSlot.Card);
				}
				else
				{
					bool flag3 = attackingSlot.Card.CanAttackDirectly(opposingSlot);
					if (flag3)
					{
						this.DamageDealtThisPhase += attackingSlot.Card.Attack;
						yield return holder.VisualizeCardAttackingDirectly(attackingSlot, opposingSlot, attackingSlot.Card.Attack);
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
			Singleton<CombatPhaseManager>.Instance.VisualizeClearSniperAbility();
			yield break;
		}

		protected virtual IEnumerator DealOverkillDamage(int damage, CardSlot attackingSlot, CardSlot opposingSlot)
		{
			var holder = Singleton<CombatPhaseManager>.Instance;
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