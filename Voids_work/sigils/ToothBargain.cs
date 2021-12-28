using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System;
using Pixelplacement;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by blind
		private NewAbility AddToothpicker()
		{
			// setup ability
			const string rulebookName = "Tooth Bargain";
			const string rulebookDescription = "[creature] will put a tooth on your opponant's side of the scale when played, but will put two on yours when it dies.";
			const string LearnDialogue = "A deal with a devil I see...";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 0);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_tooth);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Toothpicker), tex, abIds);

			// set ability to behaviour class
			void_Toothpicker.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Toothpicker : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToResolveOnBoard()
		{
			return true;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			yield return base.PreSuccessfulTriggerSequence();

			yield return new WaitForSeconds(0.25f);
			yield return Singleton<LifeManager>.Instance.ShowDamageSequence(1, 1, false, 0.25f, ResourceBank.Get<GameObject>("Prefabs/Environment/ScaleWeights/Weight_RealTooth"), 0f);
			yield return new WaitForSeconds(0.25f);
			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.25f);
			yield break;
		}

		public override bool RespondsToPreDeathAnimation(bool wasSacrifice)
		{
			return base.Card.OnBoard;
		}

		public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
		{
			base.Card.Anim.LightNegationEffect();
			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.25f);
			yield return Singleton<LifeManager>.Instance.ShowDamageSequence(2, 2, true, 0.25f, null, 0f);
			yield return new WaitForSeconds(0.25f);
			yield return base.LearnAbility(0.25f);
			yield return new WaitForSeconds(0.25f);
			yield break;
		}

	}
}