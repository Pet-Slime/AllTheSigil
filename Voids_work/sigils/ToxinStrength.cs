using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private void AddToxinStrength()
		{
			// setup ability
			const string rulebookName = "Toxin (Strength)";
			const string rulebookDescription = "When [creature] damages another creature, that creature looses 1 power.";
			const string LearnDialogue = "Even once combat is over, strength leaves it's target";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Toxin_Strength);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Toxin_Strength_a2);
			int powerlevel = 2;
			bool LeshyUsable = Plugin.configToxin.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Toxin_Strength.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Toxin_Strength), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Toxin_Strength : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private CardModificationInfo mod;

		private void Start()
		{
			this.mod = new CardModificationInfo();
			this.mod.attackAdjustment = -1;
		}

		public override bool RespondsToDealDamage(int amount, PlayableCard target)
		{
			if (target.Dead)
            {
				return false;
            }
			return true;
		}

		public override IEnumerator OnDealDamage(int amount, PlayableCard target)
		{
			if (target != null)
            {
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
				yield return new WaitForSeconds(0.1f);
				base.Card.Anim.LightNegationEffect();
				yield return base.PreSuccessfulTriggerSequence();
				CardModificationInfo cardModificationInfo = target.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "void_Toxin_Strength");
				if (cardModificationInfo == null)
				{
					cardModificationInfo = new CardModificationInfo();
					cardModificationInfo.singletonId = "void_Toxin_Strength";
					target.AddTemporaryMod(cardModificationInfo);
				}
				cardModificationInfo.attackAdjustment--;
				target.OnStatsChanged();
				yield return new WaitForSeconds(0.1f);
				yield return base.LearnAbility(0.1f);
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
				
            }
			yield break;
		}

	}
}