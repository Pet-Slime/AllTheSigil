using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using HarmonyLib;
using Artwork = voidSigils.Resources.Resources;
using System.Linq;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by blind
		private NewAbility AddMidas()
		{
			// setup ability
			const string rulebookName = "Midas";
			const string rulebookDescription = "[creature] will grant one tooth per instance of Midas when it kills a card.";
			const string LearnDialogue = "A bounty, paid in full.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 8, true);
			info.canStack = true;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Midas);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Midas), tex, abIds);

			// set ability to behaviour class
			void_Midas.ability = newAbility.ability;

			return newAbility;

		}
	}

	public class void_Midas : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
		{
			return killer.HasAbility(void_Midas.ability);
		}

		public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
		{
			List<Ability> baseAbilities = base.Card.Info.Abilities;

			int count1 = baseAbilities.Where(a => a == void_Midas.ability).Count();

			List<Ability> modAbilities = base.Card.Info.ModAbilities;

			int count2 = modAbilities.Where(a => a == void_Midas.ability).Count();

			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.25f);
			yield return CurrencyBowl.Instance.ShowGain(count1 + count2, true);
			yield return new WaitForSeconds(0.25f);
			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.25f);
			yield break;
		}

	}
}