using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Port from Cyn Sigil a day
		private NewAbility AddBonePicker()
		{
			// setup ability
			const string rulebookName = "Bone Picker";
			const string rulebookDescription = "[creature] kills a creature, it will generate 1 Bone.";
			const string LearnDialogue = "My creature's bones, You thief!";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 0);
			info.canStack = true;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.bonepicker_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.ability_bonepicker);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_BonePicker), tex, abIds);

			// set ability to behaviour class
			void_BonePicker.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_BonePicker : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		
		public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
		{
			return fromCombat && base.Card == killer && base.Card.slot.IsPlayerSlot;
		}
		
		public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
		{
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
			yield return new WaitForSeconds(0.1f);
			base.Card.Anim.LightNegationEffect();
			yield return base.PreSuccessfulTriggerSequence();
			yield return Singleton<ResourcesManager>.Instance.AddBones(1, base.Card.Slot);
			yield return new WaitForSeconds(0.1f);
			yield return base.LearnAbility(0.1f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}
	}
}
