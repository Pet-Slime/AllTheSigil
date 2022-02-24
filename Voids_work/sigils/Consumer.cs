using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Tilted hat
		private NewAbility AddConsumer()
		{
			// setup ability
			const string rulebookName = "Consumer";
			const string rulebookDescription = "When [creature] kills another creature, it gains 2 health.";
			const string LearnDialogue = "Nothing but bones left in its wake. A truly horrific appetite.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 4, Plugin.configConsumer.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.no_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_consumer);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Consumer), tex, abIds);

			// set ability to behaviour class
			void_Consumer.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_Consumer : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private void Start()
		{
			int health = base.Card.Info.Health;
			this.mod = new CardModificationInfo();
			this.mod.nonCopyable = true;
			this.mod.singletonId = "increaseHP";
			this.mod.healthAdjustment = 0;
			base.Card.AddTemporaryMod(this.mod);
		}

		public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
		{
			return base.Card == killer;
		}

		public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
		{
			yield return base.PreSuccessfulTriggerSequence();
			this.mod.healthAdjustment += 2;
			base.Card.OnStatsChanged();
			base.Card.Anim.StrongNegationEffect();
			yield return new WaitForSeconds(0.25f);
			yield return base.LearnAbility(0.25f);
			yield break;
		}

		private CardModificationInfo mod;

	}
}