using HarmonyLib;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddDesperation()
		{
			// setup ability
			const string rulebookName = "Desperation";
			const string rulebookDescription = "[creature] is damaged to 1 health, it will gain 3 power.";
			const string LearnDialogue = "So close to death, it strikes out.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 0);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.no_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_desperation);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_desperation), tex, abIds);

			// set ability to behaviour class
			void_desperation.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_desperation : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private CardModificationInfo mod;

		private void Start()
		{
			this.mod = new CardModificationInfo();
			this.mod.attackAdjustment = 3;
		}

		public override bool RespondsToTakeDamage(PlayableCard source)
		{
			return base.Card.Health == 1;
		}

		public override IEnumerator OnTakeDamage(PlayableCard source)
		{
			yield return base.PreSuccessfulTriggerSequence();
			base.Card.Anim.StrongNegationEffect();
			yield return new WaitForSeconds(0.55f);
			base.Card.temporaryMods.Add(this.mod);
			yield return base.LearnAbility(0.4f);
			yield break;
		}
	}
}