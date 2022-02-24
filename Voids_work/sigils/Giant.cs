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
		private NewAbility AddGiant()
		{
			// setup ability
			const string rulebookName = "Giant";
			const string rulebookDescription = "When [creature] is drawn, it will gain one unit blood of cost, as well as one attack and two health.";
			const string LearnDialogue = "What a large creature you have there";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 0);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.no_a2);
			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Giant);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Giant), tex, abIds);

			// set ability to behaviour class
			void_Giant.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Giant : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDrawn()
		{
			return true;
		}

		public override IEnumerator OnDrawn()
		{
			(Singleton<PlayerHand>.Instance as PlayerHand3D).MoveCardAboveHand(base.Card);
			yield return base.Card.FlipInHand(new Action(this.AddMod));
			yield return base.LearnAbility(0.5f);
			yield break;
		}

		private void AddMod()
		{
			CardModificationInfo cardModificationInfo = new CardModificationInfo();
			cardModificationInfo.attackAdjustment = 1;
			cardModificationInfo.healthAdjustment = 2;
			cardModificationInfo.nameReplacement = string.Format(Localization.Translate("Giant {0}"), base.Card.Info.DisplayedNameLocalized);
			cardModificationInfo.bloodCostAdjustment = 1;

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