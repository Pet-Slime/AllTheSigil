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
		private NewAbility AddEletric()
		{
			// setup ability
			const string rulebookName = "Electric";
			const string rulebookDescription = "[creature] attacks a creature, they will deal half the damage to creatures adjacent to the target.";
			const string LearnDialogue = "Shocking";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3, Plugin.configElectric.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.void_Electric_a2);
			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Electric);

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