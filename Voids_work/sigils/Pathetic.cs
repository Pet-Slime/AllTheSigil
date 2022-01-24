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
		private NewAbility AddPathetic()
		{
			// setup ability
			const string rulebookName = "Pathetic Sacrifice";
			const string rulebookDescription = "[creature] is so pathetic, it is not a worthy or noble sacrifice. A card with this sigil is meant to stay on the board, and thus can't be targeted by the hammer.";
			const string LearnDialogue = "That is not a noble, or worthy sacrifice";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, -2);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.PatheticSacrificeAct2);

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