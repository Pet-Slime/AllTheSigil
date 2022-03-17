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
		private void AddFireStarter()
		{
			// setup ability
			const string rulebookName = "Firestarter";
			const string rulebookDescription = "When [creature] damages another creature, that creature will gain the Burning Sigil. The Burning Sigil is define as: Each upkeep, this creature gains 1 strength but looses 1 health.";
			const string LearnDialogue = "Even once combat is over, it leaves a deadly mark";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Firestarter);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Firestarter_a2);
			int powerlevel = 2;
			bool LeshyUsable = Plugin.configToxin.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Firestarter.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Firestarter), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Firestarter : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDealDamage(int amount, PlayableCard target)
		{
			if (target.Dead)
			{
				return false;
			}
			return base.Card.HasAbility(void_Firestarter.ability);
		}

		public override IEnumerator OnDealDamage(int amount, PlayableCard target)
		{
			if (target)
            {
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
				yield return new WaitForSeconds(0.1f);
				base.Card.Anim.LightNegationEffect();
				yield return base.PreSuccessfulTriggerSequence();
				//make the card mondification info
				CardModificationInfo cardModificationInfo = new CardModificationInfo(void_Burning.ability);
				//Clone the main card info so we don't touch the main card set
				CardInfo targetCardInfo = target.Info.Clone() as CardInfo;
				//Add the modifincations to the cloned info
				targetCardInfo.Mods.Add(cardModificationInfo);
				//Set the target's info to the clone'd info
				target.SetInfo(targetCardInfo);
				target.Anim.PlayTransformAnimation();
				Plugin.Log.LogWarning("firestarter debug " + target + " should have burning");
				yield return new WaitForSeconds(0.1f);
				yield return base.LearnAbility(0.1f);
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			}
			yield break;
		}
	}
}