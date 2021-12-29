﻿using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using HarmonyLib;
using System.Linq;
using Pixelplacement;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by blind
		private NewAbility AddDeathburst()
		{
			// setup ability
			const string rulebookName = "Deathburst";
			const string rulebookDescription = "[creature] will deal one damage to the three cards infront of it when killed.";
			const string LearnDialogue = "Boom";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 8, Plugin.configDeathburst.Value);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_deathburst);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Deathburst), tex, abIds);

			// set ability to behaviour class
			void_Deathburst.ability = newAbility.ability;

			return newAbility;

		}
	}

	[HarmonyPatch(typeof(AbilityIconInteractable), "LoadIcon")]
	public class DearthBurstIcon
	{
		[HarmonyPostfix]
		public static void Postfix(ref Texture __result, ref CardInfo info, ref AbilityInfo ability)
		{
			if (ability.ability == void_Ambush.ability)
			{
				if (info != null)
				{
					Texture2D tex1 = SigilUtils.LoadTextureFromResource(Artwork.void_deathburst_1);

					Texture2D tex2 = SigilUtils.LoadTextureFromResource(Artwork.void_deathburst_2);

					Texture2D tex3 = SigilUtils.LoadTextureFromResource(Artwork.void_deathburst_3);

					List<Ability> baseAbilities = info.Abilities;

					int count = baseAbilities.Where(a => a == void_Ambush.ability).Count();

					if (count == 1)
					{
						__result = tex1;

					}
					else if (count == 2)
					{
						__result = tex2;
					}
					else if (count >= 3)
					{
						__result = tex3;
					}

				}
			}
		}
	}


	public class void_Deathburst : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private void Awake()
		{
			this.bombPrefab = ResourceBank.Get<GameObject>("Prefabs/Cards/SpecificCardModels/DetonatorHoloBomb");
		}

		public override bool RespondsToPreDeathAnimation(bool wasSacrifice)
		{
			return base.Card.OnBoard;
		}

		public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
		{
			base.Card.Anim.LightNegationEffect();
			yield return base.PreSuccessfulTriggerSequence();
			yield return this.ExplodeFromSlot(base.Card.Slot);
			yield return base.LearnAbility(0.25f);
			yield break;
		}

		protected IEnumerator ExplodeFromSlot(CardSlot slot)
		{
			List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(slot.opposingSlot);
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < slot.Index)
			{
				if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
				{
					yield return this.BombCard(adjacentSlots[0].Card, slot.Card);
				}
				adjacentSlots.RemoveAt(0);
			}
			if (slot.opposingSlot.Card != null && !slot.opposingSlot.Card.Dead)
			{
				yield return this.BombCard(slot.opposingSlot.Card, slot.Card);
			}
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
			{
				yield return this.BombCard(adjacentSlots[0].Card, slot.Card);
			}
			yield break;
		}

		private IEnumerator BombCard(PlayableCard target, PlayableCard attacker)
		{

			List<Ability> baseAbilities = base.Card.Info.Abilities;
			int count1 = baseAbilities.Where(a => a == void_Deathburst.ability).Count();
			List<Ability> modAbilities = base.Card.Info.ModAbilities;
			int count2 = modAbilities.Where(a => a == void_Deathburst.ability).Count();
			int countFinal = count1 + count2;
			GameObject bomb = UnityEngine.Object.Instantiate<GameObject>(this.bombPrefab);
			bomb.transform.position = attacker.transform.position + Vector3.up * 0.1f;
			Tween.Position(bomb.transform, target.transform.position + Vector3.up * 0.1f, 0.5f, 0f, Tween.EaseLinear, Tween.LoopType.None, null, null, true);
			yield return new WaitForSeconds(0.5f);
			target.Anim.PlayHitAnimation();
			UnityEngine.Object.Destroy(bomb);
			yield return target.TakeDamage(countFinal, attacker);
			yield break;
		}

		private const string BOMB_PREFAB_PATH = "Prefabs/Cards/SpecificCardModels/DetonatorHoloBomb";

		private GameObject bombPrefab;

	}
}