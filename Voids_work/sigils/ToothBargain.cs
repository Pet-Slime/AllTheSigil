using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System;
using Pixelplacement;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by blind
		private void AddToothpicker()
		{
			// setup ability
			const string rulebookName = "Tooth Bargain";
			const string rulebookDescription = "When [creature] is played, it will put 1 point of damage of it's opponent's side of the scale. When it perishes, it will put 2 damage on the owner's side of the scale.";
			const string LearnDialogue = "A deal with a devil I see...";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Tooth);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Tooth_a2);
			int powerlevel = 0;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Toothpicker.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Toothpicker), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Toothpicker : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToResolveOnBoard()
		{
			return true;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			yield return base.PreSuccessfulTriggerSequence();

			yield return new WaitForSeconds(0.25f);
			if (base.Card.slot.IsPlayerSlot)
            {
				yield return ShowDamageSequence(1, 1, false, 0.25f, null, 0f, true);
			}
			else
			{
				yield return ShowDamageSequence(1, 1, true, 0.25f, null, 0f, true);
			}

			yield return new WaitForSeconds(0.25f);
			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.25f);
			yield break;
		}

		public override bool RespondsToPreDeathAnimation(bool wasSacrifice)
		{
			return base.Card.OnBoard;
		}

		public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
		{
			base.Card.Anim.LightNegationEffect();
			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.25f);
			if (base.Card.slot.IsPlayerSlot)
			{
				yield return ShowDamageSequence(2, 2, true, 0.25f, null, 0f, true);
			} else
            {
				yield return ShowDamageSequence(2, 2, false, 0.25f, null, 0f, true);
			}
			yield return new WaitForSeconds(0.25f);
			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.25f);
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