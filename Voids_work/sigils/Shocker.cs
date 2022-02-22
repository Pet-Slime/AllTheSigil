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
		private NewAbility AddShocker()
		{
			// setup ability
			const string rulebookName = "Zapper";
			const string rulebookDescription = "[creature] will cause what it strikes to gain the Paralysis sigil. The Paralysis sigil is defined as: A card bearing this sigil only attack every other turn.";
			const string LearnDialogue = "Shocking";
			// const string TextureFile = "Artwork/void_weaken.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 2, Plugin.configToxin.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.zapper_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Shocker);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Shocker), tex, abIds);

			// set ability to behaviour class
			void_Shocker.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Shocker : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDealDamage(int amount, PlayableCard target)
		{
			if (target.Dead)
			{
				return false;
			}
			return base.Card.HasAbility(void_Shocker.ability);
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
				CardModificationInfo cardModificationInfo = new CardModificationInfo(void_Paralysis.ability);
				//Clone the main card info so we don't touch the main card set
				CardInfo targetCardInfo = target.Info.Clone() as CardInfo;
				//Add the modifincations to the cloned info
				targetCardInfo.Mods.Add(cardModificationInfo);
				//Set the target's info to the clone'd info
				target.SetInfo(targetCardInfo);
				target.Anim.PlayTransformAnimation();
				yield return new WaitForSeconds(0.1f);
				yield return base.LearnAbility(0.1f);
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			}
			yield break;
		}
	}
}