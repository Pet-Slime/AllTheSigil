using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Port from Cyn Sigil a day
		private NewAbility AddBloodGuzzler()
		{
			// setup ability
			const string rulebookName = "BloodGuzzler";
			const string rulebookDescription = "[creature] deals damage, it gains 1 Health for each damage dealt.";
			const string LearnDialogue = "Life, thru death";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 4, Plugin.configBloodGuzzler.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.ability_BloodGuzzler_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.ability_bloodguzzler);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_BloodGuzzler), tex, abIds);

			// set ability to behaviour class
			void_BloodGuzzler.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_BloodGuzzler : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private void Start()
		{
			int health = base.Card.Info.Health;
			this.mod = new CardModificationInfo();
			this.mod.nonCopyable = true;
			this.mod.singletonId = "increaseHP";
			this.mod.healthAdjustment = 0;
			base.Card.AddTemporaryMod(this.mod);
		}

		public override bool RespondsToDealDamage(int amount, PlayableCard target)
		{
			return amount > 0;
		}

		public override IEnumerator OnDealDamage(int amount, PlayableCard target)
		{
			yield return base.PreSuccessfulTriggerSequence();
			this.mod.healthAdjustment += amount;
			base.Card.OnStatsChanged();
			base.Card.Anim.StrongNegationEffect();
			yield return new WaitForSeconds(0.25f);
			yield return base.LearnAbility(0.25f);
			yield break;
		}

		private CardModificationInfo mod;

	}
}