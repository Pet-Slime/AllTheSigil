using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Sire
		private NewAbility AddStrafePowerUp()
		{
			// setup ability
			const string rulebookName = "Velocity";
			const string rulebookDescription = "[creature] will move in the direction the arrow is pointing during the endphase, and if able to move, will gain one attack and one health.";
			const string LearnDialogue = "The trail they leave behind, hurts.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 2, Plugin.configAcidTrail.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.acidtrail_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Volicity);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_StrafingPower), tex, abIds);

			// set ability to behaviour class
			void_StrafingPower.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_StrafingPower : Strafe
	{
		public override Ability Ability => ability;

		public static Ability ability;

		//Copied Strafe's code and just added damage

		public override IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
		{
			CardModificationInfo cardModificationInfo = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "void_StrafingPower");
			if (cardModificationInfo == null)
			{
				cardModificationInfo = new CardModificationInfo();
				cardModificationInfo.singletonId = "void_StrafingPower";
				base.Card.AddTemporaryMod(cardModificationInfo);
			}
			cardModificationInfo.attackAdjustment++;
			cardModificationInfo.healthAdjustment++;
			base.Card.OnStatsChanged();
			yield return new WaitForSeconds(0.25f);
			yield break;
		}
	}
}