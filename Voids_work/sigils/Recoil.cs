using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using HarmonyLib;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddRecoil()
		{
			// setup ability
			const string rulebookName = "Recoil";
			const string rulebookDescription = "[creature] will take one damage each time they attack";
			const string LearnDialogue = "The strength causes the creature pain.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -1, Plugin.configDying.Value);
			info.canStack = false;
			info.flipYIfOpponent = true;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.recoil_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Recoil);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Recoil), tex, abIds);

			// set ability to behaviour class
			void_Recoil.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Recoil : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToAttackEnded()
		{
			return base.Card.HasAbility(void_dying.ability);
		}

		public override IEnumerator OnAttackEnded()
		{
			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.55f);
			base.Card.Anim.LightNegationEffect();
			yield return new WaitForSeconds(0.35f);
			yield return base.Card.TakeDamage(1, null);
			yield return base.LearnAbility(0f);
			yield break;
		}
	}
}