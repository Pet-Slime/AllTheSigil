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
		private NewAbility AddDrawIce()
		{
			// setup ability
			const string rulebookName = "Draw Card";
			const string rulebookDescription = "[creature] is played, a card relating to it's ice cube parameter (default Opossum) is created in your hand.";
			const string LearnDialogue = "What will it release on death?";
			// const string TextureFile = "Artwork/void_pathetic.png";


			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.no_a2);
			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_drawjack);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(ability_drawice), tex, abIds);

			// set ability to behaviour class
			ability_drawice.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class ability_drawice : DrawCreatedCard
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