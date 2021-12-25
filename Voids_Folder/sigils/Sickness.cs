using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Turk
		private NewAbility AddSickness()
		{
			// setup ability
			const string rulebookName = "Sickness";
			const string rulebookDescription = "[creature] will loose one attack each time it declares an attack.";
			const string LearnDialogue = "The creature's strength leaves it as it strikes.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -1, true);
			info.canStack = false;

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

		private CardModificationInfo mod;

		private void Start()
		{
			this.mod = new CardModificationInfo();
			this.mod.attackAdjustment = -1;
		}


		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			return attacker.HasAbility(void_sickness.ability);
		}

		public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{

			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.55f);
			attacker.Anim.LightNegationEffect();
			yield return new WaitForSeconds(0.35f);
			attacker.temporaryMods.Add(this.mod);
			Plugin.Log.LogWarning("Sickness debug " + attacker + " has lost it's strength");
			yield return base.LearnAbility(0f);
			yield break;
		}

	}
}