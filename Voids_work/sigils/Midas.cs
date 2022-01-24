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
		//Request by blind
		private NewAbility AddMidas()
		{
			// setup ability
			const string rulebookName = "Midas";
			const string rulebookDescription = "[creature] will grant one tooth per instance of Midas when it kills a card.";
			const string LearnDialogue = "A bounty, paid in full.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 2, false);
			info.canStack = true;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.midas_sigil_a2);

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

		public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
		{
			return fromCombat == true && killer.HasAbility(void_Midas.ability) && base.Card.OnBoard;
		}

		public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
		{
			if (fromCombat == true && killer.HasAbility(void_Midas.ability))
			{
				int count = SigilUtils.getAbilityCount(base.Card, void_Midas.ability);

				yield return base.PreSuccessfulTriggerSequence();
				yield return new WaitForSeconds(0.15f);
				Singleton<ViewManager>.Instance.SwitchToView(View.Scales, false, true);
				yield return new WaitForSeconds(0.25f);
				RunState.Run.currency += (count);
				yield return Singleton<CurrencyBowl>.Instance.ShowGain(count, true, false);
				yield return new WaitForSeconds(0.25f);
				yield return base.LearnAbility(0.25f);
				yield return new WaitForSeconds(0.1f);
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			}
			yield break;
		}

	}
}