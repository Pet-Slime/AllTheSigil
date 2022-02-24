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
		private NewAbility AddFireStarter()
		{
			// setup ability
			const string rulebookName = "Firestarter";
			const string rulebookDescription = "When [creature] damages another creature, that creature will gain the Burning Sigil. The Burning Sigil is define as: Each upkeep, this creature gains 1 strength but looses 1 health.";
			const string LearnDialogue = "Even once combat is over, it leaves a deadly mark";
			// const string TextureFile = "Artwork/void_weaken.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 2, Plugin.configToxin.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.void_Firestarter_a2);
			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Firestarter);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Firestarter), tex, abIds);

			// set ability to behaviour class
			void_Firestarter.ability = newAbility.ability;

			return newAbility;
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