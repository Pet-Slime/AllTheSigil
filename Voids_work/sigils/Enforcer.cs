using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using HarmonyLib;
using System.Collections.Generic;
using Pixelplacement;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private void AddEnforcer()
		{
			// setup ability
			const string rulebookName = "Enforcer";
			const string rulebookDescription = "At the start of the owner's turn, [creature] will cause adjacent creatures to attack.";
			const string LearnDialogue = "It causes it's allies to attack.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Enforcer);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Enforcer_a2);
			int powerlevel = 7;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Enforcer.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Enforcer), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Enforcer : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public static bool isCombatPhase = false;


		public int DamageDealtThisPhase { get; private set; }

		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			return base.Card.OpponentCard != playerUpkeep;
		}


		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{
			CardSlot slot = base.Card.slot;
			List<CardSlot> attackingSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(slot);
			if (attackingSlots.Count > 0)
			{
				if (Plugin.voidCombatPhase == false)
				{
					yield return FakeCombat.FakeCombatPhase(base.Card.slot.IsPlayerSlot, null, attackingSlots);
				}
				else
				{
					foreach (CardSlot attacker in attackingSlots)
					{
						yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSequence(attacker);
					}
				}
			}
			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.1f);
			yield break;

		}
	}
}