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

		private int triggerPriority = int.MinValue;


		public override bool RespondsToResolveOnBoard()
		{
			return true;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
			yield return new WaitForSeconds(0.15f);
			setCarback(base.Card);
			base.Card.SetFaceDown(true, false);
			yield return new WaitForSeconds(0.3f);
			yield return base.LearnAbility(0f);
			this.triggerPriority = int.MaxValue;
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
				yield return new WaitForSeconds(0.15f);
				yield return base.PreSuccessfulTriggerSequence();
				base.Card.SetFaceDown(false, false);
				base.Card.UpdateFaceUpOnBoardEffects();
				this.OnResurface();
				yield return new WaitForSeconds(0.3f);
				this.triggerPriority = int.MinValue;
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
	}
}