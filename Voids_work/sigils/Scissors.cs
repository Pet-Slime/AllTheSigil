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
		private void AddScissors()
		{
			// setup ability
			const string rulebookName = "Scissors";
			const string rulebookDescription = "When [creature] is played, a targeted card cut in two.";
			const string LearnDialogue = "My card!";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Scissors);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Scissors_a2);
			int powerlevel = 5;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Scissors.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Scissors), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Scissors : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToResolveOnBoard()
		{
			return true;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.5f);	
			if (base.Card.slot.IsPlayerSlot)
			{
				if (GetPlayerValidTargets().Count == 0)
				{
					Card.Anim.StrongNegationEffect();
					yield return new WaitForSeconds(0.3f);
					yield break;
				}
				yield return ActivateSequence();
			} else
            {
				if (GetLeshyValidTargets().Count == 0)
				{
					Card.Anim.StrongNegationEffect();
					yield return new WaitForSeconds(0.3f);
					yield break;
				}
				yield return EnemyActivateSequence();
			}
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
///			Transform firstPersonItem = Singleton<FirstPersonController>.Instance.AnimController.SpawnFirstPersonAnimation("FirstPersonScissors", null).transform;
///			firstPersonItem.localPosition = new Vector3(0f, -1.25f, 4f) + Vector3.right * 3f;
///			firstPersonItem.localEulerAngles = new Vector3(0f, 0f, 0f);
			Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
			CardSlot target = null;
			List<CardSlot> validTargets = this.GetPlayerValidTargets();
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield return Singleton<BoardManager>.Instance.ChooseTarget(this.GetAllTargets(), validTargets, delegate (CardSlot slot)
			{
				target = slot;
			}, new Action<CardSlot>(this.OnInvalidTargetSelected), delegate (CardSlot slot)
			{
			}, () => Singleton<ViewManager>.Instance.CurrentView != View.OpponentQueue, CursorType.Scissors);
			if (target != null)
			{
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Locked;
				Singleton<InteractionCursor>.Instance.InteractionDisabled = true;
				yield return this.OnValidTargetSelected(target);
			}

///			Object.Destroy(firstPersonItem.gameObject);
			Singleton<UIManager>.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(0f, 0.2f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}

		private IEnumerator EnemyActivateSequence()
		{
			yield return new WaitForSeconds(0.1f);
			Singleton<UIManager>.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(0.6f, 0.2f);
			Singleton<ViewManager>.Instance.SwitchToView(View.BoardCentered, false, true);
			yield return new WaitForSeconds(0.25f);
			Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
			CardSlot target = null;
			List<CardSlot> validTargets = this.GetPlayerValidTargets();
			target = validTargets[SeededRandom.Range(0, validTargets.Count, base.GetRandomSeed())];
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Locked;
			Singleton<InteractionCursor>.Instance.InteractionDisabled = true;
			yield return this.OnValidTargetSelected(target);
			///			Object.Destroy(firstPersonItem.gameObject);
			Singleton<UIManager>.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(0f, 0.2f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}

		private IEnumerator OnValidTargetSelected(CardSlot target)
		{
			PlayableCard targetCard = target.Card;
			Tween.LocalPosition(targetCard.transform, new Vector3(0f, 1.25f, -0.5f), 0.1f, 0f, Tween.EaseInOut, Tween.LoopType.None, null, null, true);
			Tween.LocalRotation(targetCard.transform, this.CARD_ROT, 0.1f, 0f, Tween.EaseInOut, Tween.LoopType.None, null, null, true);
///			firstPersonItem.GetComponentInChildren<Animator>().SetTrigger("cut");
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
///			Tween.Position(firstPersonItem.transform, firstPersonItem.transform.position + Vector3.back * 4f, 0.2f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
			yield return new WaitForSeconds(0.15f);
			yield break;
		}

		private List<CardSlot> GetPlayerValidTargets()
		{
			List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
			opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card == null || x.Card.Info.HasTrait(Trait.Uncuttable));
			return opponentSlotsCopy;
		}

		private List<CardSlot> GetLeshyValidTargets()
		{
			List<CardSlot> playerSlotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
			playerSlotsCopy.RemoveAll((CardSlot x) => x.Card == null || x.Card.Info.HasTrait(Trait.Uncuttable));
			return playerSlotsCopy;
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