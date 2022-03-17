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
		//Request by Blind
		private void AddWithering()
		{
			// setup ability
			const string rulebookName = "Withering";
			const string rulebookDescription = "[creature] will perish at the end of the opponant's turn.";
			const string LearnDialogue = "Gone like the dust";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Withering);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Withering_a2);
			int powerlevel = -1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Withering.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Withering), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Withering : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToTurnEnd(bool playerTurnEnd)
		{
			return base.Card != null && base.Card.OpponentCard == playerTurnEnd;
		}

		public override IEnumerator OnTurnEnd(bool playerTurnEnd)
		{
			yield return base.PreSuccessfulTriggerSequence();
			bool flag = base.Card != null && !base.Card.Dead;
			if (flag)
			{
				bool flag2 = !SaveManager.SaveFile.IsPart2;
				if (flag2)
				{
					Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
					yield return new WaitForSeconds(0.1f);
				}
				yield return base.Card.Die(false, null, true);
				yield return base.LearnAbility(0.25f);
				bool flag3 = !SaveManager.SaveFile.IsPart2;
				if (flag3)
				{
					yield return new WaitForSeconds(0.1f);
				}
			}
			yield break;
		}
	}
}