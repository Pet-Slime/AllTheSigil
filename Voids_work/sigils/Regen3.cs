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
		private NewAbility AddRegen3()
		{
			// setup ability
			const string rulebookName = "Regen 3";
			const string rulebookDescription = "At the end of the owner's turn, [creature] will regen 3 health.";
			const string LearnDialogue = "This creature will heal 3 Health at the end of it's owner's turn.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.ability_regen_3);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(void_Regen3), tex, abIds);

			// set ability to behaviour class
			void_Regen3.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Regen3 : AbilityBehaviour
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
				base.Card.HealDamage(3);
			}
			yield return base.LearnAbility(0.25f);
			yield break;
		}
	}
}
