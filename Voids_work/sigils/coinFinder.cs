using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using HarmonyLib;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using GBC;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility addCoinFinder()
		{
			// setup ability
			const string rulebookName = "Coin Finder";
			const string rulebookDescription = "[creature] will grant one currency during the owner's endphase.";
			const string LearnDialogue = "A tooth for your thoughts?";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 2, false);
			info.canStack = true;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.coinFinder_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_coinFinder);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_coinFinder), tex, abIds);

			// set ability to behaviour class
			void_coinFinder.ability = newAbility.ability;

			return newAbility;

		}
	}

	public class void_coinFinder : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToTurnEnd(bool playerTurnEnd)
		{
			return base.Card != null && base.Card.OpponentCard != playerTurnEnd;
		}

		public override IEnumerator OnTurnEnd(bool playerTurnEnd)
		{

			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.15f);
			bool flag1 = !SaveManager.SaveFile.IsPart2;
			if (flag1)
			{
				if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("extraVoid.inscryption.LifeCost"))
				{
					Singleton<ViewManager>.Instance.SwitchToView(View.Scales, false, true);
					yield return new WaitForSeconds(0.25f); RunState.Run.currency += (1);
					yield return Singleton<CurrencyBowl>.Instance.DropWeightsIn(1);
					yield return new WaitForSeconds(0.75f);
					Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, true);
					Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;


				} else
                {
					Singleton<ViewManager>.Instance.SwitchToView(View.Scales, false, true);
					yield return new WaitForSeconds(0.25f); RunState.Run.currency += (1);
					yield return Singleton<CurrencyBowl>.Instance.ShowGain(1, true, false);
					yield return new WaitForSeconds(0.25f);
					Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, true);
					Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
				}
			}
			else
			{
				SaveData.Data.currency += 1;
				base.Card.Anim.LightNegationEffect();
			}
			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.1f);
			yield break;
		}
	}
}