using UnityEngine;
using DiskCardGame;
using HarmonyLib;
using System.Collections;
using APIPlugin;
using Artwork = voidSigils.Voids_work.Resources.Resources;


namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Sire
		private NewAbility AddDrawJack()
		{
			// setup ability
			const string rulebookName = "Draw Jackalope";
			const string rulebookDescription = "[creature] will grant a Jackalope card when played.";
			const string LearnDialogue = "Pull a Jackalope from a hat why don't ya.";
			// const string TextureFile = "Artwork/void_pathetic.png";


			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_drawjack);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(ability_drawjack), tex, abIds);

			// set ability to behaviour class
			ability_drawjack.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class ability_drawjack : DrawCreatedCard
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override CardInfo CardToDraw
		{
			get
			{
				return CardLoader.GetCardByName("void_Jackalope");
			}
		}

		public override bool RespondsToResolveOnBoard()
		{
			return true;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			yield return base.PreSuccessfulTriggerSequence();
			bool flag = Singleton<ViewManager>.Instance.CurrentView != this.DrawCardView;
			if (flag)
			{
				yield return new WaitForSeconds(0.2f);
				Singleton<ViewManager>.Instance.SwitchToView(this.DrawCardView, false, false);
				yield return new WaitForSeconds(0.2f);
			}
			yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(this.CardToDraw, base.Card.TemporaryMods, 0.25f, null);
			yield return new WaitForSeconds(0.45f);
			yield return base.LearnAbility(0.1f);
			yield break;
		}

	}
}