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
		private NewAbility AddTransient()
		{
			// setup ability
			const string rulebookName = "Transient";
			const string rulebookDescription = "At the end of the owner's turn, [creature] will return to your hand.";
			const string LearnDialogue = "The creature blinks back into the owner's hand at the end of their turn.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -1);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.ability_transient_a2);
			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.ability_transient);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(void_Transient), tex, abIds);

            // set ability to behaviour class
            void_Transient.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Transient : DrawCreatedCard
	{

		public override Ability Ability
		{
			get
			{
				return ability;
			}
		}

		public static Ability ability;

		private void Start()
		{
			this.copy = CardLoader.Clone(base.Card.Info);
		}

		public override CardInfo CardToDraw
		{
			get
			{
				return CardLoader.Clone(this.copy);
			}
		}

		public override bool RespondsToTurnEnd(bool playerTurnEnd)
		{
			return playerTurnEnd;
		}

		public override IEnumerator OnTurnEnd(bool playerTurnEnd)
		{
			yield return base.PreSuccessfulTriggerSequence();
			yield return base.CreateDrawnCard();
			base.Card.Anim.PlayDeathAnimation(false);
			base.Card.UnassignFromSlot();
			base.Card.StartCoroutine(base.Card.DestroyWhenStackIsClear());
			base.Card.Slot = null;
			yield break;
		}

		private CardInfo copy;
	}
}