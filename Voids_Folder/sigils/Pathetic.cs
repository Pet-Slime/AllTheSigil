﻿using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddPathetic()
		{
			// setup ability
			const string rulebookName = "Pathetic Sacrifice";
			const string rulebookDescription = "[creature] is so pathetic, it is not a worthy or noble sacrifice.";
			const string LearnDialogue = "That is not a noble, or worthy sacrifice";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, -2);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_pathetic);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Pathetic), tex, abIds);

			// set ability to behaviour class
			void_Pathetic.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Pathetic : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

	}
}