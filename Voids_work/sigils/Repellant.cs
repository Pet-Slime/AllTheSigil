using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System;
using Pixelplacement;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private void AddRepellant()
		{
			// setup ability
			const string rulebookName = "Repellant";
			const string rulebookDescription = "When [creature] perishes, the creature that killed it gets pushed into the back row.";
			const string LearnDialogue = "Foul";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Repellant);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Repellant_a2);
			int powerlevel = 3;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Repellant.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Repellant), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Repellant : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		PlayableCard target = null;

		public override bool RespondsToPreDeathAnimation(bool wasSacrifice)
		{
			if (base.Card.slot.opposingSlot.Card != null 
				&& base.Card.HasAbility(void_Repellant.ability)
				&& base.Card.InOpponentQueue == false)
            {
				PlayableCard card = base.Card.Slot.opposingSlot.Card;

				if (card.Info.HasTrait(Trait.Uncuttable))
                {
					CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear(
						"That card resists your repel.", 2.5f, 0f, Emotion.Anger, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
					return false;

                } else if (card.Info.HasTrait(Trait.Giant)) {

					CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear(
						"You can't repel the moon!", 2.5f, 0f, Emotion.Anger, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
					return false;

				} else {

					target = card;
					return base.Card.OnBoard;
				}
            }

			return false;
		}

		public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
		{


			if (!target.FaceDown && (!target.HasAbility(Ability.Flying) || base.Card.HasAbility(Ability.Reach)))
			{
				CardSlot oldSlot = target.slot;
				Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
				yield return new WaitForSeconds(0.1f);
				yield return new WaitForSeconds(0.1f);
				target.UnassignFromSlot();
				yield return Singleton<TurnManager>.Instance.Opponent.ReturnCardToQueue(target, 0.25f);
				yield return this.PostSuccessfulMoveSequence(oldSlot);
				yield return new WaitForSeconds(0.4f);
				yield return base.LearnAbility(0.25f);
				yield return new WaitForSeconds(0.1f);
			}
			yield break;

		}
		protected virtual IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
		{
			yield break;
		}

	}
}