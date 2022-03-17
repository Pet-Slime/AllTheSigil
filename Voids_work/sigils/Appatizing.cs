using HarmonyLib;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private void AddAppetizing()
		{
			// setup ability
			const string rulebookName = "Appetizing Target";
			const string rulebookDescription = "[creature] makes for a great target, causing the creature opposing a card bearing this sigil to gain 1 power.";
			const string LearnDialogue = "That creature makes the opponant stronger";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Antler);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = -2;
			bool LeshyUsable = Plugin.configAppetizing.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Appetizing.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Appetizing), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Appetizing : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

	}

	[HarmonyPatch(typeof(PlayableCard), "GetPassiveAttackBuffs")]
	public class PortOfAlarm
	{
		[HarmonyPostfix]
		public static void Postfix(ref int __result, ref PlayableCard __instance)
		{
			if (__instance.OnBoard)
			{
				if (__instance.slot.opposingSlot.Card != null && __instance.slot.opposingSlot.Card.Info.HasAbility(void_Appetizing.ability))
				{
					__result++;
				}
			}
		}
	}
}