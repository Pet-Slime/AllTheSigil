using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Balrog Bean
		private NewAbility AddEletric()
		{
			// setup ability
			const string rulebookName = "Electric";
			const string rulebookDescription = "[creature] attacks a creatre, they will deal half the damage to creatures adjacent to the target.";
			const string LearnDialogue = "Shocking";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 4, true);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_eletric);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_eletric), tex, abIds);

			// set ability to behaviour class
			void_eletric.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_eletric : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDealDamage(int amount, PlayableCard target)
		{
			return amount > 0 && target != null;
		}

		public override IEnumerator OnDealDamage(int amount, PlayableCard target)
		{
			CardSlot baseSlot = base.Card.slot;
			List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(baseSlot.opposingSlot);
			yield return new WaitForSeconds(0.2f);
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < baseSlot.Index)
			{
				if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
				{
					yield return this.ShockCard(adjacentSlots[0].Card, baseSlot.Card, amount);
				}
				adjacentSlots.RemoveAt(0);
			}
			yield return new WaitForSeconds(0.2f);
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
			{
				yield return this.ShockCard(adjacentSlots[0].Card, baseSlot.Card, amount);
			}
			yield break;
		}

		private IEnumerator ShockCard(PlayableCard target, PlayableCard attacker, int damage)
		{
			
			double newDamage = System.Math.Floor(damage * 0.5);
			int finalDamage = (int)newDamage;
			if (attacker.Anim is CardAnimationController)
			{
				(attacker.Anim as CardAnimationController).PlayAttackAnimation(false, target.slot);

			}
			yield return target.TakeDamage(finalDamage, attacker);
			yield return new WaitForSeconds(0.2f);
			yield break;
		}

	}
}