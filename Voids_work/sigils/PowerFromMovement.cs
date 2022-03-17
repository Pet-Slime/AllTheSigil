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
		private void AddMovingPowerUp()
		{
			// setup ability
			const string rulebookName = "Power from Movement";
			const string rulebookDescription = "At the start of the owner's turn, [creature] will gain 1 power and 1 health if it moved last round.";
			const string LearnDialogue = "Each move, it grows";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_MovementPowerUp);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = 1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_MovingPowerUp.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_MovingPowerUp), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
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