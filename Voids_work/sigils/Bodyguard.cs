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
		private NewAbility AddBodyguard()
		{
			// setup ability
			const string rulebookName = "Bodyguard";
			const string rulebookDescription = "[creature] will redirect the initial attack of a card to it.";
			const string LearnDialogue = "A protector, till the very end.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 8);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.bodyguard_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_bodyguard);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_bodyguard), tex, abIds);

			// set ability to behaviour class
			void_bodyguard.ability = newAbility.ability;

			

			return newAbility;
		}
	}

	public class void_bodyguard : AbilityBehaviour
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
				PlayableCard card = attackingSlot.Card;

				if (opposingSlot.Card != null)
				{
					Plugin.Log.LogDebug("bodyguard test succesfully patched");
					List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(opposingSlot);

					if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < opposingSlot.Index)
					{
						if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
						{
							Plugin.Log.LogDebug("Bodyguard test: card: " + adjacentSlots[0].Card);
							Plugin.Log.LogDebug("Bodyguard test: ability: " + adjacentSlots[0].Card.HasAbility(void_bodyguard.ability));
							if (adjacentSlots[0].Card.Info.HasAbility(void_bodyguard.ability))
							{
								opposingSlot = adjacentSlots[0];
							}
						}
						adjacentSlots.RemoveAt(0);
					}
					if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
					{
						Plugin.Log.LogDebug("bodyguard test: card: " + adjacentSlots[0].Card);
						Plugin.Log.LogDebug("bodyguard test: ability: " + adjacentSlots[0].Card.HasAbility(void_bodyguard.ability));
						if (adjacentSlots[0].Card.Info.HasAbility(void_bodyguard.ability))
						{
							opposingSlot = adjacentSlots[0];
						}
					}
				}
			}
		}
	}
}