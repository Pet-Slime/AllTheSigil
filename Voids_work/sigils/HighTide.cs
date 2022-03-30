using UnityEngine;
using DiskCardGame;
using System.Collections.Generic;
using APIPlugin;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System.Collections;
using HarmonyLib;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private void AddHighTide()
		{
			// setup ability
			const string rulebookName = "High Tide";
			const string rulebookDescription = "While [creature] is on the board, it will grant creatures that are on the same side of the board the waterborn sigil. Does not affect cards that are Airborne.";
			const string LearnDialogue = "The waters rise.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_HighTide);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_HighTide_a2);
			int powerlevel = 1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_HighTide.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_HighTide), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_HighTide : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
		{
			return base.Card.OnBoard && otherCard.slot != base.Card.slot && !otherCard.HasAbility(Ability.Flying);
		}

		public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
		{

			/// I hate how I coded this but I couldn't figure out (might be cause I made this at 5am) how to make sure the base card is on the players side
			/// and the card that is resolving on board is also on the player's side. So I just got all slots based on if the base.card.slot is a player slot or not
			/// then ran a for loop to check if the other card is in a slot on that side. 

			base.Card.Anim.LightNegationEffect();
			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.25f);
			List<CardSlot> cardSlots = Singleton<BoardManager>.Instance.GetSlots(base.Card.slot.IsPlayerSlot);
			for (var index = 0; index < cardSlots.Count; index++)
            {
				if (cardSlots[index].Card == otherCard && !otherCard.HasAbility(Ability.Submerge) && !otherCard.HasAbility(Ability.SubmergeSquid))
                {
					//make the card mondification info
					CardModificationInfo cardModificationInfo = new CardModificationInfo(Ability.Submerge);
					//Clone the main card info so we don't touch the main card set
					CardInfo targetCardInfo = otherCard.Info.Clone() as CardInfo;
					//Add the modifincations to the cloned info
					targetCardInfo.Mods.Add(cardModificationInfo);
					//Set the target's info to the clone'd info
					otherCard.SetInfo(targetCardInfo);
					otherCard.Anim.PlayTransformAnimation();
					yield return new WaitForSeconds(0.3f);
					yield return base.LearnAbility(0.25f);
					yield break;
				}
            }
			yield break;
		}
	}
}