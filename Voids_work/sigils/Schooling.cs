using UnityEngine;
using DiskCardGame;
using System.Collections.Generic;
using APIPlugin;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System.Collections;
using HarmonyLib;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private void AddSchooling()
		{
			// setup ability
			const string rulebookName = "Schooling";
			const string rulebookDescription = "[creature] will grant creatures with the waterborn sigil to gain 1 power";
			const string LearnDialogue = "The waterborn stick together.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Schooling);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Schooling_a2);
			int powerlevel = 2;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Schooling.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Schooling), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Schooling : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

	}
}