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
		private void AddDrawJack()
		{
			// setup ability
			const string rulebookName = "Draw Jackalope";
			const string rulebookDescription = "[creature] is played, a Jackalope is created in your hand.";
			const string LearnDialogue = "Pull a Jackalope from a hat why don't ya.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_DrawJack);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = 3;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_DrawJack.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_DrawJack), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_DrawJack : DrawCreatedCard
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