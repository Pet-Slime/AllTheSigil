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
		private NewAbility AddBlight()
		{
			// setup ability
			const string rulebookName = "Blight";
			const string rulebookDescription = "[creature] is a diseased card that lowers the strength and vigor of those it is sacrificed to.";
			const string LearnDialogue = "A disease shouldnt spread.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -1);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_blight);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Blight), tex, abIds);

			// set ability to behaviour class
			void_Blight.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Blight : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private void Start()
		{
			this.mod = new CardModificationInfo();
			this.mod.healthAdjustment = -1;
			this.mod.attackAdjustment = -1;
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