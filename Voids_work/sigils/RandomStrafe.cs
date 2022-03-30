﻿using HarmonyLib;
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
		//Original
		private void AddRandomStrafe()
		{
			// setup ability
			const string rulebookName = "Random Strafe";
			const string rulebookDescription = "[creature] is drawn, it will gain a random strafe sigil.";
			const string LearnDialogue = "How will it move?";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_RandomStrafe);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_RandomStrafe_a2);
			int powerlevel = 1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_RandomStrafe.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_RandomStrafe), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_RandomStrafe : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDrawn()
		{
			return true;
		}

		public override IEnumerator OnDrawn()
		{
			(Singleton<PlayerHand>.Instance as PlayerHand3D).MoveCardAboveHand(base.Card);
			yield return base.Card.FlipInHand(new Action(this.AddMod));
			yield return base.LearnAbility(0.5f);
			yield break;
		}

		private void AddMod()
		{
			base.Card.Status.hiddenAbilities.Add(this.Ability);
			CardModificationInfo cardModificationInfo = new CardModificationInfo(this.ChooseAbility());
			CardModificationInfo cardModificationInfo2 = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.HasAbility(this.Ability));
			bool flag = cardModificationInfo2 == null;
			if (flag)
			{
				cardModificationInfo2 = base.Card.Info.Mods.Find((CardModificationInfo x) => x.HasAbility(this.Ability));
			}
			bool flag2 = cardModificationInfo2 != null;
			if (flag2)
			{
				cardModificationInfo.fromTotem = cardModificationInfo2.fromTotem;
				cardModificationInfo.fromCardMerge = cardModificationInfo2.fromCardMerge;
			}
			base.Card.AddTemporaryMod(cardModificationInfo);
		}

		private Ability ChooseAbility()
		{
			List<Ability> learnedAbilities = new List<Ability>();
			learnedAbilities.Add(Ability.Strafe);
			learnedAbilities.Add(Ability.StrafePush);
///			learnedAbilities.Add(Ability.SkeletonStrafe);
			learnedAbilities.Add(Ability.SquirrelStrafe);
			learnedAbilities.Add(Ability.MoveBeside);
			learnedAbilities.Add(void_AcidTrail.ability);
			learnedAbilities.Add(void_Caustic.ability);

			learnedAbilities.RemoveAll((Ability x) => x == Ability.RandomAbility || base.Card.HasAbility(x));
			bool flag = learnedAbilities.Count > 0;
			Ability result;
			if (flag)
			{
				result = learnedAbilities[SeededRandom.Range(0, learnedAbilities.Count, base.GetRandomSeed())];
			}
			else
			{
				result = Ability.Sharp;
			}
			return result;
		}

	}

	

}