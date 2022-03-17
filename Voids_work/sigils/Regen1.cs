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
		private void AddRegen1()
		{
			// setup ability
			const string rulebookName = "Regen 1";
			const string rulebookDescription = "At the end of the owner's turn, [creature] will regen 1 health.";
			const string LearnDialogue = "This creature will heal 1 Health at the end of it's owner's turn.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.ability_regen_1);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.ability_regen_1_a2);
			int powerlevel = 1;
			bool LeshyUsable = Plugin.configRegen.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Regen1.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Regen1), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Regen1 : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			return base.Card.OpponentCard != playerUpkeep;
		}

		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{
			yield return base.PreSuccessfulTriggerSequence();
			if (base.Card.Status.damageTaken > 0)
			{
				base.Card.HealDamage(1);
			}
			yield return base.LearnAbility(0.25f);
			yield break;
		}
	}
}
