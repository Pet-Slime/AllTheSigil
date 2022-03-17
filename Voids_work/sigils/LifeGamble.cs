using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using Random = UnityEngine.Random;

namespace voidSigils
{
	public partial class Plugin
	{
		private void AddLifeStatsUp()
		{
			// setup ability
			const string rulebookName = "Life Gambler";
			const string rulebookDescription = "At the end of the owner's turn, [creature] will deal 2 damage to the owner in exchange for a 0 to 6 increase in stats. Failing to pay this cost will result in death.";
			const string LearnDialogue = "Your blood, their strength!";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_lifeStatsUp);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_lifeStatsUp_a2);
			int powerlevel = 2;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_LifeStatsUp.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_LifeStatsUp), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_LifeStatsUp : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToTurnEnd(bool playerTurnEnd)
		{
			return base.Card != null && base.Card.OpponentCard != playerTurnEnd;
		}

		public override IEnumerator OnTurnEnd(bool playerTurnEnd)
		{

			
			yield return new WaitForSeconds(0.5f);
			var StatsUp = Random.Range(0, 6);
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);

			for (var index = 0; index < StatsUp; index++)
            {
				CardModificationInfo statsUp = new CardModificationInfo();

				var HealthorAttack = Random.Range(1, 100);

				if (HealthorAttack > 50)
				{
					yield return new WaitForSeconds(0.2f);
					yield return statsUp.healthAdjustment = 1;
					base.Card.AddTemporaryMod(statsUp);
					base.Card.Anim.StrongNegationEffect();

				} else
				{
					yield return new WaitForSeconds(0.2f);
					yield return statsUp.attackAdjustment = 1;
					base.Card.AddTemporaryMod(statsUp);
					base.Card.Anim.StrongNegationEffect();
				}
			}
			yield return new WaitForSeconds(0.1f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield return ShowDamageSequence(2, 2, true);

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