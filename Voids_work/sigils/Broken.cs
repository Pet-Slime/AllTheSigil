using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Port from act 3 to match act 1 better
		private void AddBroken()
		{
			// setup ability
			const string rulebookName = "Broken";
			const string rulebookDescription = "[creature] is permanently removed from your deck if it dies.";
			const string LearnDialogue = "None of us can escape our age";
			//const string rulebookNameChinese = "老化";
			const string rulebookDescriptionChinese = "[creature]阵亡时，它将从你的牌组中永久移除。";
			const string LearnDialogueChinese = "没人能逃过时间。";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Broken);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = -2;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			if (Localization.CurrentLanguage == Language.ChineseSimplified)
				void_Broken.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescriptionChinese, typeof(void_Broken), tex_a1, tex_a2, LearnDialogueChinese,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
			else
				void_Broken.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Broken), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Broken : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
		{
			return true;
		}

		public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
		{
			DeckInfo currentDeck = SaveManager.SaveFile.CurrentDeck;
			CardInfo card = currentDeck.Cards.Find((CardInfo x) => x.HasAbility(void_Broken.ability) && x.name == base.Card.Info.name);
			if (card != null)
			{
				currentDeck.RemoveCard(card);
			}
			if (!base.HasLearned)
			{
				CustomCoroutine.WaitThenExecute(2f, delegate
				{
					Singleton<VideoCameraRig>.Instance.PlayCameraAnim("refocus_medium");
					Singleton<VideoCameraRig>.Instance.VOPlayer.PlayVoiceOver("God damn it.", "vo_goddamnit");
				}, false);
			}
			yield return base.LearnAbility(0.5f);
			yield break;
		}
		public override bool LearnAbilityConditionsMet()
		{
			return true;
		}
	}
}