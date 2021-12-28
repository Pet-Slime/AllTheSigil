using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using HarmonyLib;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System.Linq;

namespace voidSigils
{
	public partial class Plugin
	{
        //Port from Zerg mod by James
		private NewAbility AddDoubleAttack()
		{
			// setup ability
			const string rulebookName = "Multi-Strike";
			const string rulebookDescription = "[creature] will strike a card multiple times, if it lives through the first attack. Will not trigger -on attack- or -on damage- effects with the extra strikes.";
            const string LearnDialogue = "So fast, so many strikes";
            // const string TextureFile = "Artwork/void_double_attack.png";

            AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 0, true);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_double_attack);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_DoubleAttack), tex, abIds);

			// set ability to behaviour class
			void_DoubleAttack.ability = newAbility.ability;

			return newAbility;
		}
	}

    [HarmonyPatch(typeof(AbilityIconInteractable), "LoadIcon")]
    public class MultiStrikePatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Texture __result, ref CardInfo info, ref AbilityInfo ability)
        {
            if (ability.ability == void_DoubleAttack.ability)
            {
                if (info != null)
                {
                    Texture2D tex1 = SigilUtils.LoadTextureFromResource(Artwork.void_double_attack_1);

                    Texture2D tex2 = SigilUtils.LoadTextureFromResource(Artwork.void_double_attack_2);

                    Texture2D tex3 = SigilUtils.LoadTextureFromResource(Artwork.void_double_attack_3);

                    List<Ability> baseAbilities = info.Abilities;

                    int count = baseAbilities.Where(a => a == void_DoubleAttack.ability).Count();

                    if (count == 1)
                    {
                        __result = tex1;

                    }
                    else if (count == 2)
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

    public class void_DoubleAttack : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;


        public override bool RespondsToDealDamage(int amount, PlayableCard target)
        {
            if (Card.Dead)
            {
                return false;
            }

            if (target.Dead)
            {
                // Hit a card if it's replaced the slot (Anglers Fish Bucket)
                CardSlot cardSlot = SigilUtils.GetSlot(target);
                if (cardSlot.Card == null || cardSlot.Card.Dead)
                {
                    return false;
                }
            }

            return true;
        }

        public override IEnumerator OnDealDamage(int amount, PlayableCard target)
        {

            CardSlot theSlot = SigilUtils.GetSlot(target);
            List<Ability> baseAbilities = base.Card.Info.Abilities;
            int count = baseAbilities.Where(a => a == void_DoubleAttack.ability).Count();

            Plugin.Log.LogMessage("multi-strike count: " + count);
            yield return base.PreSuccessfulTriggerSequence();
            for (int index = 0; index < count; index++)
            {
                yield return new WaitForSeconds(0.35f);
                if (base.Card.Anim is CardAnimationController)
                {
                    
                    if (theSlot.Card != null)
                    {
                        (base.Card.Anim as CardAnimationController).PlayAttackAnimation(false, theSlot);
                        yield return new WaitForSeconds(0.1f);
                        PlayableCard theTarget = theSlot.Card;
                        yield return theTarget.TakeDamage(base.Card.Info.Attack, null);
                    }

                }
                if (target.Dead)
                {
                    break;
                }
            }
            yield return new WaitForSeconds(0.25f);
            yield return base.LearnAbility(0.0f);
        }

    }
}