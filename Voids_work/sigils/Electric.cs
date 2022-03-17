using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Balrog Bean
		private void AddEletric()
		{
			// setup ability
			const string rulebookName = "Electric";
			const string rulebookDescription = "When [creature] decalres an attack, they will deal half the damage to creatures adjacent to the target.";
			const string LearnDialogue = "Shocking";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Electric);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Electric_a2);
			int powerlevel = 3;
			bool LeshyUsable = Plugin.configElectric.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Electric.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Electric), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Electric : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			return base.Card == attacker;
		}

		public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			CardSlot baseSlot = base.Card.slot;
			List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(baseSlot.opposingSlot);
			yield return new WaitForSeconds(0.2f);
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < baseSlot.Index)
			{
				if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
				{
					yield return this.ShockCard(adjacentSlots[0].Card, baseSlot.Card, base.Card.Attack);
				}
				adjacentSlots.RemoveAt(0);
			}
			yield return new WaitForSeconds(0.2f);
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
			{
				yield return this.ShockCard(adjacentSlots[0].Card, baseSlot.Card, base.Card.Attack);
			}
			yield break;
		}

		private IEnumerator ShockCard(PlayableCard target, PlayableCard attacker, int damage)
		{
			
			double newDamage = System.Math.Floor(damage * 0.5);
			int finalDamage = (int)newDamage;
			target.Anim.SetOverclocked(true);
			target.Anim.PlayHitAnimation();
			yield return target.TakeDamage(finalDamage, attacker);
			target.Anim.SetOverclocked(false);
			yield return new WaitForSeconds(0.2f);
			yield break;
		}

	}
}