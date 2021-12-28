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
			if (ability.ability == void_Ambush.ability)
			{
				if (info != null)
				{
					Texture2D tex1 = SigilUtils.LoadTextureFromResource(Artwork.void_Medic_1);

					Texture2D tex2 = SigilUtils.LoadTextureFromResource(Artwork.void_Medic_2);

					Texture2D tex3 = SigilUtils.LoadTextureFromResource(Artwork.void_Medic_3);

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
			List<Ability> baseAbilities = base.Card.Info.Abilities;
			int count1 = baseAbilities.Where(a => a == void_Medic.ability).Count();
			List<Ability> modAbilities = base.Card.Info.ModAbilities;
			int count2 = modAbilities.Where(a => a == void_Medic.ability).Count();
			int finalCount = count1 + count2;



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