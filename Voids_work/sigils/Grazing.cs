using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Port from Cyn Sigil a day
		private void AddGrazing()
		{
			// setup ability
			const string rulebookName = "Grazing";
			const string rulebookDescription = "At the end of the owner's turn, [creature] will regen 1 health if there is no opposing creature.";
			const string LearnDialogue = "This creature will heal 1 Health at the end of it's owner's turn.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Grazing);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = 1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Grazing.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Grazing), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Grazing : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			return base.Card.OpponentCard != playerUpkeep;
		}

		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{
			if (base.Card.slot.opposingSlot.Card == null)
            {
				yield return base.PreSuccessfulTriggerSequence();
				if (base.Card.Status.damageTaken > 0)
				{
					base.Card.HealDamage(1);
				}
				yield return base.LearnAbility(0.25f);
			}
			yield break;
		}
	}
}
