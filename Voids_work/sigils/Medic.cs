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
		private NewAbility AddMedic()
		{
			// setup ability
			const string rulebookName = "Medic";
			const string rulebookDescription = "[creature] will try heal a random friendly target, if there is one, during upkeep.";
			const string LearnDialogue = "A good patching";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 4);
			info.canStack = true;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.medic_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Medic);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Medic), tex, abIds);

			// set ability to behaviour class
			void_Medic.ability = newAbility.ability;

			return newAbility;

		}
	}

	[HarmonyPatch(typeof(AbilityIconInteractable), "LoadIcon")]
	public class MedicIcon
	{
		[HarmonyPostfix]
		public static void Postfix(ref Texture __result, ref CardInfo info, ref AbilityInfo ability)
		{
			if (ability.ability == void_Medic.ability)
			{
				if (info != null && !SaveManager.SaveFile.IsPart2)
				{
					//Get count of how many instances of the ability the card has
					int count = Mathf.Max(info.Abilities.FindAll((Ability x) => x == void_Medic.ability).Count, 1);
					//Switch statement to the right texture
					switch (count)
					{
						case 1:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Medic_1);
							break;
						case 2:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Medic_2);
							break;
						case 3:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Medic_3);
							break;
						case 4:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Medic_4);
							break;
						case 5:
							__result = SigilUtils.LoadTextureFromResource(Artwork.void_Medic_5);
							break;
					}
				}
			}
		}
	}



	public class void_Medic : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			return base.Card.OnBoard && base.Card.OpponentCard != playerUpkeep;
		}

		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{		
			// Get the base card
			PlayableCard card = base.Card;
			// Get the Medic count

			int finalCount = SigilUtils.getAbilityCount(card, void_Medic.ability);



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

				// pick a random target from the target list
				PlayableCard target = targets[Random.Range(0, (targets.Count))];
				base.Card.Anim.LightNegationEffect();
				yield return new WaitForSeconds(0.15f);
				yield return base.PreSuccessfulTriggerSequence();
				target.Anim.StrongNegationEffect();
				if (target.Status.damageTaken > 0)
				{
					target.HealDamage(finalCount);
				}
				yield return new WaitForSeconds(0.15f);
				yield return base.LearnAbility(0.25f);
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
				// pick a random target from the target list
				PlayableCard target = targets[Random.Range(0, (targets.Count))];
				base.Card.Anim.LightNegationEffect();
				yield return new WaitForSeconds(0.15f);
				yield return base.PreSuccessfulTriggerSequence();
				target.Anim.StrongNegationEffect();
				if (target.Status.damageTaken > 0)
				{
					target.HealDamage(finalCount);
				}
				yield return new WaitForSeconds(0.15f);
				yield return base.LearnAbility(0.25f);
			}
			yield break;
		}
	}
}