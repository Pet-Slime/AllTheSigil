using HarmonyLib;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private NewAbility AddAppetizing()
		{
			// setup ability
			const string rulebookName = "Appetizing Target";
			const string rulebookDescription = "[creature] makes for a great target, causing the enemy across from it to gain one attack.";
			const string LearnDialogue = "That creature makes the opponant stronger";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, -2, true);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_alarm);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_appetizing), tex, abIds);

			// set ability to behaviour class
			void_appetizing.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_appetizing : AbilityBehaviour
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
				if (__instance.slot.opposingSlot.Card != null && __instance.slot.opposingSlot.Card.Info.HasAbility(void_appetizing.ability))
				{
					__result++;
				}
			}
		}
	}
}