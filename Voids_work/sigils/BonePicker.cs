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
		private void AddBonePicker()
		{
			// setup ability
			const string rulebookName = "Bone Picker";
			const string rulebookDescription = "[creature] kills a creature, it will generate 1 Bone.";
			const string LearnDialogue = "My creature's bones, You thief!";
			//const string rulebookNameChinese = "拾骨者";
			const string rulebookDescriptionChinese = "[creature]击杀造物时，会产生1根骨头。";
			const string LearnDialogueChinese = "明明是我的造物的骨头，你不讲武德！";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_BonePicker);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_BonePicker_a2);
			int powerlevel = 1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = true;

			// set ability to behaviour class
			if (Localization.CurrentLanguage == Language.ChineseSimplified)
				void_BonePicker.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescriptionChinese, typeof(void_BonePicker), tex_a1, tex_a2, LearnDialogueChinese,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
			else
				void_BonePicker.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_BonePicker), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
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
