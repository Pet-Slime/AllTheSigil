using System.Collections;
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
		private NewAbility AddPredator()
		{
			// setup ability
			const string rulebookName = "Predator";
			const string rulebookDescription = "[creature] will gain one strength for each instance of Predator, when the opposing slot has a card.";
			const string LearnDialogue = "It hunts";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3);
			info.canStack = true;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Predator);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(void_Predator), tex, abIds);

			// set ability to behaviour class
			void_Predator.ability = newAbility.ability;

			return newAbility;
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
					List<Ability> baseAbilities = __instance.Info.Abilities;
					List<Ability> modAbilities = __instance.Info.ModAbilities;
					int finalBuff = baseAbilities.Where(a => a == void_Predator.ability).Count() + modAbilities.Where(a => a == void_Predator.ability).Count() + __result;
					__result = finalBuff;
				}
			}
		}
	}
}