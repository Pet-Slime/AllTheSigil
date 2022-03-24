using DiskCardGame;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private void AddSluggish()
		{
			// setup ability
			const string rulebookName = "Sluggish";
			const string rulebookDescription = "[creature] will not attack during normal combat. It will instead attack after the Opponent's cards attack.";
			const string LearnDialogue = "A bit slow there eh?";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Sluggish);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Sluggish_a2);
			int powerlevel = -1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Sluggish.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Sluggish), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Sluggish : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToTurnEnd(bool playerTurnEnd)
		{
			return base.Card != null && base.Card.OpponentCard == playerTurnEnd;
		}

		public override IEnumerator OnTurnEnd(bool playerTurnEnd)
		{
			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.1f);
			var list = new List<CardSlot>();
			list.Add(base.Card.slot);
			yield return FakeCombat.FakeCombatPhase(base.Card.slot.IsPlayerSlot, null, list);
			yield return new WaitForSeconds(0.1f);
			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.1f);
			yield break;
		}
	}
}