using System.Collections;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;
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
			const string rulebookDescription = "When [creature] decalres an attack, they will deal half the damage to cards adjacent to the target.";
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
			CardSlot centerSlot = target.slot;
			double newDamage = System.Math.Floor(damage * 0.5);
			int finalDamage = (int)newDamage;
			if (!SaveManager.SaveFile.IsPart2)
            {
				Singleton<TableVisualEffectsManager>.Instance.ThumpTable(0.3f);
				AudioController.Instance.PlaySound3D("teslacoil_overload", MixerGroup.TableObjectsSFX, centerSlot.transform.position, 1f, 0f, null, null, null, null, false);
				GameObject gameObject = Object.Instantiate<GameObject>(ResourceBank.Get<GameObject>("Prefabs/Environment/TableEffects/LightningBolt"));
				gameObject.GetComponent<LightningBoltScript>().StartObject = attacker.gameObject;
				gameObject.GetComponent<LightningBoltScript>().EndObject = centerSlot.Card.gameObject;
				yield return new WaitForSeconds(0.2f);
				Object.Destroy(gameObject, 0.25f);
				centerSlot.Card.Anim.StrongNegationEffect();
				target.Anim.PlayHitAnimation();
			} else
            {
				bool impactFrameReached = false;
				base.Card.Anim.PlayAttackAnimation(false, centerSlot, delegate ()
				{
					impactFrameReached = true;
				});
				yield return new WaitUntil(() => impactFrameReached);
			}
			yield return target.TakeDamage(finalDamage, attacker);
			yield return new WaitForSeconds(0.2f);
			yield break;
		}

	}
}