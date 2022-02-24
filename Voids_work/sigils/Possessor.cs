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
		private NewAbility AddPossessor()
		{
			// setup ability
			const string rulebookName = "Possessor";
			const string rulebookDescription = "When [creature] perishes, it will grant a random friendly card it's base power and health.";
			const string LearnDialogue = "It passes it's strength onto those who remain";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 0, true);
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.void_possessor_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_possessor);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_possessor), tex, abIds);

			// set ability to behaviour class
			void_possessor.ability = newAbility.ability;

			return newAbility;

		}
	}

	public class void_possessor : AbilityBehaviour
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