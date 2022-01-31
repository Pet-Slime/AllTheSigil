using HarmonyLib;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using Random = UnityEngine.Random;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddAntler()
		{
			// setup ability
			const string rulebookName = "Antler Bearer";
			const string rulebookDescription = "[creature] is killed, gain three random hooved tribe cards.";
			const string LearnDialogue = "The herd sticks together.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 2);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.PatheticSacrificeAct2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_antler);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_antler), tex, abIds);

			// set ability to behaviour class
			void_antler.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_antler : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		protected virtual View DrawCardView
		{
			get
			{
				return View.Default;
			}
		}


		public override bool RespondsToPreDeathAnimation(bool wasSacrifice)
		{
			return true;
		}

		public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
		{

			var cards = ScriptableObjectLoader<CardInfo>.AllData;
			List<CardInfo> targets = new List<CardInfo>();

			for (int index = 0; index < cards.Count; index++)
			{
				if (cards[index] != null && cards[index].tribes.Contains(Tribe.Hooved))
				{
					targets.Add(cards[index]);
				}
			}


			if (Singleton<ViewManager>.Instance.CurrentView != this.DrawCardView)
			{
				yield return new WaitForSeconds(0.2f);
				Singleton<ViewManager>.Instance.SwitchToView(this.DrawCardView, false, false);
				yield return new WaitForSeconds(0.2f);
			}

			var target = targets[Random.Range(0, (targets.Count))];

			yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(target, null, 0.25f, null);
			yield return new WaitForSeconds(0.45f);

			target = targets[Random.Range(0, (targets.Count))];

			yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(target, null, 0.25f, null);
			yield return new WaitForSeconds(0.45f);

			target = targets[Random.Range(0, (targets.Count))];

			yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(target, null, 0.25f, null);
			yield return new WaitForSeconds(0.45f);

			yield return base.LearnAbility(0.1f);
			yield break;
		}
	}
}