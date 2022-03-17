using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using Random = UnityEngine.Random;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by blind
		private void AddPossessor()
		{
			// setup ability
			const string rulebookName = "Possessor";
			const string rulebookDescription = "When [creature] perishes, it will grant a random friendly card that is on the board it's base power and health.";
			const string LearnDialogue = "It passes it's strength onto those who remain";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Possessor);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Possessor_a2);
			int powerlevel = 1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Possessor.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Possessor), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;

		}
	}

	public class void_Possessor : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private CardModificationInfo mod;

		private void Start()
		{
			this.mod = new CardModificationInfo();
		}


		public override bool RespondsToPreDeathAnimation(bool wasSacrifice)
		{
			return base.Card.OnBoard;
		}

		public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
		{		
			// Get the base card
			PlayableCard card = base.Card;
			this.mod.attackAdjustment = card.Info.baseAttack;
			this.mod.healthAdjustment = card.Info.baseHealth;

			if (card.slot.IsPlayerSlot)
			{
				// Get all slots
				List<CardSlot> allSlots = Singleton<BoardManager>.Instance.playerSlots;

				// Initalize target list
				List<PlayableCard> targets = new List<PlayableCard>();

				// Go thru all slots to see if there is a card in it, and if there is, add it to the target list
				for (int index = 0; index < allSlots.Count; index++)
				{
					if (allSlots[index].Card != null && allSlots[index].Card != base.Card)
					{
						targets.Add(allSlots[index].Card);
					}
				}
				if (targets.Count > 0)
                {
					// pick a random target from the target list
					PlayableCard target = targets[Random.Range(0, (targets.Count))];
					base.Card.Anim.LightNegationEffect();
					yield return base.PreSuccessfulTriggerSequence();
					target.Anim.StrongNegationEffect();
					target.temporaryMods.Add(this.mod);
					yield return base.LearnAbility(0.25f);
				}
				
			} else
            {
				// Get all slots
				List<CardSlot> allSlots = Singleton<BoardManager>.Instance.opponentSlots;

				// Initalize target list
				List<PlayableCard> targets = new List<PlayableCard>();

				// Go thru all slots to see if there is a card in it, and if there is, add it to the target list
				for (int index = 0; index < allSlots.Count; index++)
				{
					if (allSlots[index].Card != null && allSlots[index].Card != base.Card)
					{
						targets.Add(allSlots[index].Card);
					}
				}
				if (targets.Count > 0)
				{
					// pick a random target from the target list
					PlayableCard target = targets[Random.Range(0, (targets.Count))];
					base.Card.Anim.LightNegationEffect();
					yield return base.PreSuccessfulTriggerSequence();
					target.Anim.StrongNegationEffect();
					target.temporaryMods.Add(this.mod);
					yield return base.LearnAbility(0.25f);
				}
			}
			yield break;
		}
	}
}