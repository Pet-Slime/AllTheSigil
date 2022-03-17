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
		private void AddNutritious()
		{
			// setup ability
			const string rulebookName = "Nutritious";
			const string rulebookDescription = "When [creature] is sacrificed, it adds 1 power and 2 health to the card it was sacrificed for.";
			const string LearnDialogue = "That creature is so full of nutrients, the creature you play comes in stronger!";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Nutritious);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = 2;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Nutritious.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Nutritious), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
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
