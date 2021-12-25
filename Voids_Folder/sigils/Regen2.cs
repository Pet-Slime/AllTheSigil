using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Port from Cyn Sigil a day
		private NewAbility AddRegen2()
		{
			// setup ability
			const string rulebookName = "Regen 2";
			const string rulebookDescription = "At the end of the owner's turn, [creature] will regen 2 health.";
			const string LearnDialogue = "This creature will heal 2 Health at the end of it's owner's turn.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 2);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.ability_regen_2);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(void_Regen2), tex, abIds);

			// set ability to behaviour class
			void_Regen2.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Regen2 : AbilityBehaviour
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
				base.Card.HealDamage(2);
			}
			yield return base.LearnAbility(0.25f);
			yield break;
		}
	}
}
