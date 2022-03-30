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
		private void AddFishHook()
		{
			// setup ability
			const string rulebookName = "Fish Hook";
			const string rulebookDescription = "When [creature], a targeted card is moved to the owner's side of the board.";
			const string LearnDialogue = "Go Fish";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_FishHook);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_FishHook_a2);
			int powerlevel = 0;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_FishHook.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_FishHook), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_FishHook : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToResolveOnBoard()
		{
			return true;
		}

		public override IEnumerator OnResolveOnBoard()
		{
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
				yield return LeshyActivateSequence();

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
			Transform firstPersonItem = Singleton<FirstPersonController>.Instance.AnimController.SpawnFirstPersonAnimation("FirstPersonFishHook", null).transform;
			firstPersonItem.localPosition = new Vector3(0f, -1.25f, 4f) + Vector3.right * 3f;
			firstPersonItem.localEulerAngles = new Vector3(0f, 0f, 0f);
			Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
			CardSlot target = null;
			List<CardSlot> validTargets = this.GetPlayerValidTargets();
			this.MoveItemToPosition(firstPersonItem, validTargets[validTargets.Count - 1].transform.position);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield return Singleton<BoardManager>.Instance.ChooseTarget(this.GetAllTargets(), validTargets, delegate (CardSlot slot)
			{
				target = slot;
			}, new Action<CardSlot>(this.OnInvalidTargetSelected), delegate (CardSlot slot)
			{
				this.MoveItemToPosition(firstPersonItem, slot.transform.position);
			}, () => Singleton<ViewManager>.Instance.CurrentView != View.OpponentQueue || !Singleton<TurnManager>.Instance.IsPlayerMainPhase, CursorType.FishHook);
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

		private IEnumerator LeshyActivateSequence()
		{
			yield return new WaitForSeconds(0.1f);
			Singleton<UIManager>.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(0.6f, 0.2f);
			Singleton<ViewManager>.Instance.SwitchToView(View.BoardCentered, false, false);
			yield return new WaitForSeconds(0.25f);
			Singleton<InteractionCursor>.Instance.InteractionDisabled = false;
			CardSlot target = null;
			List<CardSlot> validTargets = this.GetLeshyValidTargets();
			target = validTargets[SeededRandom.Range(0, validTargets.Count, base.GetRandomSeed())];
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Locked;
			Singleton<InteractionCursor>.Instance.InteractionDisabled = true;
			Singleton<UIManager>.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(0f, 0.2f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}

		private void MoveItemToPosition(Transform item, Vector3 targetPos)
		{
			Tween.Position(item, new Vector3(targetPos.x, item.position.y, item.position.z), 0.2f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
		}

		private IEnumerator OnLeshyValidTargetSelected(CardSlot target, GameObject firstPersonItem)
		{
			Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
			yield return new WaitForSeconds(0.2f);
			AudioController.Instance.PlaySound3D("angler_use_hook", MixerGroup.TableObjectsSFX, target.transform.position, 1f, 0.1f, null, null, null, null, false);
			yield return new WaitForSeconds(0.75f);
			if (target.opposingSlot.Card != null)
			{
				yield return Singleton<TurnManager>.Instance.Opponent.ReturnCardToQueue(target.opposingSlot.Card, 0.25f);
			}
			if (target.Card.Status != null)
			{
				target.Card.Status.anglerHooked = true;
			}
			target.Card.SetIsOpponentCard(true);
			target.Card.transform.eulerAngles += new Vector3(0f, 0f, -180f);
			CardSlot cardSlot = target;
			yield return Singleton<BoardManager>.Instance.AssignCardToSlot(cardSlot.Card, cardSlot.opposingSlot, 0.25f, null, true);
			yield return new WaitForSeconds(0.25f);
			yield return new WaitForSeconds(0.4f);
			yield break;
		}


		private IEnumerator OnValidTargetSelected(CardSlot target, GameObject firstPersonItem)
		{
			AudioController.Instance.PlaySound3D("angler_use_hook", MixerGroup.TableObjectsSFX, target.transform.position, 1f, 0.1f, null, null, null, null, false);
			firstPersonItem.GetComponentInChildren<Animator>().SetTrigger("hook");
			yield return new WaitForSeconds(0.51f);
			PlayableCard targetCard = target.Card;
			targetCard.SetIsOpponentCard(false);
			targetCard.transform.eulerAngles += new Vector3(0f, 0f, -180f);
			yield return Singleton<BoardManager>.Instance.AssignCardToSlot(targetCard, target.opposingSlot, 0.33f, null, true);
			if (targetCard.FaceDown)
			{
				targetCard.SetFaceDown(false, false);
				targetCard.UpdateFaceUpOnBoardEffects();
			}
			yield return new WaitForSeconds(0.66f);
			Tween.Position(firstPersonItem.transform, firstPersonItem.transform.position + Vector3.back * 4f, 0.2f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
			yield return new WaitForSeconds(0.15f);
			yield break;
		}

		private List<CardSlot> GetPlayerValidTargets()
		{
			List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
			opponentSlotsCopy.RemoveAll((CardSlot x) => x.Card == null || x.opposingSlot.Card != null || x.Card.Info.HasTrait(Trait.Uncuttable));
			return opponentSlotsCopy;
		}

		private List<CardSlot> GetLeshyValidTargets()
		{
			List<CardSlot> playerSlotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
			playerSlotsCopy.RemoveAll((CardSlot x) => x.Card == null || x.opposingSlot.Card != null || x.Card.Info.HasTrait(Trait.Uncuttable));
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
				if (targetSlot.Card.Info.HasTrait(Trait.Uncuttable))
				{
					CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear("You can't hook that one.", 2.5f, 0f, Emotion.Anger, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
					return;
				}
				if (targetSlot.opposingSlot.Card != null)
				{
					CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear("There's no space to pull that one into.", 3f, 0f, Emotion.Neutral, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
				}
			}
		}
	}
}