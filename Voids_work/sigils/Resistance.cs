using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using APIPlugin;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private NewAbility AddResistant()
		{
			// setup ability
			const string rulebookName = "Resistant";
			const string rulebookDescription = "[creature] is so tough, it will only ever take one damage.";
			const string LearnDialogue = "A hardy creature that one is.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 4, Plugin.configResistant.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.resistant_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Resistant);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Resistant), tex, abIds);

			// set ability to behaviour class
			void_Resistant.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Resistant : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

	}

	[HarmonyPatch(typeof(PlayableCard), "TakeDamage")]
	public class TakeDamagePatch : PlayableCard
	{
		static void Prefix(ref PlayableCard __instance, ref int damage)
		{
			if (__instance.HasAbility(void_Resistant.ability))
			{
				damage = 1;
			}
			if (__instance.HasAbility(void_ThickShell.ability))
			{
				damage--;
			}
		}
	}
}