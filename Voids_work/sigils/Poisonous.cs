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
		private void AddPoisonous()
		{
			// setup ability
			const string rulebookName = "Poisonous";
			const string rulebookDescription = "When [creature] perishes, the creature that killed it perishes as well.";
			const string LearnDialogue = "Attacking something poisonous, isn't that smart.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Poisonous);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Poisonous_a2);
			int powerlevel = 2;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Poisonous.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Poisonous), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
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
