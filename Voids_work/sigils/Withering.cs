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
		private NewAbility AddWithering()
		{
			// setup ability
			const string rulebookName = "Withering";
			const string rulebookDescription = "[creature] will perish at the end of the opponant's turn.";
			const string LearnDialogue = "Gone like the dust";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, -1);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.void_Withering_a2);
			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Withering);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(voidWithering), tex, abIds);

			// set ability to behaviour class
			voidWithering.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class voidWithering : AbilityBehaviour
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