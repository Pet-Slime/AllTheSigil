using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Sire
		private void AddLeadEnergy()
		{
			// setup ability
			const string rulebookName = "Lead (Energy)";
			const string rulebookDescription = "Pay 2 bones to move the creature in the direction inscribed on the sigil.";
			const string LearnDialogue = "You can lead  a horse to water...";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_lead_energy);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = 1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;


			var test = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Lead_Energy), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack);
			test.activated = true;
			test.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.void_lead_energy_a2);



			// set ability to behaviour class
			void_Lead_Energy.ability = test.ability;
		}
	}

	public class void_Lead_Energy : ActivatedAbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override int EnergyCost
		{
			get
			{
				return 2;
			}
		}

		public override IEnumerator Activate()
		{
			CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, true);
			CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, false);
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
			yield return new WaitForSeconds(0.25f);
			yield return this.DoStrafe(toLeft, toRight);
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
			yield return this.MoveToSlot(destination, destinationValid);
			if (destination != null && destinationValid)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return base.LearnAbility(0f);
			}
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
			if (base.Card.Info.name == "Snelk" && oldSlot.Card == null)
			{
				yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("Snelk_Neck"), oldSlot, 0.1f, true);
			}
			yield break;
		}

		protected bool movingLeft;
	}
}
