using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using APIPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddShove()
		{
			// setup ability
			const string rulebookName = "Ram";
			const string rulebookDescription = "[creature] will try to ram the card infront of it when played, or every upkeep till it succeeds once. It will send the rammed target to the queue if on my side, or back to the hand if on your side. Does not work during combat.";
			const string LearnDialogue = "Moving creatures around? How Rude!";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.rammer_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_shove);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(void_Shove), tex, abIds);

			// set ability to behaviour class
			void_Shove.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Shove : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public bool hasShoved = false;


		public override bool RespondsToResolveOnBoard()
		{
			if (hasShoved || Plugin.voidCombatPhase)
			{
				return false;
			}
			return base.Card.OnBoard && Plugin.voidCombatPhase == false;
		}

		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			Plugin.voidCombatPhase = false;
			return base.Card.OpponentCard != playerUpkeep && this.hasShoved == false;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			if (base.Card.Slot.IsPlayerSlot)
			{
				if (base.Card.slot.opposingSlot.Card != null
				&& base.Card.HasAbility(void_Shove.ability)
				&& base.Card.InOpponentQueue == false)
				{
					PlayableCard target = base.Card.slot.opposingSlot.Card;

					if (!target.FaceDown && !target.Info.HasTrait(Trait.Uncuttable) && !target.Info.HasTrait(Trait.Giant))
					{
						CardSlot oldSlot = target.slot;
						Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
						yield return new WaitForSeconds(0.1f);
						yield return new WaitForSeconds(0.1f);
						target.UnassignFromSlot();
						yield return Singleton<TurnManager>.Instance.Opponent.ReturnCardToQueue(target, 0.25f);
						yield return new WaitForSeconds(0.4f);
						yield return base.LearnAbility(0.25f);
						yield return new WaitForSeconds(0.1f);
						hasShoved = true;
					}
				}
			}
			else
			{
				if (base.Card.slot.opposingSlot.Card != null)
				{
					PlayableCard target = base.Card.slot.opposingSlot.Card;
					yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(target.Info, null, 0.25f, null);
					UnityEngine.Object.Destroy(target.gameObject);
					hasShoved = true;
				}
			}
			yield break;

		}

		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{
			if (base.Card.Slot.IsPlayerSlot && playerUpkeep == true)
			{
				if (base.Card.slot.opposingSlot.Card != null
				&& base.Card.HasAbility(void_Shove.ability)
				&& base.Card.InOpponentQueue == false)
				{
					PlayableCard target = base.Card.slot.opposingSlot.Card;

					if (!target.FaceDown && !target.Info.HasTrait(Trait.Uncuttable) && !target.Info.HasTrait(Trait.Giant))
					{
						CardSlot oldSlot = target.slot;
						Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
						yield return new WaitForSeconds(0.1f);
						yield return new WaitForSeconds(0.1f);
						target.UnassignFromSlot();
						yield return Singleton<TurnManager>.Instance.Opponent.ReturnCardToQueue(target, 0.25f);
						yield return new WaitForSeconds(0.4f);
						yield return base.LearnAbility(0.25f);
						yield return new WaitForSeconds(0.1f);
						hasShoved = true;
					}
				}
			}
			else
			{
				if (base.Card.slot.opposingSlot.Card != null && playerUpkeep == false)
				{

					PlayableCard target = base.Card.slot.opposingSlot.Card;

					yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(target.Info, null, 0.25f, null);
					target.Anim.PlayDeathAnimation();
					CardSlot slotBeforeDeath = target.slot;
					target.UnassignFromSlot();
					target.StartCoroutine(target.DestroyWhenStackIsClear());
					slotBeforeDeath = null;
					hasShoved = true;
				}
			}
			yield break;
		}
	}
}