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
		private void AddStampede()
		{
			// setup ability
			const string rulebookName = "Stampede";
			const string rulebookDescription = "[creature] will cause adjacent creatures to attack when played on the board if played not during combat.";
			const string LearnDialogue = "Power in Numbers";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Stampede);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Stampede_a2);
			int powerlevel = 4;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Stampede.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Stampede), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Stampede : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


		public int DamageDealtThisPhase { get; private set; }

		public override bool RespondsToResolveOnBoard()
		{
			return base.Card.HasAbility(void_Stampede.ability);
		}


		public override IEnumerator OnResolveOnBoard()
		{
			CardSlot slot = base.Card.slot;
			List<CardSlot> attackingSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(slot);
			if (attackingSlots.Count > 0)
			{
				if (Plugin.voidCombatPhase == false)
				{
					yield return FakeCombat.FakeCombatPhase(base.Card.slot.IsPlayerSlot, null, attackingSlots);
				}
			}
			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.1f);
			yield break;

		}
	}
}