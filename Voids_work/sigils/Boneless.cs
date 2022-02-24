using UnityEngine;
using DiskCardGame;
using HarmonyLib;
using System.Collections;
using APIPlugin;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private NewAbility AddBoneless()
		{
			// setup ability
			const string rulebookName = "Boneless";
			const string rulebookDescription = "[creature] gives no bones! Any bones gained from sigils or death will be negated.";
			const string LearnDialogue = "That creature has no bones!";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, -1);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.void_boneless_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Boneless);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Boneless), tex, abIds);

			// set ability to behaviour class
			void_Boneless.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Boneless : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


		[HarmonyPatch(typeof(ResourcesManager), nameof(ResourcesManager.AddBones))]
		public class Boneless_Patch
		{
			[HarmonyPrefix]
			public static bool Prefix(int amount, CardSlot slot)
			{
				if (slot != null
				&& slot.Card != null
				&& slot.Card.HasAbility(void_Boneless.ability))
				{
					return false;
				} else
				{
					return true;
                }
			}
		}
	}
}