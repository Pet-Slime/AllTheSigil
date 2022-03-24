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
			const string rulebookDescription = "While [creature] is on the board, it will grant creatures that are played the waterborne sigil. Does not affect cards that are Airborne.";
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

			base.Card.Anim.LightNegationEffect();
			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.25f);
			if (!otherCard.HasAbility(Ability.Submerge) && !otherCard.HasAbility(Ability.SubmergeSquid))
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
			}
			yield return new WaitForSeconds(0.3f);
			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.1f);
			yield break;
		}
	}
}