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
		private NewAbility AddThickShell()
		{
			// setup ability
			const string rulebookName = "Thick Shell";
			const string rulebookDescription = "When attacked, [creature] takes one less damage.";
			const string LearnDialogue = "The thick shell on that creature protected it from one damage!";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 2, Plugin.configThickShell.Value);
			info.canStack = false;

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.ability_thickshell);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);

			NewAbility newAbility = new NewAbility(info, typeof(void_ThickShell), tex, abIds);

			// set ability to behaviour class
			void_ThickShell.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_ThickShell : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

        private void Start()
        {
            int health = base.Card.Info.Health;
            this.mod = new CardModificationInfo();
            this.mod.nonCopyable = true;
            this.mod.singletonId = "ShellHP";
            this.mod.healthAdjustment = 0;
            base.Card.AddTemporaryMod(this.mod);
        }

        public override bool RespondsToCardGettingAttacked(PlayableCard source)
        {
            return source == base.Card;
        }

        public override bool RespondsToAttackEnded()
        {
            return this.attacked;
        }

        public override IEnumerator OnCardGettingAttacked(PlayableCard source)
        {
            this.attacked = true;
            yield return base.PreSuccessfulTriggerSequence();
            this.mod.healthAdjustment = 1;
            yield break;
        }

        public override IEnumerator OnAttackEnded()
        {
            this.attacked = false;
            yield return new WaitForSeconds(0.1f);
            this.mod.healthAdjustment = 0;
            base.Card.HealDamage(1);
            base.Card.Anim.LightNegationEffect();
            yield return new WaitForSeconds(0.1f);
            yield return base.LearnAbility(0.25f);
            yield break;
        }

        private bool attacked;
        private CardModificationInfo mod;
    }
}
