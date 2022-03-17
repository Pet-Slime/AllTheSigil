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
		private void AddPathetic()
		{
			// setup ability
			const string rulebookName = "Pathetic Sacrifice";
			const string rulebookDescription = "[creature] is so pathetic, it is not a worthy or noble sacrifice. [creature] is meant to stay on the board, and thus can't be targeted by the hammer.";
			const string LearnDialogue = "That is not a noble, or worthy sacrifice";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Pathetic);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Pathetic_a2);
			int powerlevel = -3;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Pathetic.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Pathetic), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Pathetic : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

	}

	[HarmonyPatch(typeof(HammerItem), "GetValidTargets", MethodType.Normal)]
	public class PatheticSacrifice_Hammer_Patch
	{
		[HarmonyPrefix]
		public static bool Prefix(ref List<CardSlot> __result)
		{
			if (Plugin.configHammerBlock.Value)
            {
				List<CardSlot> playerSlotsCopy = Singleton<BoardManager>.Instance.PlayerSlotsCopy;
				playerSlotsCopy.RemoveAll((CardSlot x) => x.Card == null);
				playerSlotsCopy.RemoveAll((CardSlot x) => x.Card.HasAbility(void_Pathetic.ability));
				__result = playerSlotsCopy;
				return false;
			}
			return true;
		}
	}



	[HarmonyPatch(typeof(CardInfo), "Sacrificable", MethodType.Getter)]
	public class CardInfo_Sacrificable
	{
		[HarmonyPostfix]
		public static void Postfix(ref CardInfo __instance, ref bool __result)
		{
			if (__instance.abilities.Contains(void_Pathetic.ability) || __instance.ModAbilities.Contains(void_Pathetic.ability))
			{
				__result = false;
			}
		}
	}
}