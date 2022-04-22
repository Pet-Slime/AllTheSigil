using HarmonyLib;
using DiskCardGame;
using UnityEngine;
using System.Collections;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Inspired by Nevernamed, coded differently.
		private void AddSubmergedAmbush()
		{
			// setup ability
			const string rulebookName = "Submerged Ambush";
			const string rulebookDescription = "[creature] will deal 1 damage to cards that attacked over it while it was face-down. Does not affect cards that are Airborne.";
			const string LearnDialogue = "It strikes from the water.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_SubmergedAmbush);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_SubmergedAmbush_a2);
			int powerlevel = 4;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_SubmergedAmbush.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_SubmergedAmbush), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_SubmergedAmbush : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


	}

	[HarmonyPatch(typeof(CombatPhaseManager), "SlotAttackSlot", MethodType.Normal)]
	public class CombatPhaseManager_SlotAttackSlot_SubmergedAmbushAttack
	{
		[HarmonyPostfix]
		public static IEnumerator Postfix(IEnumerator enumerator, CardSlot attackingSlot, CardSlot opposingSlot, float waitAfter = 0f)
		{
			if (attackingSlot.Card != null && opposingSlot.Card != null && opposingSlot.Card.FaceDown && opposingSlot.Card.HasAbility(void_SubmergedAmbush.ability) && !attackingSlot.Card.AttackIsBlocked(opposingSlot) && !attackingSlot.Card.HasAbility(Ability.Flying))
			{
				yield return enumerator;
				yield return new WaitForSeconds(0.55f);
				yield return attackingSlot.Card.TakeDamage(1, opposingSlot.Card);

			} else
            {
				yield return enumerator;

			}
			yield break;
		}
	}
}