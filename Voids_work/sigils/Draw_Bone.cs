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
		private void AddDrawBone()
		{
			// setup ability
			const string rulebookName = "Draw Bone";
			const string rulebookDescription = "When [creature] is played, a card costing bone is created in your hand.";
			const string LearnDialogue = "What will it release on death?";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_DrawBone);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_DrawBone_a2);
			int powerlevel = 3;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_DrawBone.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_DrawBone), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_DrawBone : DrawCreatedCard
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override CardInfo CardToDraw
		{
			get
			{
				var creatureWithinId = GetRandomChoosableCardWithCost(base.GetRandomSeed());

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
			List<CardInfo> list = CardLoader.GetUnlockedCards(CardMetaCategory.ChoiceNode, CardTemple.Nature).FindAll((CardInfo x) => x.bonesCost > 0);
			bool flag1 = SaveManager.SaveFile.IsPart2;
			if (flag1)
			{
				list.Clear();
				list = CardLoader.GetUnlockedCards(CardMetaCategory.GBCPack, CardTemple.Undead).FindAll((CardInfo x) => x.bonesCost > 0);
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