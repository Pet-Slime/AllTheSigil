using HarmonyLib;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddManeuver()
		{
			// setup ability
			const string rulebookName = "Maneuver";
			const string rulebookDescription = "At the start of the owner's turn, [creature] will strafe in the direction inscribed on the sigil if there is a creature in the opposing slot from it. Else it will strafe in the opposite direction inscribed on the sigil.";
			const string LearnDialogue = "That is not a noble, or worthy sacrifice";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 2);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.void_Maneuver_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Maneuver);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Maneuver), tex, abIds);

			// set ability to behaviour class
			void_Maneuver.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Maneuver : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			return base.Card != null && base.Card.OpponentCard != playerUpkeep;
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x00057180 File Offset: 0x00055380
		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{
			CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, true);
			CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, false);
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
			yield return new WaitForSeconds(0.25f);
			if (base.Card.slot.opposingSlot.Card != null)
			{
				yield return this.DoStrafe(toLeft, toRight);
			} else
            {
				yield return this.DoStrafe(toRight, toLeft);
			}
			yield break;
		}

		// Token: 0x06001578 RID: 5496 RVA: 0x00057196 File Offset: 0x00055396
		protected virtual IEnumerator DoStrafe(CardSlot toLeft, CardSlot toRight)
		{
			bool toLeftValid = toLeft != null && toLeft.Card == null;
			bool toRightValid = toRight != null && toRight.Card == null;
			bool flag = this.movingLeft && !toLeftValid;
			if (flag)
			{
				this.movingLeft = false;
			}
			bool flag2 = !this.movingLeft && !toRightValid;
			if (flag2)
			{
				this.movingLeft = true;
			}
			CardSlot destination = this.movingLeft ? toLeft : toRight;
			bool destinationValid = this.movingLeft ? toLeftValid : toRightValid;
			yield return this.MoveToSlot(destination, destinationValid);
			bool flag3 = destination != null && destinationValid;
			if (flag3)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return base.LearnAbility(0f);
			}
			yield break;
		}

		// Token: 0x06001579 RID: 5497 RVA: 0x000571B3 File Offset: 0x000553B3
		protected IEnumerator MoveToSlot(CardSlot destination, bool destinationValid)
		{
			base.Card.RenderInfo.SetAbilityFlipped(this.Ability, this.movingLeft);
			base.Card.RenderInfo.flippedPortrait = (this.movingLeft && base.Card.Info.flipPortraitForStrafe);
			base.Card.RenderCard();
			bool flag = destination != null && destinationValid;
			if (flag)
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

		// Token: 0x0600157A RID: 5498 RVA: 0x000571D0 File Offset: 0x000553D0
		protected virtual IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
		{
			bool flag = base.Card.Info.name == "Snelk";
			if (flag)
			{
				bool flag2 = oldSlot.Card == null;
				if (flag2)
				{
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("Snelk_Neck"), oldSlot, 0.1f, true);
				}
			}
			yield break;
		}

		// Token: 0x04000F1F RID: 3871
		protected bool movingLeft;
	}
}