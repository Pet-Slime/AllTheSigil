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
		private void AddPrideful()
		{
			// setup ability
			const string rulebookName = "Prideful";
			const string rulebookDescription = "[creature] will not attack a card with a power 2 lower than its own.";
			const string LearnDialogue = "A creature's pride will be it's downfall.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Prideful);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Prideful_a2);
			int powerlevel = -1;
			bool LeshyUsable = Plugin.configPrideful.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Prideful.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Prideful), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Prideful : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;
	}

	[HarmonyPatch(typeof(PlayableCard), "AttackIsBlocked", MethodType.Normal)]
	public class AttackIsBlocked_Prideful_Patch
	{

		[HarmonyPostfix]
		public static void Postfix(ref CardSlot opposingSlot, ref bool __result, PlayableCard __instance)
		{
			if (__instance.OnBoard && opposingSlot.Card != null && __instance.HasAbility(void_Prideful.ability))
            {
				int cardAttack = __instance.Attack - 2;
				int opposingAttack = opposingSlot.Card.Attack;
				if (cardAttack > opposingAttack)
                {
					__result = true;
				}
			}
		}
	}
}