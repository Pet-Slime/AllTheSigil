using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using HarmonyLib;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddTribalTutor()
		{
			// setup ability
			const string rulebookName = "Tribal Tutor";
			const string rulebookDescription = "When [creature] is played, you may search your deck for a card of the same tribe and take it into your hand. No tribe counts as a tribe of tribeless.";
			const string LearnDialogue = "It Calls for Kin.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 3);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.no_a2);
			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_TribeTutor);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_TribalTutor), tex, abIds);

			// set ability to behaviour class
			void_TribalTutor.ability = newAbility.ability;

			return newAbility;
		}
	}

	[HarmonyPatch(typeof(AbilityIconInteractable), "LoadIcon")]
	public class TribalTutorPatch
	{
		[HarmonyPostfix]
		public static void Postfix(ref Texture __result, ref CardInfo info, ref AbilityInfo ability)
		{
			if (ability.ability == void_TribalTutor.ability)
			{
				if (info != null)
				{
					Texture2D tex1 = SigilUtils.LoadTextureFromResource(Artwork.void_TribeTutor_bird);

					Texture2D tex2 = SigilUtils.LoadTextureFromResource(Artwork.void_TribeTutor_canine);

					Texture2D tex3 = SigilUtils.LoadTextureFromResource(Artwork.void_TribeTutor_hooved);

					Texture2D tex4 = SigilUtils.LoadTextureFromResource(Artwork.void_TribeTutor_insect);

					Texture2D tex5 = SigilUtils.LoadTextureFromResource(Artwork.void_TribeTutor_reptile);

					Texture2D tex6 = SigilUtils.LoadTextureFromResource(Artwork.void_TribeTutor_none);

					if (info.IsOfTribe(Tribe.Bird))
					{
						__result = tex1;

					}
					else if (info.IsOfTribe(Tribe.Canine))
					{
						__result = tex2;
					}
					else if (info.IsOfTribe(Tribe.Hooved))
					{
						__result = tex3;
					}
					else if (info.IsOfTribe(Tribe.Insect))
					{
						__result = tex4;
					}
					else if (info.IsOfTribe(Tribe.Reptile))
					{
						__result = tex5;
					}
					else
					{
						__result = tex6;
					}
				}
			}
		}
	}



	public class void_TribalTutor : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public List<CardInfo> TutorCards = new();

		public override bool RespondsToResolveOnBoard()
		{
			return true;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			yield return base.PreSuccessfulTriggerSequence();
			yield return base.LearnAbility(0.5f);
			base.Card.Anim.StrongNegationEffect();
			yield return new WaitForSeconds(0.4f);


			///Get the deck of cards in a list
			TutorCards = Singleton<CardDrawPiles>.Instance.Deck.cards;
			///Make a blank list for target cards in the deck, and non-target cards in the deck
			List<CardInfo> targets = new List<CardInfo>();
			List<CardInfo> nontargets = new List<CardInfo>();
			/// If the tribe count is > 0, then we know it has at least one tribe. if not, it is tribeless
			if (base.Card.Info.tribes.Count > 0)
			{
				/// Get the first tribe of a card (sorry multi tribe cards)
				Tribe cardTribe = base.Card.Info.tribes[0];

				///Run a for loop to go thru the list, filtering out all cards that is not nature temple, not in the card pool for act 1, and not of the same tribe
				for (int index = 0; index < TutorCards.Count; index++)
				{
					if (TutorCards[index].IsOfTribe(cardTribe))
					{
						///add those that pass to the target list
						targets.Add(TutorCards[index]);
					} else
                    {
						///add those that did not pass to the non-target list
						nontargets.Add(TutorCards[index]);
					}
				}
			}
			else
			{
				///For tribeless, we search for all other tribeless cards. then search out which ones are in the card pool and nature temple
				for (int index = 0; index < TutorCards.Count; index++)
				{
					if (TutorCards[index].tribes.Count == 0)
					{
						///add those that pass to the target list
						targets.Add(TutorCards[index]);
					}
					else
					{
						///add those that did not pass to the non-target list
						nontargets.Add(TutorCards[index]);
					}
				}
			}
			///if the targets are equal to zero, break here before we fuck with the deck
			if (targets.Count == 0) {yield break;}
			///Set the deck to just the target cards
			Singleton<CardDrawPiles>.Instance.Deck.cards = new List<CardInfo>(targets);
			///now tutor
			yield return Singleton<CardDrawPiles>.Instance.Deck.Tutor();
			///After the tutor, get the deck list and re-add all the cards we removed
			for (int index = 0; index < nontargets.Count; index++)
			{
				Singleton<CardDrawPiles>.Instance.Deck.cards.Add(nontargets[index]);
			}
			///call SpawnCards to correctly show how many cards are left.
			yield return Singleton<CardDrawPiles3D>.Instance.pile.SpawnCards(Singleton<CardDrawPiles>.Instance.Deck.cards.Count, 0.5f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}

	}
}