using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using System;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddVicious()
		{
			// setup ability
			const string rulebookName = "Vicious";
			const string rulebookDescription = "A vicious creature's strength grows the more it is attacked.";
			const string LearnDialogue = "A hit just makes it angry.";
			// const string TextureFile = "Artwork/void_vicious.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 1, true);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_vicious);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Vicious), tex, abIds);

			// set ability to behaviour class
			void_Vicious.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Vicious : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private CardModificationInfo mod;

		private void Start()
		{
			this.mod = new CardModificationInfo();
			this.mod.attackAdjustment = 1;
		}



		public override bool RespondsToTakeDamage(PlayableCard source)
		{
			return base.Card.OnBoard;
		}

		public override IEnumerator OnTakeDamage(PlayableCard source)
		{
			if (source)
			{
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
				yield return new WaitForSeconds(0.1f);
				base.Card.Anim.LightNegationEffect();
				yield return base.PreSuccessfulTriggerSequence();
				base.Card.AddTemporaryMod(this.mod);
				yield return new WaitForSeconds(0.1f);
				yield return base.LearnAbility(0.1f);
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;

				
			}
			yield break;
		}

	}
}