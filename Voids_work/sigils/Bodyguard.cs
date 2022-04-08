using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private void AddBodyguard()
		{
			// setup ability
			const string rulebookName = "Bodyguard";
			const string rulebookDescription = "[creature] will redirect the initial attack of a card to it, if the attack was targeting a card in an adjacent space.";
			const string LearnDialogue = "A protector, till the very end.";
			//const string rulebookNameChinese = "小嘲讽;
			const string rulebookDescriptionChinese = "[creature]相邻的造物受到攻击时，它会替代受到攻击。";
			const string LearnDialogueChinese = "这个造物能吸引周围造物的仇恨并保护它们。";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_bodyguard);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Bodyguard_a2);
			int powerlevel = 2;
			bool LeshyUsable = false;
			bool part1Shops = false;
			bool canStack = false;

			// set ability to behaviour class
			if (Localization.CurrentLanguage == Language.ChineseSimplified)
				void_Bodyguard.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescriptionChinese, typeof(void_Bodyguard), tex_a1, tex_a2, LearnDialogueChinese,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
			else
				void_Bodyguard.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Bodyguard), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Bodyguard : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

	}


	[HarmonyPatch(typeof(CombatPhaseManager), "SlotAttackSlot", MethodType.Normal)]
	public class AttackIsBlocked_Bodyguard_Patch
	{
		[HarmonyPrefix]
		public static void SlotAttackSlot(ref CardSlot attackingSlot, ref CardSlot opposingSlot, float waitAfter = 0f)
		{
			if (attackingSlot.Card != null && opposingSlot.Card != null && !attackingSlot.Card.HasAbility(Ability.AllStrike))
			{
				if (opposingSlot.Card != null && !opposingSlot.Card.InOpponentQueue)
				{
					PlayableCard card = attackingSlot.Card;

					List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(opposingSlot);

					if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < opposingSlot.Index)
					{
						if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
						{
							if (adjacentSlots[0].Card.Info.HasAbility(void_Bodyguard.ability))
							{
								opposingSlot = adjacentSlots[0];
							}
						}
						adjacentSlots.RemoveAt(0);
					}
					if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
					{
						if (adjacentSlots[0].Card.Info.HasAbility(void_Bodyguard.ability))
						{
							opposingSlot = adjacentSlots[0];
						}
					}
				}
			}
		}
	}
}