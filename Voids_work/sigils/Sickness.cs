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
		private NewAbility AddSickness()
		{
			// setup ability
			const string rulebookName = "Sickness";
			const string rulebookDescription = "[creature] will loose 1 attack each time it declares an attack.";
			const string LearnDialogue = "The creature's strength leaves it as it strikes.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -1, Plugin.configSickness.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.void_sick_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_sick);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_sickness), tex, abIds);

			// set ability to behaviour class
			void_sickness.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_sickness : AbilityBehaviour
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
				cardModificationInfo.attackAdjustment++;
				base.Card.OnStatsChanged();
				yield return base.LearnAbility(0f);
			}
			yield break;
		}

	}
}