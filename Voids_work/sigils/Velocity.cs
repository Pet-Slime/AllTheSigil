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
		private void AddStrafePowerUp()
		{
			// setup ability
			const string rulebookName = "Velocity";
			const string rulebookDescription = "At the end of the owner's turn, [creature] will move in the direction inscribed in the sigil. If it is able to move, it will gain 1 power and 1 health.";
			const string LearnDialogue = "The trail they leave behind, hurts.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Volicity);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = 1;
			bool LeshyUsable = Plugin.configAcidTrail.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Volicity.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Volicity), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Volicity : Strafe
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
					powerMod.singletonId = "Velocity";
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