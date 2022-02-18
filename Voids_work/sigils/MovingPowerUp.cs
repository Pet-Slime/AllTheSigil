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
		//Request by Sire
		private NewAbility AddMovingPowerUp()
		{
			// setup ability
			const string rulebookName = "Power from Movement";
			const string rulebookDescription = "[creature] will gain health and power each upkeep if it moved the previous round";
			const string LearnDialogue = "Each move, it grows";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 1);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.dying_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_MovementPowerUp);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_MovingPowerUp), tex, abIds);

			// set ability to behaviour class
			void_MovingPowerUp.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_MovingPowerUp : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public static CardSlot lastSlot = null;

		public override bool RespondsToResolveOnBoard()
		{
			return true;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			lastSlot = base.Card.Slot;
			yield break;
		}

		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			return base.Card.OpponentCard != playerUpkeep;
		}

		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{
			if (lastSlot == base.Card.Slot)
            {
				yield break;
			}
			CardModificationInfo cardModificationInfo = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "void_MovingPowerUp");
			if (cardModificationInfo == null)
			{
				cardModificationInfo = new CardModificationInfo();
				cardModificationInfo.singletonId = "void_MovingPowerUp";
				base.Card.AddTemporaryMod(cardModificationInfo);
			}
			cardModificationInfo.attackAdjustment++;
			cardModificationInfo.healthAdjustment++;
			base.Card.OnStatsChanged();
			yield return new WaitForSeconds(0.25f);
			lastSlot = base.Card.Slot;
			yield break;
		}
	}
}