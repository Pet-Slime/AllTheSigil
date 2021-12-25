using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using System.Linq;
using Artwork = voidSigils.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Port from act 3 to match the flavor of act 1
		private NewAbility AddAmbush()
		{
			// setup ability
			const string rulebookName = "Ambush";
			const string rulebookDescription = "[creature] will strike at a card that just got played infront of it, if it can.";
			const string LearnDialogue = "Out of the shadows, they strike";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 3, true);


			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_ambush);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Ambush), tex, abIds);

			// set ability to behaviour class
			void_Ambush.ability = newAbility.ability;

			return newAbility;
		}
	}


	[HarmonyPatch(typeof(AbilityIconInteractable), "LoadIcon")]
	public class AmbushFixTest
	{
		[HarmonyPostfix]
		public static void Postfix(ref Texture __result, ref CardInfo info, ref AbilityInfo ability)
		{
			if (ability.ability == void_Ambush.ability)
			{
				if (info != null)
                {
					Texture2D tex1 = SigilUtils.LoadTextureFromResource(Artwork.void_ambush_1);

					Texture2D tex2 = SigilUtils.LoadTextureFromResource(Artwork.void_ambush_2);

					Texture2D tex3 = SigilUtils.LoadTextureFromResource(Artwork.void_ambush_3);

					List<Ability> baseAbilities = info.Abilities;

					int count = baseAbilities.Where(a => a == void_Ambush.ability).Count();

					if (count == 1)
                    {
						__result = tex1;

					} else if (count == 2)
					{
						__result = tex2;
					}
					else if (count >= 3)
					{
						__result = tex3;
					}

				}
			}
		}
	}




	public class void_Ambush : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override int Priority
		{
			get
			{
				return -1;
			}
		}

		private int NumShots
		{
			get
			{
				return Mathf.Max(base.Card.Info.Abilities.FindAll((Ability x) => x == this.Ability).Count, 1);
			}
		}


		public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
		{
			return this.RespondsToTrigger(otherCard);
		}

		public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
		{
			yield return this.FireAtOpposingSlot(otherCard);
			yield break;
		}

		public override bool RespondsToOtherCardAssignedToSlot(PlayableCard otherCard)
		{
			return this.RespondsToTrigger(otherCard);
		}

		public override IEnumerator OnOtherCardAssignedToSlot(PlayableCard otherCard)
		{
			yield return this.FireAtOpposingSlot(otherCard);
			yield break;
		}

		private bool RespondsToTrigger(PlayableCard otherCard)
		{
			return !base.Card.Dead && !otherCard.Dead && otherCard.Slot == base.Card.Slot.opposingSlot;
		}

		private IEnumerator FireAtOpposingSlot(PlayableCard otherCard)
		{
			if (otherCard != this.lastShotCard || Singleton<TurnManager>.Instance.TurnNumber != this.lastShotTurn)
			{
				this.lastShotCard = otherCard;
				this.lastShotTurn = Singleton<TurnManager>.Instance.TurnNumber;
				Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
				yield return new WaitForSeconds(0.25f);
				int num;
				for (int i = 0; i < this.NumShots; i = num + 1)
				{
					if (otherCard != null && !otherCard.Dead)
					{
						yield return base.PreSuccessfulTriggerSequence();
						base.Card.Anim.LightNegationEffect();
						yield return new WaitForSeconds(0.5f);
						if (base.Card.Anim is CardAnimationController)
						{
							(base.Card.Anim as CardAnimationController).PlayAttackAnimation(false, otherCard.Slot);

						}
						
						yield return otherCard.TakeDamage(1, base.Card);
						yield return new WaitForSeconds(0.5f);
					}
					num = i;
				}
				yield return base.LearnAbility(0.5f);
				Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			}
			yield break;
		}

		private int lastShotTurn = -1;

		private PlayableCard lastShotCard;

	}
}