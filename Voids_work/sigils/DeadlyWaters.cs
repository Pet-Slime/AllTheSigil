using HarmonyLib;
using DiskCardGame;
using UnityEngine;
using System.Collections;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private void AddDeadlyWaters()
		{
			// setup ability
			const string rulebookName = "Deadly Waters";
			const string rulebookDescription = "[creature] will kill cards that attacked over it while it was face-down. Does not affect cards that have Airborne.";
			const string LearnDialogue = "It's not always safe to go into the waters.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_DeadlyWaters);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_DeadlyWaters_a2);
			int powerlevel = 4;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_DeadlyWaters.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_DeadlyWaters), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_DeadlyWaters : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;
	}

	[HarmonyPatch(typeof(CombatPhaseManager), "SlotAttackSlot", MethodType.Normal)]
	public class CombatPhaseManager_SlotAttackSlot_SubmergedDeathAttack
	{
		[HarmonyPostfix]
		public static IEnumerator Postfix(IEnumerator enumerator, CardSlot attackingSlot, CardSlot opposingSlot, float waitAfter = 0f)
		{
			if (attackingSlot.Card != null && opposingSlot.Card != null && opposingSlot.Card.FaceDown && opposingSlot.Card.HasAbility(void_DeadlyWaters.ability) && !attackingSlot.Card.AttackIsBlocked(opposingSlot) && !attackingSlot.Card.HasAbility(Ability.Flying))
			{
				yield return enumerator;
				yield return new WaitForSeconds(0.55f);
				yield return attackingSlot.Card.Die(false);

			} else
            {
				yield return enumerator;

			}
			yield break;
		}
	}
}