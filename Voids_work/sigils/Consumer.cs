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
		private void AddConsumer()
		{
			// setup ability
			const string rulebookName = "Consumer";
			const string rulebookDescription = "When [creature] kills another creature, it gains 2 health.";
			const string LearnDialogue = "Nothing but bones left in its wake. A truly horrific appetite.";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Consumer);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = 4;
			bool LeshyUsable = Plugin.configConsumer.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Consumer.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Consumer), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
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