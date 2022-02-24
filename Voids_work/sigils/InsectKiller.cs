using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddInsectKiller()
		{
			// setup ability
			const string rulebookName = "Entomophage";
			const string rulebookDescription = "[creature] will deal 2 additional damage to cards of the insect tribe.";
			const string LearnDialogue = "They are a good source of protine I hear.";
			// const string TextureFile = "Artwork/void_weaken.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 0);
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.no_a2);
			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_insectKiller);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_InsectKiller), tex, abIds);

			// set ability to behaviour class
			void_InsectKiller.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_InsectKiller : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDealDamage(int amount, PlayableCard target)
		{	
			return amount > 0 && target != null && !target.Dead;
		}

		public override IEnumerator OnDealDamage(int amount, PlayableCard target)
		{
			if (target.Info.IsOfTribe(Tribe.Insect))
            {
				yield return base.PreSuccessfulTriggerSequence();
				yield return new WaitForSeconds(0.15f);
				target.Anim.LightNegationEffect();
				yield return new WaitForSeconds(0.15f);
				yield return target.TakeDamage(2, base.Card);
				yield return new WaitForSeconds(0.15f);
				yield return base.LearnAbility(0f);
			}
			
			yield break;
		}

	}
}