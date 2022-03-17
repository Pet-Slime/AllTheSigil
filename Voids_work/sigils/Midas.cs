﻿using System.Collections;
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
		//Request by blind
		private void AddMidas()
		{
			// setup ability
			const string rulebookName = "Midas";
			const string rulebookDescription = "[creature] kills a creature, it will generate 1 Foil for each instance of Midas the card has.";
			const string LearnDialogue = "A bounty, paid in full.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Midas);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Midas_a2);
			int powerlevel = 2;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = true;

			// set ability to behaviour class
			void_Midas.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Midas), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;

		}
	}

	public class void_Midas : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
		{
			return fromCombat == true && base.Card.OnBoard && killer.HasAbility(void_Midas.ability);
		}

		public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
		{
			if (fromCombat == true && killer.HasAbility(void_Midas.ability))
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
			}
			yield break;
		}
	}
}