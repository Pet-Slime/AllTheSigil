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
		//Original
		private NewAbility AddBoneless()
		{
			// setup ability
			const string rulebookName = "Boneless";
			const string rulebookDescription = "[creature] leaves no bones on death!";
			const string LearnDialogue = "That creature has no bones!";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, -2);
			info.canStack = false;

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



		[HarmonyPatch(typeof(ResourcesManager), "AddBones")]
		[HarmonyPostfix]
		public static IEnumerator StopBones(IEnumerator sequenceResult, CardSlot slot)
		{
			if (slot != null
				&& slot.Card != null
				&& slot.Card.gameObject != null
				&& slot.Card.gameObject.GetComponent<void_Boneless>() != null)
			{
				yield break;
			}
			else
			{
				while (sequenceResult.MoveNext())
					yield return sequenceResult.Current;
			}

		}
	}

}