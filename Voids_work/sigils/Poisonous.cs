using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Port from Cyn Sigil a day
		private NewAbility AddPoisonous()
		{
			// setup ability
			const string rulebookName = "Poisonous";
			const string rulebookDescription = "When [creature] perishes, the creature that killed it perishes as well.";
			const string LearnDialogue = "Attacking something poisonous, isn't that smart.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 2);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.ability_poisonous);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(void_Poisonous), tex, abIds);

			// set ability to behaviour class
			void_Poisonous.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Poisonous : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


		public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
		{
			return !wasSacrifice && base.Card.OnBoard;
		}

		public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
		{
			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.25f);
			if (killer != null)
			{
				yield return killer.Die(false, base.Card, true);
				if (Singleton<BoardManager>.Instance is BoardManager3D)
				{
					yield return new WaitForSeconds(0.5f);
					yield return base.LearnAbility(0.5f);
				}
			}
			yield break;
		}
	}
}
