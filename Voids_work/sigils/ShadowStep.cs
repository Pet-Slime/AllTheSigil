using System.Collections;
using UnityEngine;
using DiskCardGame;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System.Collections.Generic;
using HarmonyLib;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private void AddShadowStep()
		{
			// setup ability
			const string rulebookName = "Shadow Step";
			const string rulebookDescription = "If the card opposing [creature] moves, [creature] will try to move with it";
			const string LearnDialogue = "The trail they leave behind, hurts.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_ShadowStep);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_ShadowStep_a2);
			int powerlevel = 1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;



			// set ability to behaviour class
			void_ShadowStep.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_ShadowStep), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_ShadowStep : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


		public override bool RespondsToResolveOnBoard()
		{
			return true;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
			yield return new WaitForSeconds(0.15f);
			yield return base.PreSuccessfulTriggerSequence();
			base.Card.SetFaceDown(true, false);
///			this.setCarback(base.Card);
			base.Card.UpdateFaceUpOnBoardEffects();
			this.OnResurface();
			yield return new WaitForSeconds(0.3f);
			this.triggerPriority = int.MinValue;
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}

		public override bool RespondsToTurnEnd(bool playerTurnEnd)
		{
			return base.Card != null && base.Card.OpponentCard != playerTurnEnd;
		}

		public override IEnumerator OnTurnEnd(bool playerTurnEnd)
		{
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
			yield return new WaitForSeconds(0.15f);
			yield return base.PreSuccessfulTriggerSequence();
			base.Card.SetFaceDown(false, false);
///			this.setCarback(base.Card);
			base.Card.UpdateFaceUpOnBoardEffects();
			this.OnResurface();
			yield return new WaitForSeconds(0.3f);
			this.triggerPriority = int.MinValue;
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}

		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			return base.Card.OpponentCard != playerUpkeep;
		}

		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{

			List<CardSlot> slots = Singleton<BoardManager>.Instance.GetSlots(base.Card.slot.IsPlayerSlot);
			bool othercards = false;
			for (var j = 0; j < slots.Count; j++)
			{
				if (slots[j].Card != null && !slots[j].Card.HasAbility(void_ShadowStep.ability))
                {
					othercards = true;
				}
            }
			if (othercards)
            {
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
				yield return new WaitForSeconds(0.15f);
				yield return base.PreSuccessfulTriggerSequence();
				base.Card.SetFaceDown(true, false);
///				this.setCarback(base.Card);
				base.Card.UpdateFaceUpOnBoardEffects();
				this.OnResurface();
				yield return new WaitForSeconds(0.3f);
				this.triggerPriority = int.MinValue;
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
				yield break;
			}
			yield break;
		}

		protected virtual void OnResurface()
		{
		}


		protected virtual void setCarback(PlayableCard card)
		{
			if (!SaveManager.SaveFile.IsPart2)
			{
				card.SetCardback(SigilUtils.LoadTextureFromResource(Artwork.void_ShadowStep_back));
			}
			else
			{
				card.SetCardback(SigilUtils.LoadTextureFromResource(Artwork.void_ShadowStep_back_a2));
			}
		}

		private int triggerPriority = int.MinValue;


		[HarmonyPatch(typeof(CombatPhaseManager), "SlotAttackSequence", MethodType.Normal)]
		public class AttackIsBlocked_ShadowStep_Patch
		{
			[HarmonyPrefix]
			public static bool SlotAttackSequence(CardSlot slot)
			{
				if (slot.Card.HasAbility(void_ShadowStep.ability))
				{
					//skip combat and do nothing
					return false;
				} else
                {
					//do combat
					return true;
                }
			}
		}

		[HarmonyPatch(typeof(PaperCardAnimationController), "NegationEffect", MethodType.Normal)]
		public class Sacrifice_ShadowStep_Patch
		{
			[HarmonyPostfix]
			public static void ChooseSacrificesForCard()
			{

			}
		}
	}
}