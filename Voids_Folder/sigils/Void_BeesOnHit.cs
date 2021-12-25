using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Resources.Resources;
using System;
using Random = UnityEngine.Random;
using Pixelplacement;

namespace voidSigils
{
	public partial class Plugin
	{
		private static void RemoveVanillaBeesOnHit()
		{
			Ability port;
			Ability original;

			port = void_BeesOnHit.ability;
			original = Ability.BeesOnHit;
			var card = ScriptableObjectLoader<CardInfo>.AllData;
			var ability = AbilitiesUtil.allData;

			Plugin.Log.LogMessage("Patching BeesOnHit");

			for (int index = 0; index < card.Count; index++)
			{
				CardInfo info = card[index];
				if (info.HasAbility(original))
				{
					Plugin.Log.LogMessage("Switching Abilities on Card: " + info.name);
					info.DefaultAbilities.Remove(original);
					info.DefaultAbilities.Add(port);
				}
			}

			for (int index = 0; index < ability.Count; index++)
			{
				AbilityInfo info = ability[index];
				if (info.ability == original)
				{
					info.metaCategories.Clear();
					info.powerLevel = 8;
					info.opponentUsable = false;
					Plugin.Log.LogMessage("Removing original BeesOnHit ability from the rulebook, totems, and Leshy: " + info.rulebookName);
				}
			}

		}

		private NewAbility AddPatchedBeesOnHit()
		{
			// setup ability
			const string rulebookName = "Bees Within";
			const string rulebookDescription = "Once a card bearing this sigil is struck, a bee card is created for the owner of the struck card. A bee is defined as: 1 power, 1 health, airborn.";
			const string LearnDialogue = "Stirring the hive has been never good.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -2);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.ability_beesonhit);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(void_BeesOnHit), tex, abIds);

			// set ability to behaviour class
			void_BeesOnHit.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_BeesOnHit : DrawCreatedCard
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private List<PlayableCard> queuedCards = new List<PlayableCard>();

		public override CardInfo CardToDraw
		{
			get
			{
				return CardLoader.GetCardByName("Bee");
			}
		}

		public override bool RespondsToTakeDamage(PlayableCard source)
		{
			return true;
		}

		public override IEnumerator OnTakeDamage(PlayableCard source)
		{
			if (base.Card.slot.IsPlayerSlot)
            {
				yield return base.PreSuccessfulTriggerSequence();
				base.Card.Anim.StrongNegationEffect();
				yield return new WaitForSeconds(0.4f);
				yield return base.CreateDrawnCard();
				yield return base.LearnAbility(0.5f);
			} else
            {
				yield return base.PreSuccessfulTriggerSequence();
				base.Card.Anim.StrongNegationEffect();
				List<CardSlot> opponentSlotsCopy = Singleton<BoardManager>.Instance.OpponentSlotsCopy;
				queuedCards = Singleton<Opponent>.Instance.Queue;
				opponentSlotsCopy.RemoveAll((CardSlot x) => this.queuedCards.Find((PlayableCard y) => y.QueuedSlot == x));
				if (opponentSlotsCopy.Count != 0)
                {
					CardSlot randomTarget = opponentSlotsCopy[Random.Range(0, (opponentSlotsCopy.Count - 1))];
					yield return Singleton<TurnManager>.Instance.opponent.QueueCard(this.CardToDraw, randomTarget, true, true, true);
				}
				yield return base.LearnAbility(0.5f);
			}
			
			yield break;
		}

	}

}