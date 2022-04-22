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
		private void AddStrongWind()
		{
			// setup ability
			const string rulebookName = "Strong Wind";
			const string rulebookDescription = "If [creature] is on the board, negate the airbone sigil of all cards that are played after it.";
			const string LearnDialogue = "The Wind forces a landing.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_StrongWind);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_StrongWind_a2);
			int powerlevel = 1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_StrongWind.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_StrongWind), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_StrongWind : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
		{
			return base.Card.OnBoard && otherCard.slot != base.Card.slot && otherCard.HasAbility(Ability.Flying);
		}

		public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
		{
			base.Card.Anim.LightNegationEffect();
			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.25f);
			CardModificationInfo negateMod = new CardModificationInfo();
			//go through each of the cards default abilities and add them to the modifincation info
			negateMod.negateAbilities.Add(Ability.Flying);
			//Clone the main card info so we don't touch the main card set
			CardInfo OpponentCardInfo = otherCard.Info.Clone() as CardInfo;
			//Add the modifincations
			OpponentCardInfo.Mods.Add(negateMod);
			OpponentCardInfo.Mods.AddRange(otherCard.Info.Mods);
			//Update the opponant card info
			otherCard.Anim.PlayTransformAnimation();
			otherCard.SetInfo(OpponentCardInfo);
			otherCard.Anim.PlayTransformAnimation();
			yield return new WaitForSeconds(0.3f);
			yield return base.LearnAbility(0.25f);
			yield break;
		}
	}
}