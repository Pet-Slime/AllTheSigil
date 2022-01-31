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
		private NewAbility AddGrazing()
		{
			// setup ability
			const string rulebookName = "Grazing";
			const string rulebookDescription = "At the end of the owner's turn, [creature] will regen 1 health if there is no opposing creature.";
			const string LearnDialogue = "This creature will heal 1 Health at the end of it's owner's turn.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 0);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_grazing);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(void_grazing), tex, abIds);

			// set ability to behaviour class
			void_grazing.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_grazing : AbilityBehaviour
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
