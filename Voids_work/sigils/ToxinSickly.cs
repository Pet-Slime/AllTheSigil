using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Turk
		private void AddToxinSickly()
		{
			// setup ability
			const string rulebookName = "Toxin (Sickening)";
			const string rulebookDescription = "When [creature] damages another creature, that creature gains the Sickness Sigil. The Sickness Sigil is defined as: When ever a creature bearing this sigil declares an attack, they will loose one attack.";
			const string LearnDialogue = "Even once combat is over, it leaves sickness spreads in what it fights.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Toxin_Sickness);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Toxin_Sickness_a2);
			int powerlevel = 2;
			bool LeshyUsable = Plugin.configToxin.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Toxin_Sickness.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Toxin_Sickness), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Toxin_Sickness : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDealDamage(int amount, PlayableCard target)
		{
			if (target.Dead)
            {
				return false;
            }
			return base.Card.HasAbility(void_Toxin_Sickness.ability);
		}

		public override IEnumerator OnDealDamage(int amount, PlayableCard target)
		{
			if (target != null)
            {
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
				yield return new WaitForSeconds(0.1f);
				base.Card.Anim.LightNegationEffect();
				yield return base.PreSuccessfulTriggerSequence();
				//make the card mondification info
				CardModificationInfo cardModificationInfo = new CardModificationInfo(void_Sickness.ability);
				//Clone the main card info so we don't touch the main card set
				CardInfo targetCardInfo = target.Info.Clone() as CardInfo;
				//Add the modifincations to the cloned info
				targetCardInfo.Mods.Add(cardModificationInfo);
				//Set the target's info to the clone'd info
				target.SetInfo(targetCardInfo);
				target.Anim.PlayTransformAnimation();
				Plugin.Log.LogWarning("toxin debug " + target + " should have sickness");
				yield return new WaitForSeconds(0.1f);
				yield return base.LearnAbility(0.1f);
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;

			}
			yield break;
		}
	}
}