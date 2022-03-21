using System;
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
		private void AddDwarf()
		{
			// setup ability
			const string rulebookName = "Dwarf";
			const string rulebookDescription = "When [creature] is drawn, it will loose one unit of cost, as well as 1 power and 2 health (can't go below 1 health). A unit is defined as: 1 blood, 3 bones, 3 energy, or all mox.";
			const string LearnDialogue = "What a tiny creature you have there";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Dwarf);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = 0;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Dwarf.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Dwarf), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Dwarf : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDrawn()
		{
			return true;
		}

		public override IEnumerator OnDrawn()
		{
			yield return base.PreSuccessfulTriggerSequence();
			(Singleton<PlayerHand>.Instance as PlayerHand3D).MoveCardAboveHand(base.Card);
			yield return base.Card.FlipInHand(new Action(this.AddMod));
			yield return base.LearnAbility(0.5f);
			yield break;
		}

		private void AddMod()
		{
			CardModificationInfo cardModificationInfo = new CardModificationInfo();
			cardModificationInfo.attackAdjustment = -1;
			int cardHealth = base.Card.MaxHealth;
			if (cardHealth > 2)
            {
				cardModificationInfo.healthAdjustment = -2;
			} else if (cardHealth == 2)
            {
				cardModificationInfo.healthAdjustment = -1;
			}
			cardModificationInfo.nameReplacement = string.Format(Localization.Translate("Smol {0}"), base.Card.Info.DisplayedNameLocalized);

			if (base.Card.Info.cost > 0)
            {
				cardModificationInfo.bloodCostAdjustment = -1;
			} else if (base.Card.Info.bonesCost > 0)
            {
				if (base.Card.Info.bonesCost >= 3)
				{
					cardModificationInfo.bonesCostAdjustment = -3;
				}
				else if (base.Card.Info.bonesCost == 2)
				{
					cardModificationInfo.bonesCostAdjustment = -2;
				}
				else
				{
					cardModificationInfo.bonesCostAdjustment = -1;
				}
			} else if (base.Card.Info.energyCost > 0)
            {
				if (base.Card.Info.energyCost >= 3)
                {
					cardModificationInfo.energyCostAdjustment = -3;
				} else if (base.Card.Info.energyCost == 2)
                {
					cardModificationInfo.energyCostAdjustment = -2;
				} else
                {
					cardModificationInfo.energyCostAdjustment = -1;

				}
            } else if (base.Card.Info.GemsCost.Count > 0)
            {
				cardModificationInfo.nullifyGemsCost = true;
			}
			CardInfo ClonedCardInfo = base.Card.Info.Clone() as CardInfo;

			//Add the modifincations
			ClonedCardInfo.Mods.Add(cardModificationInfo);

			for (int index = 0; index < base.Card.Info.Mods.Count; index++)
            {
				ClonedCardInfo.Mods.Add(base.Card.Info.Mods[index]);
			}
			

			//Update the card info
			base.Card.SetInfo(ClonedCardInfo);
		}
	}
}