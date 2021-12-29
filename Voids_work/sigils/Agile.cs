using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using APIPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddAgile()
		{
			// setup ability
			const string rulebookName = "Agile";
			const string rulebookDescription = "[creature] will dodge oncoming attacks, causing them to hit directly.";
			const string LearnDialogue = "The Card jumped out of the way to save itself...";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 1, Plugin.configAgile.Value);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_agile);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_agile), tex, abIds);

			// set ability to behaviour class
			void_agile.ability = newAbility.ability;

			

			return newAbility;
		}
	}

	public class void_agile : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			return slot == base.Card.Slot;
		}


		public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, true);
			CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, false);
			bool flag = toLeft != null && toLeft.Card == null;
			bool toRightValid = toRight != null && toRight.Card == null;
			if (flag || toRightValid)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return new WaitForSeconds(0.2f);
				if (toRightValid)
				{
					yield return Singleton<BoardManager>.Instance.AssignCardToSlot(base.Card, toRight, 0.1f, null, true);
				}
				else
				{
					yield return Singleton<BoardManager>.Instance.AssignCardToSlot(base.Card, toLeft, 0.1f, null, true);
				}
				base.Card.Anim.StrongNegationEffect();
				base.Card.RenderCard();
				yield return new WaitForSeconds(0.2f);
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
				yield return new WaitForSeconds(0.2f);
				yield return base.StartCoroutine(base.LearnAbility(0.5f));
				yield return new WaitForSeconds(0.2f);
			}
			yield break;
		}
	}
}