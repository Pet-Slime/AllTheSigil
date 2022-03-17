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
		private void AddDying()
		{
			// setup ability
			const string rulebookName = "Dying";
			const string rulebookDescription = "[creature] will lose 1 health each time it declares an attack.";
			const string LearnDialogue = "Tik Toc";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Dying);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Dying_a2);
			int powerlevel = 0;
			bool LeshyUsable = Plugin.configDying.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Dying.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Dying), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Dying : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			return attacker.HasAbility(void_Dying.ability);
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
			yield break;
		}
	}
}