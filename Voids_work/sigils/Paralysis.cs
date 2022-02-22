using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using APIPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddParalise()
		{
			// setup ability
			const string rulebookName = "Paralysis";
			const string rulebookDescription = "A card bearing this sigil will not attack every other turn";
			const string LearnDialogue = "A creature's pride will be it's downfall.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -1, Plugin.configPrideful.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.paralysis_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_paralysis);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Paralysis), tex, abIds);

			// set ability to behaviour class
			void_Paralysis.ability = newAbility.ability;

			return newAbility;
		}
	}


	public class void_Paralysis : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


		public override bool RespondsToTurnEnd(bool playerTurnEnd)
		{
			return base.Card != null && base.Card.OpponentCard != playerTurnEnd;
		}

		public override IEnumerator OnTurnEnd(bool playerTurnEnd)
		{
			yield return base.PreSuccessfulTriggerSequence();
			yield return base.LearnAbility(0f);
			yield break;
		}
	}
}