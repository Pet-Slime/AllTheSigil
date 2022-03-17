using HarmonyLib;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private void AddFrightful()
		{
			// setup ability
			const string rulebookName = "Frightful";
			const string rulebookDescription = "[creature] will cause opposing creatures to move out of the way when it attacks.";
			const string LearnDialogue = "Scary";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Frightful);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Frightful_a2);
			int powerlevel = 5;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Frightful.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Frightful), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Frightful : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			return base.Card == attacker && slot.Card != null;
		}

		public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			if (slot.Card != null)
			{
				PlayableCard opposingCard = slot.Card;
				CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(slot, true);
				CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(slot, false);
				bool toLeftValid = toLeft != null && toLeft.Card == null;
				bool toRightValid = toRight != null && toRight.Card == null;
				bool flag = toLeftValid || toRightValid;
				if (flag)
				{
					yield return base.PreSuccessfulTriggerSequence();
					yield return new WaitForSeconds(0.2f);
					base.Card.Anim.StrongNegationEffect();
					bool flag2 = toRightValid;
					if (flag2)
					{
						yield return Singleton<BoardManager>.Instance.AssignCardToSlot(opposingCard, toRight, 0.1f, null, true);
						opposingCard.Anim.StrongNegationEffect();
					}
					else
					{
						yield return Singleton<BoardManager>.Instance.AssignCardToSlot(opposingCard, toLeft, 0.1f, null, true);
						opposingCard.Anim.StrongNegationEffect();
					}
					yield return new WaitForSeconds(0.2f);
					Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
					yield return new WaitForSeconds(0.2f);
					yield return base.StartCoroutine(base.LearnAbility(0.5f));
					yield return new WaitForSeconds(0.2f);
				}
			}
			yield break;
		}
	}
}