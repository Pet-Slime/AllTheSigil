using UnityEngine;
using DiskCardGame;
using System.Collections.Generic;
using APIPlugin;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System.Collections;
using HarmonyLib;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private NewAbility AddZapper()
		{
			// setup ability
			const string rulebookName = "Enrage";
			const string rulebookDescription = "[creature] will empower adjacent allies, increasing their strenght by 2. However, if they perish while empowered, they are permamently removed from your deck.";
			const string LearnDialogue = "A boost of strength, but at what cost...";

			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -1);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.no_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_enrage);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_zapper), tex, abIds);

			// set ability to behaviour class
			void_zapper.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_zapper : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public CardInfo cardInfo;

		public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
		{
			return base.Card.OnBoard && deathSlot.IsPlayerSlot && card != base.Card;
		}

		public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
		{
			//Get the Deck info
			DeckInfo currentDeck = SaveManager.SaveFile.CurrentDeck;

			//Find the card in the deck
			CardInfo ci = currentDeck.Cards.Find((CardInfo x) => x.name == card.Info.name);

			//get the base card
			PlayableCard zapper = base.Card;

			//get cards adjacent to the base card
			List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(zapper.slot);

			//if ci is null, then skip the rest
			if (ci == null)
            {
				yield break;
            }

			//check the lower number slot first 
			if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < zapper.slot.Index)
			{
				if (adjacentSlots[0] == deathSlot)
				{
					currentDeck.RemoveCard(ci);
					if (!base.HasLearned)
					{
						CustomCoroutine.WaitThenExecute(2f, delegate
						{
							Singleton<VideoCameraRig>.Instance.PlayCameraAnim("refocus_medium");
							Singleton<VideoCameraRig>.Instance.VOPlayer.PlayVoiceOver("God damn it.", "vo_goddamnit");
						}, false);
					}
					yield return base.LearnAbility(0.5f);
				}
				adjacentSlots.RemoveAt(0);
			}
			if (adjacentSlots.Count > 0 && adjacentSlots[0] == deathSlot)
			{
				currentDeck.RemoveCard(ci);
				if (!base.HasLearned)
				{
					CustomCoroutine.WaitThenExecute(2f, delegate
					{
						Singleton<VideoCameraRig>.Instance.PlayCameraAnim("refocus_medium");
						Singleton<VideoCameraRig>.Instance.VOPlayer.PlayVoiceOver("God damn it.", "vo_goddamnit");
					}, false);
				}
				yield return base.LearnAbility(0.5f);
			}
			yield break;
		}
	}

	[HarmonyPatch(typeof(PlayableCard), "GetPassiveAttackBuffs")]
	public class MakeZapperWorkTry3
	{
		[HarmonyPostfix]
		public static void Postfix(ref int __result, ref PlayableCard __instance)
		{
			if (__instance.OnBoard)
			{
				foreach (CardSlot slotState in Singleton<BoardManager>.Instance.GetAdjacentSlots(__instance.slot))
				{
					if (slotState.Card != null && slotState.Card.Info.HasAbility(void_zapper.ability))
					{
						__result = __result + 2;
					}
				}
			}
		}
	}


}