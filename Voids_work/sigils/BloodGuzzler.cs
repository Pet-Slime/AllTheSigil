using System.Collections;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Port from Cyn Sigil a day
		private void AddBloodGuzzler()
		{
			// setup ability
			const string rulebookName = "BloodGuzzler";
			const string rulebookDescription = "[creature] deals damage, it gains 1 Health for each damage dealt.";
			const string LearnDialogue = "Gain life from the blood.";
			//const string rulebookNameChinese = "鲜血吞噬者";
			const string rulebookDescriptionChinese = "[creature]对其他造物造成伤害时，每造成1点伤害增加1点生命。";
			const string LearnDialogueChinese = "从鲜血中获得生命。";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.ability_bloodguzzler);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.ability_BloodGuzzler_a2);
			int powerlevel = 0;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;



			// set ability to behaviour class
			if (Localization.CurrentLanguage == Language.ChineseSimplified)
				void_BloodGuzzler.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescriptionChinese, typeof(void_BloodGuzzler), tex_a1, tex_a2, LearnDialogueChinese,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
			else
				void_BloodGuzzler.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_BloodGuzzler), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
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