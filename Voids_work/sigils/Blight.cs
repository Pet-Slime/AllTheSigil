﻿using System.Collections;
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
			const string rulebookDescription = "When [creature] is sacrificed, it subtracts its stat values to the card it was sacrificed for.";
			const string LearnDialogue = "A disease shouldnt spread.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -3);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.void_Blight_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Blight);

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
		}

		public override bool RespondsToSacrifice()
		{
			return true;
		}

		public override IEnumerator OnSacrifice()
		{
			yield return base.PreSuccessfulTriggerSequence();

			if (Singleton<BoardManager>.Instance.currentSacrificeDemandingCard.MaxHealth > base.Card.MaxHealth)
            {
				this.mod.healthAdjustment = base.Card.MaxHealth * -1;
				this.mod.attackAdjustment = base.Card.Attack * -1;
				Singleton<BoardManager>.Instance.currentSacrificeDemandingCard.AddTemporaryMod(this.mod);
				Singleton<BoardManager>.Instance.currentSacrificeDemandingCard.OnStatsChanged();
			} else
			{
				this.mod.healthAdjustment = (Singleton<BoardManager>.Instance.currentSacrificeDemandingCard.MaxHealth -1) * -1;
				this.mod.attackAdjustment = base.Card.Attack * -1;
				Singleton<BoardManager>.Instance.currentSacrificeDemandingCard.AddTemporaryMod(this.mod);
				Singleton<BoardManager>.Instance.currentSacrificeDemandingCard.OnStatsChanged();
			}
			
			yield return new WaitForSeconds(0.25f);
			yield return base.LearnAbility(0.25f);
			yield break;
		}

		private CardModificationInfo mod;

	}
}