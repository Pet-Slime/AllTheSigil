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
		private NewAbility AddToothpicker()
		{
			// setup ability
			const string rulebookName = "Tooth Bargain";
			const string rulebookDescription = "[creature] will put a tooth on your opponant's side of the scale when played, but will put two on yours when it dies.";
			const string LearnDialogue = "A deal with a devil I see...";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 0);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.tooth_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_tooth);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Toothpicker), tex, abIds);

			// set ability to behaviour class
			void_Toothpicker.ability = newAbility.ability;

			return newAbility;
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
			yield return ShowDamageSequence(1, 1, false, 0.25f, ResourceBank.Get<GameObject>("Prefabs/Environment/ScaleWeights/Weight_RealTooth"), 0f, true);
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
			yield return ShowDamageSequence(2, 2, true, 0.25f, null, 0f, true);
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