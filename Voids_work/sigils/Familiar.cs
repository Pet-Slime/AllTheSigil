using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using HarmonyLib;
using System;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private NewAbility AddFamiliar()
		{
			// setup ability
			const string rulebookName = "Familiar";
			const string rulebookDescription = "A familiar will help with attacking when it's adjacent allies attack a card.";
			const string LearnDialogue = "A familiar helps those in need.";
			// const string TextureFile = "Artwork/void_vicious.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 1, Plugin.configFamiliar.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.void_familair_a2);
			info.flipYIfOpponent = true;
			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_familair);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Familiar), tex, abIds);

			// set ability to behaviour class
			void_Familiar.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Familiar : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;



		public override bool RespondsToOtherCardDealtDamage(PlayableCard attacker, int amount, PlayableCard target)
		{
			return true;
		}
		public override IEnumerator OnOtherCardDealtDamage(PlayableCard attacker, int amount, PlayableCard target)
		{
			yield return base.PreSuccessfulTriggerSequence();
			CardSlot slotSaved = base.Card.slot;
			CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, true);
			CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, false);

			if (toLeft != null && toLeft.Card != null && toLeft.Card == attacker && !target.Dead && !target.InOpponentQueue)
            {
				yield return new WaitForSeconds(0.1f);
				yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(slotSaved, target.slot);
				yield return new WaitForSeconds(0.1f);
			}

			if (toRight != null && toRight.Card != null && toRight.Card == attacker && !target.Dead && !target.InOpponentQueue)
			{
				yield return new WaitForSeconds(0.1f);
				yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(slotSaved, target.slot);
				yield return new WaitForSeconds(0.1f);
			}
			yield return base.LearnAbility(0.1f);
			yield break;
		}
	}
}