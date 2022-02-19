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
		//Request by Tilted hat
		private NewAbility AddCaustic()
		{
			// setup ability
			const string rulebookName = "Caustic";
			const string rulebookDescription = "[creature] will leave an acid puddle behind when it strafes.";
			const string LearnDialogue = "What it leaves behind is deadly.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 2);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.PatheticSacrificeAct2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_caustic);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Caustic), tex, abIds);

			// set ability to behaviour class
			void_Caustic.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Caustic : Strafe
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override IEnumerator PostSuccessfulMoveSequence(CardSlot cardSlot)
		{
			bool flag = cardSlot.Card == null;
			if (flag)
			{
				yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("void_Acid_Puddle"), cardSlot, 0.1f, true);
			}
			yield break;
		}
	}
}