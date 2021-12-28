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
		private NewAbility AddPrideful()
		{
			// setup ability
			const string rulebookName = "Prideful";
			const string rulebookDescription = "[creature] will not attack a card if it's strength is much lower than it's own.";
			const string LearnDialogue = "A protector, till the very end.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -1, true);
			info.canStack = false;
			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_bodyguard);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Prideful), tex, abIds);

			// set ability to behaviour class
			void_Prideful.ability = newAbility.ability;

			

			return newAbility;
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
				int cardAttack = __instance.Attack;
				int opposingAttack = opposingSlot.Card.Attack;

				if (cardAttack - 2 > opposingAttack)
                {
					__result = false;

				}
			}
		}
	}
}