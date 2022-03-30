using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using Random = UnityEngine.Random;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private void AddBlind()
		{
			// setup ability
			const string rulebookName = "Random Strike";
			const string rulebookDescription = "[creature] will strike at opponent slots randomly when it attacks.";
			const string LearnDialogue = "They strike widly...";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Blind);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Blind_a2);
			int powerlevel = -1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;


			// set ability to behaviour class
			void_Blind.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Blind), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Blind : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

	}

	[HarmonyPatch(typeof(CombatPhaseManager), "SlotAttackSlot", MethodType.Normal)]
	public class SlotAttackSlot_Blind_Patch
	{
		[HarmonyPrefix]
		public static void SlotAttackSlot(ref CardSlot attackingSlot,ref CardSlot opposingSlot, float waitAfter = 0f)
		{
			if (attackingSlot.Card != null && attackingSlot.Card.HasAbility(void_Blind.ability) && !attackingSlot.Card.HasAbility(Ability.AllStrike))
			{
				PlayableCard card = attackingSlot.Card;

				// Get all slots
				List<CardSlot> allSlots = new List<CardSlot>();
				if (card.slot.IsPlayerSlot)
                {
					allSlots = Singleton<BoardManager>.Instance.opponentSlots;
				} else
                {
					allSlots = Singleton<BoardManager>.Instance.playerSlots;
				}

				CardSlot target = allSlots[SeededRandom.Range(0, (allSlots.Count), SaveManager.SaveFile.GetCurrentRandomSeed())];

				opposingSlot = target;

			}
		}
	}
}