using DiskCardGame;
using UnityEngine;
using System.Collections;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private void AddBloodGrowth()
		{
			// setup ability
			const string rulebookName = "Blood Growth";
			const string rulebookDescription = "When [creature] attacks, the amount of blood it is counted as when sacrificed will increase.";
			const string LearnDialogue = "There is power in the blood.";
			//const string rulebookNameChinese = "吸血祭品";
			const string rulebookDescriptionChinese = "每次攻击之后，[creature]在献祭时算的血量会增加。";
			const string LearnDialogueChinese = "鲜血即是力量！";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_bloodgrowth);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_bloodgrowth_a2);
			int powerlevel = 0;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;



			// set ability to behaviour class
			if (Localization.CurrentLanguage == Language.ChineseSimplified)
				void_BloodGrowth.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescriptionChinese, typeof(void_BloodGrowth), tex_a1, tex_a2, LearnDialogueChinese,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
			else
				void_BloodGrowth.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_BloodGrowth), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;

		}
	}

	public class void_BloodGrowth : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public int attacks = 0;

		private CardModificationInfo mod;


		public static readonly Ability CustomAbility1 = InscryptionAPI.Guid.GuidManager.GetEnumValue<Ability>("org.memez4life.inscryption.customsigils", "Noble Sacrifice");
		public static readonly Ability CustomAbility2 = InscryptionAPI.Guid.GuidManager.GetEnumValue<Ability>("org.memez4life.inscryption.customsigils", "Superior Sacrifice");


		private void Start()
		{
			this.mod = new CardModificationInfo();
		}

		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			return attacker == base.Card;
		}

		public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
			yield return new WaitForSeconds(0.05f);
			yield return base.PreSuccessfulTriggerSequence();
			base.Card.Status.hiddenAbilities.Add(this.Ability);

			this.attacks += 1;

			switch (this.attacks)
            {
				case 1:
					base.Card.RemoveTemporaryMod(this.mod);
					Ability Ab1 = CustomAbility1;
					this.mod.abilities.Clear();
					this.mod.abilities.Add(Ab1);
					base.Card.AddTemporaryMod(this.mod);
					break;
				case 2:
					base.Card.RemoveTemporaryMod(this.mod);	
					Ability Ab2 = Ability.TripleBlood;
					this.mod.abilities.Clear();
					this.mod.abilities.Add(Ab2);
					base.Card.AddTemporaryMod(this.mod);
					break;
				case 3:
					base.Card.RemoveTemporaryMod(this.mod);
					Ability Ab3 = CustomAbility2;
					this.mod.abilities.Clear();
					this.mod.abilities.Add(Ab3);
					base.Card.AddTemporaryMod(this.mod);
					break;
			}


			yield return new WaitForSeconds(0.05f);
			yield return base.LearnAbility(0f);
			yield break;
		}

	}
}