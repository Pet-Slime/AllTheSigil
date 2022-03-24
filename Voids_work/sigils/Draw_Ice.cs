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
		private void AddDrawIce()
		{
			// setup ability
			const string rulebookName = "Draw Card";
			const string rulebookDescription = "When [creature] is played, a card relating to it's ice cube parameter (default Opossum) is created in your hand.";
			const string LearnDialogue = "What will it release on death?";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_DrawIce);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_DrawIce_a2);
			int powerlevel = 3;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_DrawIce.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_DrawIce), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_DrawIce : DrawCreatedCard
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override CardInfo CardToDraw
		{
			get
			{
				string creatureWithinId = "Opossum";
				bool flag = base.Card.Info.iceCubeParams != null && base.Card.Info.iceCubeParams.creatureWithin != null;
				if (flag)
				{
					creatureWithinId = base.Card.Info.iceCubeParams.creatureWithin.name;
				}
				return CardLoader.GetCardByName(creatureWithinId);
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