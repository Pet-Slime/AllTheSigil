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
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.piercing_sigil_a2);

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
				yield return ShowDamageSequence(base.Card.Info.Attack, base.Card.Info.Attack, false);
			}
			yield break;
		}

		//Port KCM damage formula to fix sigils that deal damage to leshy
		public IEnumerator ShowDamageSequence(int damage, int numWeights, bool toPlayer, float waitAfter = 0.125f, GameObject alternateWeightPrefab = null, float waitBeforeCalcDamage = 0f, bool changeView = true)
		{
			bool flag = damage > 1 && Singleton<OpponentAnimationController>.Instance != null;
			if (flag)
			{
				bool flag2 = P03AnimationController.Instance != null && P03AnimationController.Instance.CurrentFace == P03AnimationController.Face.Default;
				if (flag2)
				{
					P03AnimationController.Instance.SwitchToFace(toPlayer ? P03AnimationController.Face.Happy : P03AnimationController.Face.Angry, false, true);
				}
				else
				{
					bool flag3 = Singleton<LifeManager>.Instance.scales != null;
					if (flag3)
					{
						Singleton<OpponentAnimationController>.Instance.SetLookTarget(Singleton<LifeManager>.Instance.scales.transform, Vector3.up * 2f);
					}
				}
			}
			bool flag4 = Singleton<LifeManager>.Instance.scales != null;
			if (flag4)
			{
				if (changeView)
				{
					Singleton<ViewManager>.Instance.SwitchToView(Singleton<LifeManager>.Instance.scalesView, false, false);
					yield return new WaitForSeconds(0.1f);
				}
				yield return Singleton<LifeManager>.Instance.scales.AddDamage(damage, numWeights, toPlayer, alternateWeightPrefab);
				bool flag5 = waitBeforeCalcDamage > 0f;
				if (flag5)
				{
					yield return new WaitForSeconds(waitBeforeCalcDamage);
				}
				if (toPlayer)
				{
					Singleton<LifeManager>.Instance.PlayerDamage += damage;
				}
				else
				{
					Singleton<LifeManager>.Instance.OpponentDamage += damage;
				}
				yield return new WaitForSeconds(waitAfter);
			}
			bool flag6 = Singleton<OpponentAnimationController>.Instance != null;
			if (flag6)
			{
				bool flag7 = P03AnimationController.Instance != null && (P03AnimationController.Instance.CurrentFace == P03AnimationController.Face.Angry || P03AnimationController.Instance.CurrentFace == P03AnimationController.Face.Happy);
				if (flag7)
				{
					P03AnimationController.Instance.PlayFaceStatic();
					P03AnimationController.Instance.SwitchToFace(P03AnimationController.Face.Default, false, false);
				}
				else
				{
					Singleton<OpponentAnimationController>.Instance.ClearLookTarget();
				}
			}
			yield break;
		}

	}
}