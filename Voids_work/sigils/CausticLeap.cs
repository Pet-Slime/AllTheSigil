using HarmonyLib;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Tilted hat
		private void AddCaustic()
		{
			// setup ability
			const string rulebookName = "Caustic";
			const string rulebookDescription = "At the end of the towner's turn, [creature] will move in the direction inscribed in the sigil, and drop an acid puddle in their old space.";
			const string LearnDialogue = "What it leaves behind is deadly.";
			//const string rulebookNameChinese = "漏酸冲刺";
			const string rulebookDescriptionChinese = "持牌人回合结束时，[creature]将向印记标注的方向移动，并在原地留下一张Caustic Puddle牌。[define:void_Acid_Puddle]";
			const string LearnDialogueChinese = "这个造物冲刺时会留下酸液。";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Caustic);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Caustic_a2);
			int powerlevel = 2;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;



			// set ability to behaviour class
			if (Localization.CurrentLanguage == Language.ChineseSimplified)
				void_Caustic.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescriptionChinese, typeof(void_Caustic), tex_a1, tex_a2, LearnDialogueChinese,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
			else
				void_Caustic.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Caustic), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Caustic : Strafe
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override IEnumerator PostSuccessfulMoveSequence(CardSlot cardSlot)
		{
			bool flag = cardSlot.Card == null;
			if (flag)
			{
				yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName("void_Acid_Puddle"), cardSlot, 0.1f, true);
			}
			yield break;
		}
	}
}