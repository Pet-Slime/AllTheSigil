using HarmonyLib;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddBox()
		{
			// setup ability
			const string rulebookName = "Box";
			const string rulebookDescription = "[creature] will get removed from your deck on death, and a new creature contained within will be added to it.";
			const string LearnDialogue = "What is contained within?";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 1);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.PatheticSacrificeAct2);
			info.metaCategories.Remove(AbilityMetaCategory.Part1Modular);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_crate);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_crate), tex, abIds);

			// set ability to behaviour class
			void_crate.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_crate : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
		{
			return !wasSacrifice;
		}

		public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
		{
			yield return this.BreakCage(true);
			yield break;
		}

		private IEnumerator BreakCage(bool fromBattle)
		{
			string creatureWithinId = "Opossum";
			bool flag = base.Card.Info.iceCubeParams != null && base.Card.Info.iceCubeParams.creatureWithin != null;
			if (flag)
			{
				creatureWithinId = base.Card.Info.iceCubeParams.creatureWithin.name;
			}
			yield return new WaitForSeconds(0.5f);
			if (fromBattle)
			{
				RunState.Run.playerDeck.RemoveCard(base.Card.Info);
				RunState.Run.playerDeck.AddCard(CardLoader.GetCardByName(creatureWithinId));
				yield return new WaitForSeconds(1f);
				yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName(creatureWithinId), base.Card.Slot, 0.15f, true);
			}
			yield return new WaitForSeconds(1f);
			yield break;
		}
	}
}