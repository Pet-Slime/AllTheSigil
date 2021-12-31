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
		private NewAbility AddToxinVigor()
		{
			// setup ability
			const string rulebookName = "Toxin (Vigor)";
			const string rulebookDescription = "[creature] will inject toxin to what it attacks, that causes the target to wither away in vigor.";
			const string LearnDialogue = "Even once combat is over, vigor leaves it's target";
			// const string TextureFile = "Artwork/void_weaken.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 1, Plugin.configToxin.Value);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_toxin_health);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_ToxinVigor), tex, abIds);

			// set ability to behaviour class
			void_ToxinVigor.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_ToxinVigor : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private CardModificationInfo mod;

		private void Start()
		{
			this.mod = new CardModificationInfo();
			this.mod.healthAdjustment = -1;
		}

		public override bool RespondsToDealDamage(int amount, PlayableCard target)
		{
			if (target.Dead)
            {
				return false;
            }
			return true;
		}

		public override IEnumerator OnDealDamage(int amount, PlayableCard target)
		{
			if (target)
            {
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
				yield return new WaitForSeconds(0.1f);
				base.Card.Anim.LightNegationEffect();
				yield return base.PreSuccessfulTriggerSequence();
				target.temporaryMods.Add(this.mod);
				if (target.Health <= 0)
				{
					yield return target.Die(false, base.Card, true);
				}
				yield return new WaitForSeconds(0.1f);
				yield return base.LearnAbility(0.1f);
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
				
            }
			yield break;
		}

	}
}