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
		private void AddOpportunist()
		{
			// setup ability
			const string rulebookName = "Opportunist";
			const string rulebookDescription = "[creature] will gain 1 power for each instance of Opportunist, when the opposing slot is empty.";
			const string LearnDialogue = "It takes it's chance when it gets it.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Opportunist);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Opportunist_a2);
			int powerlevel = 2;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = true;

			// set ability to behaviour class
			void_Opportunist.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Opportunist), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	[HarmonyPatch(typeof(AbilityIconInteractable), "LoadIcon")]
	public class void_Opportunist_Icon
	{
		[HarmonyPostfix]
		public static void Postfix(ref Texture __result, ref CardInfo info, ref AbilityInfo ability)
		{
			if (ability.ability == void_Opportunist.ability)
			{
				if (info != null && !SaveManager.SaveFile.IsPart2)
				{
					//Get count of how many instances of the ability the card has
					int count = Mathf.Max(info.Abilities.FindAll((Ability x) => x == void_Opportunist.ability).Count, 1);
					//Switch statement to the right texture
					switch (count)
					{
						case 1:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Opportunist_1);
							break;
						case 2:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Opportunist_2);
							break;
						case 3:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Opportunist_3);
							break;
						case 4:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Opportunist_4);
							break;
						case 5:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Opportunist_5);
							break;
					}
				}
			}
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
					int count = SigilUtils.getAbilityCount(__instance, void_Opportunist.ability);
					__result = count + __result;
				}
			}
		}
	}
}