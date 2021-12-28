using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using HarmonyLib;
using Random = UnityEngine.Random;
using System.Linq;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by blind
		private NewAbility AddHerd()
		{
			// setup ability
			const string rulebookName = "Herd";
			const string rulebookDescription = "[creature] will summon a copy of itself each upkeep, up to three times.";
			const string LearnDialogue = "Strength in Numbers";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 4);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Heard_3);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_herd), tex, abIds);

			// set ability to behaviour class
			void_herd.ability = newAbility.ability;

			return newAbility;

		}
	}



	public class void_herd : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		int herdCount = 3;




		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			if (herdCount != 0) 
			{ 
				return base.Card.OnBoard && base.Card.OpponentCard != playerUpkeep;
			} else
            {
				return false;
            }
		}

		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{

			PlayableCard card = base.Card;

			Texture2D tex2 = SigilUtils.LoadTextureFromResource(Artwork.void_Heard_2);

			Texture2D tex1 = SigilUtils.LoadTextureFromResource(Artwork.void_Heard_1);

			Texture2D tex0 = SigilUtils.LoadTextureFromResource(Artwork.void_Heard_0);
			if (card.slot.IsPlayerSlot)
			{
				// Get all slots
				List<CardSlot> allSlots = Singleton<BoardManager>.Instance.playerSlots;

				// Initalize target list
				List<CardSlot> targets = new List<CardSlot>();

				// Go thru all slots to see if there is a card in it, and if there is, add it to the target list
				for (int index = 0; index < allSlots.Count; index++)
				{
					if (allSlots[index].Card == null)
					{
						targets.Add(allSlots[index]);
					}
				}
				// Add a card if there is one or more open slots
				if (targets.Count > 0)
				{
					// pick a random target from the target list
					CardSlot target = targets[Random.Range(0, (targets.Count))];
					base.Card.Anim.LightNegationEffect();
					yield return new WaitForSeconds(0.15f);
					yield return base.PreSuccessfulTriggerSequence();
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(base.Card.Info, target, 0.15f, true);
					//set up to negate the new card's herd ability, so it can't be spammed.
					CardModificationInfo negateMod = new CardModificationInfo();
					negateMod.negateAbilities.Add(void_herd.ability);

					//Clone the main card info so we don't touch the main card set
					CardInfo OpponentCardInfo = target.Card.Info.Clone() as CardInfo;

					//Add the modifincations
					OpponentCardInfo.Mods.Add(negateMod);

					//Update the opponant card info
					target.Card.SetInfo(OpponentCardInfo);
					yield return new WaitForSeconds(0.15f);
					yield return base.LearnAbility(0.25f);
					herdCount--;


					if (herdCount == 2)
					{
						base.Card.RenderInfo.OverrideAbilityIcon(void_herd.ability, tex2);
						base.Card.RenderCard();
					}

					if (herdCount == 1)
					{
						base.Card.RenderInfo.OverrideAbilityIcon(void_herd.ability, tex1);
						base.Card.RenderCard();
					}

					if (herdCount == 0)
					{
						base.Card.RenderInfo.OverrideAbilityIcon(void_herd.ability, tex0);
						base.Card.RenderCard();
					}
				}
			} else
            {
				// Get all slots
				List<CardSlot> allSlots = Singleton<BoardManager>.Instance.opponentSlots;

				// Initalize target list
				List<CardSlot> targets = new List<CardSlot>();

				// Go thru all slots to see if there is a card in it, and if there is, add it to the target list
				for (int index = 0; index < allSlots.Count; index++)
				{
					if (allSlots[index].Card == null)
					{
						targets.Add(allSlots[index]);
					}
				}
				// Add a card if there is one or more open slots
				if (targets.Count > 0)
                {
					// pick a random target from the target list
					CardSlot target = targets[Random.Range(0, (targets.Count))];
					base.Card.Anim.LightNegationEffect();
					yield return new WaitForSeconds(0.15f);
					yield return base.PreSuccessfulTriggerSequence();
					yield return Singleton<BoardManager>.Instance.CreateCardInSlot(base.Card.Info, target, 0.15f, true);
					//set up to negate the new card's herd ability, so it can't be spammed.
					CardModificationInfo negateMod = new CardModificationInfo();
					negateMod.negateAbilities.Add(void_herd.ability);

					//Clone the main card info so we don't touch the main card set
					CardInfo OpponentCardInfo = target.Card.Info.Clone() as CardInfo;

					//Add the modifincations
					OpponentCardInfo.Mods.Add(negateMod);

					//Update the opponant card info
					target.Card.SetInfo(OpponentCardInfo);
					yield return new WaitForSeconds(0.15f);
					yield return base.LearnAbility(0.25f);
					herdCount--;

					// Change icon
					if (herdCount == 2)
					{
						base.Card.RenderInfo.OverrideAbilityIcon(void_herd.ability, tex2);
						base.Card.RenderCard();
					}

					if (herdCount == 1)
					{
						base.Card.RenderInfo.OverrideAbilityIcon(void_herd.ability, tex1);
						base.Card.RenderCard();
					}

					if (herdCount == 0)
					{
						base.Card.RenderInfo.OverrideAbilityIcon(void_herd.ability, tex0);
						base.Card.RenderCard();
					}
				}
				
			}
			yield break;
		}




	}
}