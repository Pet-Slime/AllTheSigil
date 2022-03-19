﻿using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using HarmonyLib;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System.Linq;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private void AddPredator()
		{
			// setup ability
			const string rulebookName = "Predator";
			const string rulebookDescription = "[creature] will gain 1 power for each instance of Predator, when the opposing slot has a card.";
			const string LearnDialogue = "It hunts";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Predator);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Predator_a2);
			int powerlevel = 2;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Predator.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Predator), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	[HarmonyPatch(typeof(AbilityIconInteractable), "LoadIcon")]
	public class PrediatorIcon
	{
		[HarmonyPostfix]
		public static void Postfix(ref Texture __result, ref CardInfo info, ref AbilityInfo ability)
		{
			if (ability.ability == void_Predator.ability)
			{
				if (info != null && !SaveManager.SaveFile.IsPart2)
				{
					//Get count of how many instances of the ability the card has
					int count = Mathf.Max(info.Abilities.FindAll((Ability x) => x == void_Predator.ability).Count, 1);
					//Switch statement to the right texture
					switch (count)
					{
						case 1:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Predator_1);
							break;
						case 2:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Predator_2);
							break;
						case 3:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Predator_3);
							break;
						case 4:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Predator_4);
							break;
						case 5:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Predator_5);
							break;
					}
				}
			}
		}
	}



	public class void_Predator : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

	}

	[HarmonyPatch(typeof(PlayableCard), "GetPassiveAttackBuffs")]
	public class void_PredatorPatch
	{
		[HarmonyPostfix]
		public static void Postfix(ref int __result, ref PlayableCard __instance)
		{
			if (__instance.OnBoard)
			{
				if (__instance.slot.opposingSlot.Card != null && __instance.Info.HasAbility(void_Predator.ability))
				{
					int count = SigilUtils.getAbilityCount(__instance, void_Predator.ability);
					__result = count + __result;
				}
			}
		}
	}
}