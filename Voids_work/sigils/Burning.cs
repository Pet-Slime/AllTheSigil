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
		private void AddBurning()
		{
			// setup ability
			const string rulebookName = "Burning";
			const string rulebookDescription = "[creature] is on fire, and will gain 1 power and loose 1 health each upkeep.";
			const string LearnDialogue = "It rampages while on fire.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Burning);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Burning_a2);
			int powerlevel = 0;
			bool LeshyUsable = Plugin.configToxin.Value;
			bool part1Shops = true;
			bool canStack = false;



			// set ability to behaviour class
			void_Burning.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Burning), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Burning : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;



		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			return base.Card.OpponentCard != playerUpkeep;
		}

		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{
			if (base.Card.HasAbility(Ability.Submerge) || base.Card.HasAbility(Ability.SubmergeSquid))
			{
				yield break;
			}
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
			yield return new WaitForSeconds(0.1f);
			base.Card.Anim.LightNegationEffect();
			yield return base.PreSuccessfulTriggerSequence();
			CardModificationInfo cardModificationInfo = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "void_Burning");
			if (cardModificationInfo == null)
			{
				cardModificationInfo = new CardModificationInfo();
				cardModificationInfo.singletonId = "void_Burning";
				base.Card.AddTemporaryMod(cardModificationInfo);
			}
			cardModificationInfo.attackAdjustment++;
			cardModificationInfo.healthAdjustment--;
			base.Card.OnStatsChanged();
			if (base.Card.Health <= 0)
			{
				yield return base.Card.Die(false, base.Card, true);
			}
			yield return new WaitForSeconds(0.1f);
			yield return base.LearnAbility(0.1f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}
	}
}