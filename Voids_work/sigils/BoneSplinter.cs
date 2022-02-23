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
		private NewAbility AddBoneShard()
		{
			// setup ability
			const string rulebookName = "Bone Shard";
			const string rulebookDescription = "[creature] will grant a bone token when hit, if it lives through the attack.";
			const string LearnDialogue = "A splinter of bone.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 0);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.boneShard_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_BoneShard);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_BoneShard), tex, abIds);

			// set ability to behaviour class
			void_BoneShard.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_BoneShard : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToTakeDamage(PlayableCard source)
		{
			return source != null && source.Health > 0;
		}

		public override IEnumerator OnTakeDamage(PlayableCard source)
		{
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
			yield return new WaitForSeconds(0.1f);
			base.Card.Anim.LightNegationEffect();
			yield return base.PreSuccessfulTriggerSequence();
			yield return Singleton<ResourcesManager>.Instance.AddBones(1, base.Card.Slot);
			yield return new WaitForSeconds(0.1f);
			yield return base.LearnAbility(0.1f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}
	}
}