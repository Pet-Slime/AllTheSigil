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
		private void AddDesperation()
		{
			// setup ability
			const string rulebookName = "Desperation";
			const string rulebookDescription = "[creature] is damaged to 1 health, it will gain 3 power.";
			const string LearnDialogue = "So close to death, it strikes out.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Desperation);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = 0;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = true;

			// set ability to behaviour class
			void_Desperation.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Desperation), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Desperation : AbilityBehaviour
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