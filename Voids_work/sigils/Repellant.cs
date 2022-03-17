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
		private NewAbility AddRepellant()
		{
			// setup ability
			const string rulebookName = "Repellant";
			const string rulebookDescription = "When [creature] perishes, the creature that killed it gets pushed into the back row.";
			const string LearnDialogue = "Foul";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.void_Repellant_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Repellant);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_repellant), tex, abIds);

			// set ability to behaviour class
			void_repellant.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_repellant : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		PlayableCard target = null;

		public override bool RespondsToPreDeathAnimation(bool wasSacrifice)
		{
			if (base.Card.slot.opposingSlot.Card != null 
				&& base.Card.HasAbility(void_repellant.ability)
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