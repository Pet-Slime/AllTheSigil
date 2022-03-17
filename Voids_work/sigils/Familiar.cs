using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private void AddFamiliar()
		{
			// setup ability
			const string rulebookName = "Familiar";
			const string rulebookDescription = "A familiar will help with attacking when it's adjacent allies attack a card.";
			const string LearnDialogue = "A familiar helps those in need.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Familiar);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Familiar_a2);
			int powerlevel = 1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Familiar.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Familiar), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Familiar : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;



		public override bool RespondsToOtherCardDealtDamage(PlayableCard attacker, int amount, PlayableCard target)
		{
			return true;
		}
		public override IEnumerator OnOtherCardDealtDamage(PlayableCard attacker, int amount, PlayableCard target)
		{
			yield return base.PreSuccessfulTriggerSequence();
			CardSlot slotSaved = base.Card.slot;
			CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, true);
			CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, false);

			if (toLeft != null && toLeft.Card != null && toLeft.Card == attacker && !target.Dead && !target.InOpponentQueue)
            {
				yield return new WaitForSeconds(0.1f);
				yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(slotSaved, target.slot);
				yield return new WaitForSeconds(0.1f);
			}

			if (toRight != null && toRight.Card != null && toRight.Card == attacker && !target.Dead && !target.InOpponentQueue)
			{
				yield return new WaitForSeconds(0.1f);
				yield return Singleton<CombatPhaseManager>.Instance.SlotAttackSlot(slotSaved, target.slot);
				yield return new WaitForSeconds(0.1f);
			}
			yield return base.LearnAbility(0.1f);
			yield break;
		}
	}
}