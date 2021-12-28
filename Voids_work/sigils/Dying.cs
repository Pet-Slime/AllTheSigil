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
		private NewAbility AddDying()
		{
			// setup ability
			const string rulebookName = "Dying";
			const string rulebookDescription = "[creature] has limited time left. It will lose one health each time it declares an attack.";
			const string LearnDialogue = "Tik Toc";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -1, true);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_dying);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_dying), tex, abIds);

			// set ability to behaviour class
			void_dying.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_dying : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private CardSlot lastSlot;

		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			return attacker.HasAbility(void_dying.ability) && slot != lastSlot;
		}

		public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{

			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.55f);
			attacker.Anim.LightNegationEffect();
			yield return new WaitForSeconds(0.35f);
			yield return attacker.TakeDamage(1, null);
			Plugin.Log.LogWarning("Dying debug " + attacker + " has taken damage");
			yield return base.LearnAbility(0f);
			lastSlot = slot;
			yield break;
		}

	}
}