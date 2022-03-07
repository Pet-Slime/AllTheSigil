using UnityEngine;
using DiskCardGame;
using System.Collections.Generic;
using System.Collections;
using APIPlugin;
using Artwork = voidSigils.Voids_work.Resources.Resources;


namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddDrawBlood()
		{
			// setup ability
			const string rulebookName = "Draw Blood";
			const string rulebookDescription = "[creature] is played, a card costing blood is created in your hand.";
			const string LearnDialogue = "What will it release on death?";
			// const string TextureFile = "Artwork/void_pathetic.png";


			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.no_a2);
			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_drawblood);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(ability_drawblood), tex, abIds);

			// set ability to behaviour class
			ability_drawblood.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class ability_drawblood : DrawCreatedCard
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override CardInfo CardToDraw
		{
			get
			{
				var creatureWithinId = GetRandomChoosableCardWithCost(SaveManager.SaveFile.GetCurrentRandomSeed());

				return CardLoader.GetCardByName(creatureWithinId.name);
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

		public static CardInfo GetRandomChoosableCardWithCost(int randomSeed)
		{
			List<CardInfo> list = CardLoader.GetUnlockedCards(CardMetaCategory.ChoiceNode, CardTemple.Nature).FindAll((CardInfo x) => x.BloodCost > 0);
			bool flag1 = SaveManager.SaveFile.IsPart2;
			if (flag1)
			{
				list.Clear();
				list = CardLoader.GetUnlockedCards(CardMetaCategory.GBCPack, CardTemple.Nature).FindAll((CardInfo x) => x.BloodCost > 0);
			}
			bool flag = list.Count == 0;
			CardInfo result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = CardLoader.Clone(list[SeededRandom.Range(0, list.Count, randomSeed)]);
			}
			return result;
		}

	}
}