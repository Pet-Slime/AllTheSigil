using System.Collections;
using System;
using Pixelplacement;
using UnityEngine;
using DiskCardGame;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private void AddSticky()
		{
			// setup ability
			const string rulebookName = "Sticky";
			const string rulebookDescription = "If the card opposing [creature] moves, [creature] will try to move with it";
			const string LearnDialogue = "The trail they leave behind, hurts.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Sticky);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Sticky_a2);
			int powerlevel = 4;
			bool LeshyUsable = Plugin.configAcidTrail.Value;
			bool part1Shops = true;
			bool canStack = false;



			// set ability to behaviour class
			void_Sticky.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Sticky), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Sticky : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public CardSlot otherslot;

		public PlayableCard othercard;


		public override bool RespondsToResolveOnBoard()
		{
			return true;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			if (base.Card.slot.opposingSlot.Card != null)
			{
				othercard = base.Card.slot.opposingSlot.Card;
				otherslot = base.Card.slot.opposingSlot;
			}
			yield break;
		}

		public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
		{
			return base.Card.OnBoard;
		}

		public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
		{
			if (base.Card.slot.opposingSlot.Card == otherCard)
			{
				othercard = otherCard;
				otherslot = otherCard.slot;
			}
			yield break;
		}

		public override bool RespondsToOtherCardAssignedToSlot(PlayableCard otherCard)
		{
			return base.Card.OnBoard;
		}

		public override IEnumerator OnOtherCardAssignedToSlot(PlayableCard otherCard)
		{
			if (othercard == otherCard)
            {
				if (otherCard.slot.opposingSlot.Card == null)
                {
					Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
					yield return new WaitForSeconds(0.05f);
					yield return base.PreSuccessfulTriggerSequence();
					Vector3 a = base.Card.Slot.IsPlayerSlot ? Vector3.back : Vector3.forward;
					Tween.Position(base.Card.transform, base.Card.transform.position + a * 2f + Vector3.up * 0.25f, 0.15f, 0f, Tween.EaseOut, Tween.LoopType.None, null, null, true);
					yield return new WaitForSeconds(0.15f);
					Tween.Position(base.Card.transform, new Vector3(otherCard.slot.opposingSlot.transform.position.x, base.Card.transform.position.y, base.Card.transform.position.z), 0.1f, 0f, null, Tween.LoopType.None, null, null, true);
					yield return new WaitForSeconds(0.1f);
					yield return Singleton<BoardManager>.Instance.AssignCardToSlot(base.Card, otherCard.slot.opposingSlot, 0.1f, null, true);
					yield return new WaitForSeconds(0.05f);
					yield return base.LearnAbility(0f);
				} else
                {
					othercard = null;
                }
            }
			if (othercard == null)
            {
				othercard = base.Card.slot.opposingSlot.Card;
			}
			yield break;
		}
	}
}