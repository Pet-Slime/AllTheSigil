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
		private void AddCoward()
		{
			// setup ability
			const string rulebookName = "Cowardly";
			const string rulebookDescription = "[creature] will not attack a card with a power 2 higher than its own.";
			const string LearnDialogue = "It would rather flee than fight";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Coward);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Coward_a2);
			int powerlevel = -1;
			bool LeshyUsable = Plugin.configCowardly.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Coward.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Coward), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Coward : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;
	}

	[HarmonyPatch(typeof(PlayableCard), "AttackIsBlocked", MethodType.Normal)]
	public class AttackIsBlocked_Cowardly_Patch
	{

		[HarmonyPostfix]
		public static void Postfix(ref CardSlot opposingSlot, ref bool __result, PlayableCard __instance)
		{
			if (__instance.OnBoard && opposingSlot.Card != null && __instance.HasAbility(void_Coward.ability))
            {
				int cardAttack = __instance.Attack;
				int opposingAttack = opposingSlot.Card.Attack - 2;

				if (cardAttack < opposingAttack)
                {
					__result = true;

				}
			}
		}
	}
}