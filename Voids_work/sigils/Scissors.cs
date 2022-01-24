using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System;
using System.Collections.Generic;
using Pixelplacement;
using Object = UnityEngine.Object;

namespace voidSigils
{
	public partial class Plugin
	{
		//Ported from the Zerg mod, with permission from James
		private NewAbility AddScissors()
		{
			// setup ability
			const string rulebookName = "Scissors";
			const string rulebookDescription = "When a card bearing this sigil is played, a targeted card cut in two.";
			const string LearnDialogue = "My card!";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 0);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.scissors_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_scissors);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Scissors), tex, abIds);

			// set ability to behaviour class
			void_Scissors.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Scissors : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToResolveOnBoard()
		{
			return SigilUtils.GetSlot(Card).IsPlayerSlot;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			if (GetValidTargets().Count == 0)
			{
				Card.Anim.StrongNegationEffect();
				yield return new WaitForSeconds(0.3f);
				yield break;
			}

			yield return ActivateSequence();

			Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
			yield return new WaitForSeconds(0.1f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
		}

		private IEnumerator ActivateSequence()
		{
			yield return new WaitForSeconds(0.1f);
			Singleton<UIManager>.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(0.6f, 0.2f);
			Singleton<ViewManager>.Instance.SwitchToView(View.OpponentQueue, false, false);
			yield return new WaitForSeconds(0.25f);
			Transform firstPersonItem = Singleton<FirstPersonController>.Instance.AnimController.SpawnFirstPersonAnimation("FirstPersonScissors", null).transform;
			firstPersonItem.localPosition = new Vector3(0f, -1.25f, 4f) + Vector3.right * 3f;
			firstPersonItem.localEulerAngles = new Vector3(0f, 0f, 0f);
			Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
			CardSlot target = null;
			List<CardSlot> validTargets = this.GetValidTargets();
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield return Singleton<BoardManager>.Instance.ChooseTarget(this.GetAllTargets(), validTargets, delegate (CardSlot slot)
			{
				target = slot;
			}, new Action<CardSlot>(this.OnInvalidTargetSelected), delegate (CardSlot slot)
			{
			}, () => Singleton<ViewManager>.Instance.CurrentView != View.OpponentQueue || !Singleton<TurnManager>.Instance.IsPlayerMainPhase, CursorType.Scissors);
			if (target != null)
			{
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Locked;
				Singleton<InteractionCursor>.Instance.InteractionDisabled = true;
				yield return this.OnValidTargetSelected(target, firstPersonItem.gameObject);
			}

			Object.Destroy(firstPersonItem.gameObject);
			Singleton<UIManager>.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(0f, 0.2f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}

		private IEnumerator OnValidTargetSelected(CardSlot target, GameObject firstPersonItem)
		{
			PlayableCard targetCard = target.Card;
			Tween.LocalPosition(targetCard.transform, new Vector3(0f, 1.25f, -0.5f), 0.1f, 0f, Tween.EaseInOut, Tween.LoopType.None, null, null, true);
			Tween.LocalRotation(targetCard.transform, this.CARD_ROT, 0.1f, 0f, Tween.EaseInOut, Tween.LoopType.None, null, null, true);
			firstPersonItem.GetComponentInChildren<Animator>().SetTrigger("cut");
			yield return new WaitForSeconds(0.65f);
			AudioController.Instance.PlaySound2D("consumable_scissors_use", MixerGroup.TableObjectsSFX, 1f, 0f, null, null, null, null, false);
			GameObject gameObject = Singleton<FirstPersonController>.Instance.AnimController.PlayOneShotAnimation("SplitCard", null);
			gameObject.transform.parent = null;
			gameObject.transform.position = targetCard.transform.position;
			gameObject.transform.eulerAngles = this.CARD_ROT;
			string targetCardName = targetCard.Info.name;
			Object.Destroy(targetCard.gameObject);
			yield return new WaitForSeconds(0.5f);
			if (targetCardName == "Skink")
			{
				yield return base.StartCoroutine(Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("SkinkTail"), target, 0.1f, true));
			}
			Tween.Position(firstPersonItem.transform, firstPersonItem.transform.position + Vector3.back * 4f, 0.2f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
			yield return new WaitForSeconds(0.15f);
			yield break;
		}

		private List<CardSlot> GetValidTargets()
		{
			List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
			opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card == null || x.Card.Info.HasTrait(Trait.Uncuttable));
			return opponentSlotsCopy;
		}

		private List<CardSlot> GetAllTargets()
		{
			return Singleton<BoardManager>.Instance.OpponentSlotsCopy;
		}

		private void OnInvalidTargetSelected(CardSlot targetSlot)
		{
			if (targetSlot.Card != null)
			{
				CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear("You can't cut that... It's too thick.", 2.5f, 0f, Emotion.Laughter, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
			}
		}

		private readonly Vector3 CARD_ROT = new Vector3(90f, 0f, 70f);

	}
}