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
			const string rulebookDescription = "At the end of the owner's turn, [creature] will move in the direction inscribed in the sigil. If it is able to move, it will gain 1 power and 1 health.";
			const string LearnDialogue = "The trail they leave behind, hurts.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 2, Plugin.configAcidTrail.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.no_a2);

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
		private bool PermanentForRun
		{
			get
			{
				return base.Card.Info.HasTrait(Trait.Bear);
			}
		}

		public override IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
		{
			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.3f);
			bool permanentForRun = this.PermanentForRun;
			if (permanentForRun)
			{
				CardModificationInfo powerMod = base.Card.Info.Mods.Find((CardModificationInfo x) => x.singletonId == "Velocity");
				bool flag = powerMod == null;
				if (flag)
				{
					powerMod = new CardModificationInfo();
					powerMod.singletonId = "hodag";
					RunState.Run.playerDeck.ModifyCard(base.Card.Info, powerMod);
				}
				powerMod.attackAdjustment++;
			}
			else
			{
				CardModificationInfo mod = new CardModificationInfo(1, 0);
				base.Card.AddTemporaryMod(mod);
			}
			bool flag2 = !base.Card.Dead;
			if (flag2)
			{
				base.Card.Anim.LightNegationEffect();
				yield return new WaitForSeconds(0.3f);
				yield return base.LearnAbility(0f);
			}
			yield break;
		}
	}
}