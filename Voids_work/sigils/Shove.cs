using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddShove()
		{
			// setup ability
			const string rulebookName = "Ram";
			const string rulebookDescription = "[creature] will try to ram the card infront of it when played, or every upkeep till it succeeds once. It will send the rammed target to the queue if on my side, or back to the hand if on your side.";
			const string LearnDialogue = "Moving creatures around? How Rude!";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_shove);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Shove), tex, abIds);

			// set ability to behaviour class
			void_Shove.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Shove : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		PlayableCard target = null;

		public bool hasShoved = false;

		public override bool RespondsToResolveOnBoard()
		{
			if (hasShoved)
			{
				return false;
			}
			if (base.Card.Slot.IsPlayerSlot)
            {
				if (base.Card.slot.opposingSlot.Card != null
				&& base.Card.HasAbility(void_Shove.ability)
				&& base.Card.InOpponentQueue == false)
				{
					PlayableCard card = base.Card.Slot.opposingSlot.Card;
					if (card.Info.HasTrait(Trait.Uncuttable))
					{
						CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear(
							"That card resists your shove.", 2.5f, 0f, Emotion.Anger, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
						return false;
					}
					else if (card.Info.HasTrait(Trait.Giant))
					{
						CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear(
							"You can't shove the moon!", 2.5f, 0f, Emotion.Anger, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
						return false;
					}
					else
					{
						target = card;
						return base.Card.OnBoard;
					}
				}
			} else
            {
				if (base.Card.slot.opposingSlot.Card != null)
                {
					target = base.Card.slot.opposingSlot.Card;
					return base.Card.OnBoard;
                }
            }
			return false;
		}

		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			if (hasShoved)
			{
				return false;
			}
			if (base.Card.Slot.IsPlayerSlot && playerUpkeep == true)
			{
				if (base.Card.slot.opposingSlot.Card != null
				&& base.Card.HasAbility(void_Shove.ability)
				&& base.Card.InOpponentQueue == false)
				{
					PlayableCard card = base.Card.Slot.opposingSlot.Card;
					if (card.Info.HasTrait(Trait.Uncuttable))
					{
						CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear(
							"That card resists your shove.", 2.5f, 0f, Emotion.Anger, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
						return false;
					}
					else if (card.Info.HasTrait(Trait.Giant))
					{
						CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear(
							"You can't shove the moon!", 2.5f, 0f, Emotion.Anger, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
						return false;
					}
					else
					{
						target = card;
						return base.Card.OnBoard;
					}
				}
			}
			else
			{
				if (base.Card.slot.opposingSlot.Card != null)
				{
					target = base.Card.slot.opposingSlot.Card;
					return base.Card.OnBoard;
				}
			}
			return false;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			if (base.Card.Slot.IsPlayerSlot)
			{
				if (base.Card.slot.opposingSlot.Card != null
				&& base.Card.HasAbility(void_Shove.ability)
				&& base.Card.InOpponentQueue == false)
				{
					if (!target.FaceDown)
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
						hasShoved = true;
					}
				}
			}
			else
			{
				if (base.Card.slot.opposingSlot.Card != null)
				{
					yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(target.Info, null, 0.25f, null);
					UnityEngine.Object.Destroy(target.gameObject);
					hasShoved = true;
				}
			}		
			yield break;

		}
		protected virtual IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
		{
			yield break;
		}

		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{
			if (base.Card.Slot.IsPlayerSlot && playerUpkeep == true)
			{
				if (base.Card.slot.opposingSlot.Card != null
				&& base.Card.HasAbility(void_Shove.ability)
				&& base.Card.InOpponentQueue == false)
				{
					if (!target.FaceDown)
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
						hasShoved = true;
					}
				}
			}
			else
			{
				if (base.Card.slot.opposingSlot.Card != null)
				{
					yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(target.Info, null, 0.25f, null);
					target.Anim.PlayDeathAnimation();
					CardSlot slotBeforeDeath = target.slot;
					target.UnassignFromSlot();
					target.StartCoroutine(target.DestroyWhenStackIsClear());
					slotBeforeDeath = null;
					hasShoved = true;
				}
			}
			yield break;
		}


	}
}