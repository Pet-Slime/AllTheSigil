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
		private NewAbility AddRegenFull()
		{
			// setup ability
			const string rulebookName = "Regen";
			const string rulebookDescription = "At the end of the owner's turn, [creature] will regen to full health.";
			const string LearnDialogue = "This creature will heal to full Health at the end of it's owner's turn.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 4);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.regeneration_F_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.ability_regen_full);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(void_RegenFull), tex, abIds);

			// set ability to behaviour class
			void_RegenFull.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_RegenFull : AbilityBehaviour
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
			base.Card.HealDamage(base.Card.Status.damageTaken);
			yield return base.LearnAbility(0.25f);
			yield break;
		}
	}
}
