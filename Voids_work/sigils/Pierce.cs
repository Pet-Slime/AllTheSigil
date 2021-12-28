using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by BarlogBean
		private NewAbility AddPierce()
		{
			// setup ability
			const string rulebookName = "Pierce";
			const string rulebookDescription = "[creature] attacks the card in queue behind it's initial target.";
			const string LearnDialogue = "My creatures!";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 4);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_pierce);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_pierce), tex, abIds);

			// set ability to behaviour class
			void_pierce.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_pierce : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			CardSlot opposingSlot = base.Card.slot.opposingSlot;
			PlayableCard target = slot.Card;
			PlayableCard queuedCard = Singleton<BoardManager>.Instance.GetCardQueuedForSlot(opposingSlot);

			return target != null && !target.Dead && queuedCard != null && attacker.HasAbility(void_pierce.ability);
		}

		public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			CardSlot opposingSlot = base.Card.slot.opposingSlot;
			PlayableCard queuedCard = Singleton<BoardManager>.Instance.GetCardQueuedForSlot(opposingSlot);
			if (queuedCard != null)
			{
				yield return base.PreSuccessfulTriggerSequence();
				yield return new WaitForSeconds(0.25f);
				queuedCard.Anim.LightNegationEffect();
				yield return new WaitForSeconds(0.25f);
				if (!queuedCard.FaceDown)
				{ 
					if (base.Card.Anim is CardAnimationController)
					{
						(base.Card.Anim as CardAnimationController).PlayAttackAnimation(false, queuedCard.Slot);

					}
				}
				yield return queuedCard.TakeDamage(base.Card.Info.Attack, base.Card);
				yield return base.LearnAbility(0f);
			} else if (queuedCard == null && base.Card.Info.HasAbility(void_trample.ability))
            {
				yield return base.PreSuccessfulTriggerSequence();
				yield return new WaitForSeconds(0.25f);
				queuedCard.Anim.LightNegationEffect();
				yield return new WaitForSeconds(0.25f);
				if (base.Card.Anim is CardAnimationController)
				{
					(base.Card.Anim as CardAnimationController).PlayAttackAnimation(false, base.Card.QueuedSlot);

				}
				yield return Singleton<LifeManager>.Instance.ShowDamageSequence(base.Card.Info.Attack, base.Card.Info.Attack, false);
			}
			yield break;
		}

	}
}