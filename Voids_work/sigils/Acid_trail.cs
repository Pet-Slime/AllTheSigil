using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private NewAbility AddAcidTrail()
		{
			// setup ability
			const string rulebookName = "Acidic Trail";
			const string rulebookDescription = "[creature] will damage the card in the opposing slot, if it is able to strafe in the direction marked.";
			const string LearnDialogue = "The trail they leave behind, hurts.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 2, Plugin.configAcidTrail.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.acidtrail_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_acid);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_AcidTrail), tex, abIds);

			// set ability to behaviour class
			void_AcidTrail.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_AcidTrail : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		//Copied Strafe's code and just added damage

		public override bool RespondsToTurnEnd(bool playerTurnEnd)
		{
			return base.Card != null && base.Card.OpponentCard != playerTurnEnd;
		}

		public override IEnumerator OnTurnEnd(bool playerTurnEnd)
		{
			CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, true);
			CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, false);
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
			yield return new WaitForSeconds(0.25f);
			yield return base.StartCoroutine(this.DoStrafe(toLeft, toRight));
			yield break;
		}

		protected virtual IEnumerator DoStrafe(CardSlot toLeft, CardSlot toRight)
		{
			bool flag = toLeft != null && toLeft.Card == null;
			bool flag2 = toRight != null && toRight.Card == null;
			if (this.movingLeft && !flag)
			{
				this.movingLeft = false;
			}
			if (!this.movingLeft && !flag2)
			{
				this.movingLeft = true;
			}
			CardSlot destination = this.movingLeft ? toLeft : toRight;
			bool destinationValid = this.movingLeft ? flag : flag2;
			yield return base.StartCoroutine(this.MoveToSlot(destination, destinationValid));
			if (destination != null && destinationValid)
			yield break;
		}

		protected IEnumerator MoveToSlot(CardSlot destination, bool destinationValid)
		{
			base.Card.RenderInfo.SetAbilityFlipped(this.Ability, this.movingLeft);
			base.Card.RenderInfo.flippedPortrait = (this.movingLeft && base.Card.Info.flipPortraitForStrafe);
			base.Card.RenderCard();
			if (destination != null && destinationValid)
			{
				CardSlot oldSlot = base.Card.Slot;
				yield return Singleton<BoardManager>.Instance.AssignCardToSlot(base.Card, destination, 0.1f, null, true);
				yield return this.PostSuccessfulMoveSequence(oldSlot);
				yield return new WaitForSeconds(0.25f);
				oldSlot = null;
			}
			else
			{
				base.Card.Anim.StrongNegationEffect();
				yield return new WaitForSeconds(0.15f);
			}
			yield break;
		}

		protected virtual IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
		{
			if (oldSlot.opposingSlot.Card != null)
			{
				if (base.Card.Anim is CardAnimationController)
				{
					(base.Card.Anim as CardAnimationController).PlayAttackAnimation(false, oldSlot);

				}
				yield return base.Card.slot.opposingSlot.Card.TakeDamage(1, base.Card);
				yield return new WaitForSeconds(0.5f);
			}
			yield break;
		}

		protected bool movingLeft;

	}
}