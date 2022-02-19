﻿using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using GBC;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by blind
		private NewAbility AddAbundance()
		{
			// setup ability
			const string rulebookName = "Abundance";
			const string rulebookDescription = "[creature] will grant one tooth per instance of Abundance when killed.";
			const string LearnDialogue = "Gooooooooldddddd! *cough* sorry about that. Couldn't resist.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3, false);
			info.canStack = true;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.abundance_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Abundance);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Abundance), tex, abIds);
			

			// set ability to behaviour class
			void_Abundance.ability = newAbility.ability;

			return newAbility;

		}
	}

	public class void_Abundance : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
		{
			return base.Card.HasAbility(void_Abundance.ability);
		}

		public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
		{

			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.15f);

			bool flag2 = !SaveManager.SaveFile.IsPart2;
			if (flag2)
			{
				if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("extraVoid.inscryption.LifeCost"))
				{
					Singleton<ViewManager>.Instance.SwitchToView(View.Scales, false, true);
					yield return new WaitForSeconds(0.25f); RunState.Run.currency += (1);
					yield return Singleton<CurrencyBowl>.Instance.DropWeightsIn(1);
					yield return new WaitForSeconds(0.75f);
					Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
				}
				else
				{
					Singleton<ViewManager>.Instance.SwitchToView(View.Scales, false, true);
					yield return new WaitForSeconds(0.25f); RunState.Run.currency += (1);
					yield return Singleton<CurrencyBowl>.Instance.ShowGain(1, true, false);
					yield return new WaitForSeconds(0.25f);
					Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
				}
			}
			else
			{
				SaveData.Data.currency += 1;
			}

			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.1f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}
	}
}