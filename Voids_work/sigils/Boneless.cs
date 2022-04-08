﻿using UnityEngine;
using DiskCardGame;
using HarmonyLib;
using System.Collections;
using APIPlugin;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private void AddBoneless()
		{
			// setup ability
			const string rulebookName = "Boneless";
			const string rulebookDescription = "[creature] gives no bones! Any bones gained from sigils or death will be negated.";
			const string LearnDialogue = "That creature has no bones!";
			//const string rulebookNameChinese = "无骨者";
			const string rulebookDescriptionChinese = "[creature]不会产生骨头，无论是通过印记还是死后。";
			const string LearnDialogueChinese = "这个造物没有骨头。";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Boneless);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Boneless_a2);
			int powerlevel = -1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			if (Localization.CurrentLanguage == Language.ChineseSimplified)
				void_Boneless.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescriptionChinese, typeof(void_Boneless), tex_a1, tex_a2, LearnDialogueChinese,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
			else
				void_Boneless.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Boneless), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Boneless : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


		[HarmonyPatch(typeof(ResourcesManager), nameof(ResourcesManager.AddBones))]
		public class Boneless_Patch
		{
			[HarmonyPrefix]
			public static bool Prefix(int amount, CardSlot slot)
			{
				if (slot != null
				&& slot.Card != null
				&& slot.Card.HasAbility(void_Boneless.ability))
				{
					return false;
				} else
				{
					return true;
                }
			}
		}
	}
}