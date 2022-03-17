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
		private void AddZapper()
		{
			// setup ability
			const string rulebookName = "Zapper";
			const string rulebookDescription = "When [creature] damages another creature, that creature will gain the Paralysis Sigil. The Paralysis sigil is defined as: A card bearing this sigil only attack every other turn.";
			const string LearnDialogue = "Shocking";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Zapper);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Zapper_a2);
			int powerlevel = 1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Zapper.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Zapper), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability; ;
		}
	}

	public class void_Zapper : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDealDamage(int amount, PlayableCard target)
		{
			if (target.Dead)
			{
				return false;
			}
			return base.Card.HasAbility(void_Zapper.ability);
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