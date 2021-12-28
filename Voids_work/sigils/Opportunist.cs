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
		private NewAbility AddOpportunist()
		{
			// setup ability
			const string rulebookName = "Opportunist";
			const string rulebookDescription = "[creature] will gain one strength for each instance of Opportunist, when the opposing slot is empty.";
			const string LearnDialogue = "It takes it's chance when it gets it.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3);
			info.canStack = true;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Opportunist);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(void_Opportunist), tex, abIds);

			// set ability to behaviour class
			void_Opportunist.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Opportunist : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

	}

	[HarmonyPatch(typeof(PlayableCard), "GetPassiveAttackBuffs")]
	public class void_OpportunistPatch
	{
		[HarmonyPostfix]
		public static void Postfix(ref int __result, ref PlayableCard __instance)
		{
			if (__instance.OnBoard)
			{
				if (__instance.slot.opposingSlot.Card == null && __instance.Info.HasAbility(void_Opportunist.ability))
				{
					List<Ability> baseAbilities = __instance.Info.Abilities;
					List<Ability> modAbilities = __instance.Info.ModAbilities;
					int finalBuff = baseAbilities.Where(a => a == void_Opportunist.ability).Count() + modAbilities.Where(a => a == void_Opportunist.ability).Count() + __result;
					__result = finalBuff;
				}
			}
		}
	}
}