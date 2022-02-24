using HarmonyLib;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Original
		private NewAbility AddBloodGrowth()
		{
			// setup ability
			const string rulebookName = "Blood Growth";
			const string rulebookDescription = "When [creature] attacks, the amount of blood it is counted as when sacrificed will increase.";
			const string LearnDialogue = "There is power in the blood.";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 0);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.no_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_bloodgrowth);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_BloodGrowth), tex, abIds);

			// set ability to behaviour class
			void_BloodGrowth.ability = newAbility.ability;

			return newAbility;
		}
	}

	public class void_BloodGrowth : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public int attacks = 0;

		private CardModificationInfo mod;



		private void Start()
		{
			this.mod = new CardModificationInfo();
		}

		public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			return attacker == base.Card;
		}

		public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
		{
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
			yield return new WaitForSeconds(0.05f);
			yield return base.PreSuccessfulTriggerSequence();
			base.Card.Status.hiddenAbilities.Add(this.Ability);

			attacks += 1;

			switch (attacks)
            {
				case 1:
					base.Card.RemoveTemporaryMod(this.mod);
					Ability Ab1 = Custom_Sigils.Bi_Blood.ability;					
					this.mod.abilities.Clear();
					this.mod.abilities.Add(Ab1);
					base.Card.AddTemporaryMod(this.mod);
					break;
				case 2:
					base.Card.RemoveTemporaryMod(this.mod);	
					Ability Ab2 = Ability.TripleBlood;
					this.mod.abilities.Clear();
					this.mod.abilities.Add(Ab2);
					base.Card.AddTemporaryMod(this.mod);
					break;
				case 3:
					base.Card.RemoveTemporaryMod(this.mod);
					Ability Ab3 = Custom_Sigils.Quadra_Blood.ability;
					this.mod.abilities.Clear();
					this.mod.abilities.Add(Ab3);
					base.Card.AddTemporaryMod(this.mod);
					break;
			}


			yield return new WaitForSeconds(0.05f);
			yield return base.LearnAbility(0f);
			yield break;
		}

	}
}