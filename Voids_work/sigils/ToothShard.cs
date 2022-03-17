using HarmonyLib;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using System;
using System.Collections;
using GBC;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private void AddToothShard()
		{
			// setup ability
			const string rulebookName = "Tooth Shard";
			const string rulebookDescription = "[creature] will generate 1 foil when hit, if it lives through the attack.";
			const string LearnDialogue = "A splinter of gold.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_ToothShard);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_ToothShard_a2);
			int powerlevel = 1;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_ToothShard.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_ToothShard), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_ToothShard : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToTakeDamage(PlayableCard source)
		{
			return source != null && source.Health > 0;
		}

		public override IEnumerator OnTakeDamage(PlayableCard source)
		{
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
			yield return new WaitForSeconds(0.1f);
			base.Card.Anim.LightNegationEffect();
			yield return base.PreSuccessfulTriggerSequence();
			bool flag2 = !SaveManager.SaveFile.IsPart2;
			if (flag2)
			{
				if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("extraVoid.inscryption.LifeCost"))
				{
					Singleton<ViewManager>.Instance.SwitchToView(View.Scales, false, true);
					yield return new WaitForSeconds(0.25f); RunState.Run.currency += (1);
					yield return Singleton<CurrencyBowl>.Instance.DropWeightsIn(1);
					yield return new WaitForSeconds(0.75f);
				}
				else
				{
					Singleton<ViewManager>.Instance.SwitchToView(View.Scales, false, true);
					yield return new WaitForSeconds(0.25f); RunState.Run.currency += (1);
					yield return Singleton<CurrencyBowl>.Instance.ShowGain(1, true, false);
					yield return new WaitForSeconds(0.25f);
				}
			}
			else
			{
				SaveData.Data.currency += 1;
				base.Card.Anim.StrongNegationEffect();
				base.Card.Anim.StrongNegationEffect();
			}
			yield return new WaitForSeconds(0.1f);
			yield return base.LearnAbility(0.1f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}
	}
}