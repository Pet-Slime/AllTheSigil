using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private void AddAcidTrail()
		{
			// setup ability
			const string rulebookName = "Acidic Trail";
			const string rulebookDescription = "At the end of the owner's turn, [creature] will move in the direction inscribed in the sigil, and deal 1 damage to the opposing creature if it is able to move.";
			const string LearnDialogue = "The trail they leave behind, hurts.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_AcidTrail);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_AcidTrail_a2);
			int powerlevel = 2;
			bool LeshyUsable = Plugin.configAcidTrail.Value;
			bool part1Shops = true;
			bool canStack = false;



			// set ability to behaviour class
			void_AcidTrail.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_AcidTrail), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_AcidTrail : Strafe
	{
		public override Ability Ability => ability;

		public static Ability ability;

		//Copied Strafe's code and just added damage

		public override IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
		{
			if (oldSlot.opposingSlot.Card != null)
            {
				bool impactFrameReached = false;
				base.Card.Anim.PlayAttackAnimation(false, oldSlot.opposingSlot, delegate ()
				{
					impactFrameReached = true;
				});
				yield return new WaitUntil(() => impactFrameReached);
				yield return oldSlot.opposingSlot.Card.TakeDamage(1, base.Card);
				yield return new WaitForSeconds(0.25f);
			}
			yield break;
		}
	}
}