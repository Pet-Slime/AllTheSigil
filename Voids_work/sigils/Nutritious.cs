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
		private NewAbility AddNutritious()
		{
			// setup ability
			const string rulebookName = "Nutritious";
			const string rulebookDescription = "A creature gain 1 power and 2 health when summoned using [creature] as a sacrifice.";
			const string LearnDialogue = "That creature is so full of nutrients, the creature you play comes in stronger!";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 2);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.ability_nutritious);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(void_Nutritious), tex, abIds);

			// set ability to behaviour class
			void_Nutritious.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Nutritious : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


		private void Start()
		{
			this.mod = new CardModificationInfo();
			this.mod.healthAdjustment = 2;
			this.mod.attackAdjustment = 1;
		}

		public override bool RespondsToSacrifice()
		{
			return true;
		}

		public override IEnumerator OnSacrifice()
		{
			yield return base.PreSuccessfulTriggerSequence();
			Singleton<BoardManager>.Instance.currentSacrificeDemandingCard.AddTemporaryMod(this.mod);
			Singleton<BoardManager>.Instance.currentSacrificeDemandingCard.OnStatsChanged();
			yield return new WaitForSeconds(0.25f);
			yield return base.LearnAbility(0.25f);
			yield break;
		}

		private CardModificationInfo mod;
	}

}
