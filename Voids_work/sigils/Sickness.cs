using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Turk
		private void AddSickness()
		{
			// setup ability
			const string rulebookName = "Sickness";
			const string rulebookDescription = "[creature] will loose 1 attack each time it declares an attack.";
			const string LearnDialogue = "The creature's strength leaves it as it strikes.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Sickness);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Sickness_a2);
			int powerlevel = -1;
			bool LeshyUsable = Plugin.configSickness.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Sickness.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Sickness), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Sickness : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			return base.Card == attacker;
		}

		public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			if (base.Card == attacker && !attacker.Dead)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return new WaitForSeconds(0.55f);
				base.Card.Anim.LightNegationEffect();
				yield return new WaitForSeconds(0.35f);
				CardModificationInfo cardModificationInfo = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "void_sickness");
				if (cardModificationInfo == null)
				{
					cardModificationInfo = new CardModificationInfo();
					cardModificationInfo.singletonId = "void_sickness";
					base.Card.AddTemporaryMod(cardModificationInfo);
				}
				cardModificationInfo.attackAdjustment--;
				base.Card.OnStatsChanged();
				yield return base.LearnAbility(0f);
			}
			yield break;
		}

	}
}