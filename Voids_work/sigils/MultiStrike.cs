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
		private void AddDoubleAttack()
		{
			// setup ability
			const string rulebookName = "Multi-Strike";
			const string rulebookDescription = "[creature] will strike a card multiple times, if it lives through the first attack. Will not trigger -on attack- or -on damage- effects with the extra strikes.";
            const string LearnDialogue = "So fast, so many strikes";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_double_attack);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_doubleattack_a2);
            int powerlevel = 0;
            bool LeshyUsable = Plugin.configMultiStrike.Value;
            bool part1Shops = true;
            bool canStack = true;

            // set ability to behaviour class
            void_DoubleAttack.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_DoubleAttack), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
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
                if (info != null && !SaveManager.SaveFile.IsPart2)
                {
                    //Get count of how many instances of the ability the card has
                    int count = Mathf.Max(info.Abilities.FindAll((Ability x) => x == void_DoubleAttack.ability).Count, 1);
                    //Switch statement to the right texture
                    switch (count)
                    {
                        case 1:
                            __result = SigilUtils.LoadTextureFromResource(Artwork.void_double_attack_1);
                            break;
                        case 2:
                            __result = SigilUtils.LoadTextureFromResource(Artwork.void_double_attack_2);
                            break;
                        case 3:
                            __result = SigilUtils.LoadTextureFromResource(Artwork.void_double_attack_3);
                            break;
                        case 4:
                            __result = SigilUtils.LoadTextureFromResource(Artwork.void_double_attack_4);
                            break;
                        case 5:
                            __result = SigilUtils.LoadTextureFromResource(Artwork.void_double_attack_5);
                            break;
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
            int count = SigilUtils.getAbilityCount(base.Card, void_DoubleAttack.ability);

            yield return base.PreSuccessfulTriggerSequence();
            for (int index = 0; index < count; index++)
            {
                yield return new WaitForSeconds(0.35f);
                if (base.Card.Anim is CardAnimationController)
                {
                    if (theSlot.Card != null)
                    {
                        PlayableCard theTarget = theSlot.Card;
                        bool impactFrameReached = false;
                        base.Card.Anim.PlayAttackAnimation(false, theSlot, delegate ()
                        {
                            impactFrameReached = true;
                        });
                        yield return new WaitUntil(() => impactFrameReached);
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