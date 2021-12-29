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
		private NewAbility AddCoward()
		{
			// setup ability
			const string rulebookName = "Cowardly";
			const string rulebookDescription = "A card bearing this sigil will not attack a card with a strength 2 higher than its own.";
			const string LearnDialogue = "It would rather flee than fight";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -1, Plugin.configCowardly.Value);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Coward);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Coward), tex, abIds);

			// set ability to behaviour class
			void_Coward.ability = newAbility.ability;

			

			return newAbility;
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