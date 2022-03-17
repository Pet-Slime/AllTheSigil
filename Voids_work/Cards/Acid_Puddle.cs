using DiskCardGame;
using InscryptionAPI.Card;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils.Voids_work.Cards
{
	public static class Acid_Puddle
	{
		public static void AddCard()
		{

            // This builds our card information.
            CardInfo AcidPuddle = CardManager.New(
                // Card internal name.
                "void_Acid_Puddle",

                // Card display name.
                "Caustic Puddle",

                // Attack.
                0,

                // Health.
                0,

                // Descryption.
                description: "A puddle that errods all that touches it.",

                // Card ID Prefix
                modPrefix: "void"
            )

            // These are the abilities this card will have.
            // The format for custom abilities is 'CustomAbilityClass.ability'.
            // The format for vanilla abilitites is Ability.Ability'.
            .AddAbilities(Ability.Sharp)

            // These are the special abilities this card will have.
            // These do not show up like other abilities; They are invisible to the player.
            // The format for custom special abilities is 'CustomSpecialAbilityClass.CustomSpecialAbilityID'.
            // The format for vanilla special abilities is SpecialTriggeredAbility.Ability'.
            //.AddSpecialAbilities(NewTestSpecialAbility.TestSpecialAbility, SpecialTriggeredAbility.CardsInHand)

            // CardAppearanceBehaviours are things like card backgrounds.
            // In this case, the card has a Rare background.
            .AddAppearances(CardAppearanceBehaviour.Appearance.TerrainBackground)

            // MetaCategories tell the game where this card should be available as a rewward or for other purposes.
            // In this case, CardMetaCategory.Rare tells the game to put this card in the post-boss reward event.

            // The first image is the card portrait.
            // The second image is the emissive portrait.
            .SetPortrait(SigilUtils.LoadTextureFromResource(Artwork.void_Caustic_Puddle), SigilUtils.LoadTextureFromResource(Artwork.void_Caustic_Puddle_e))
            .SetPixelPortrait(SigilUtils.LoadTextureFromResource(Artwork.void_Caustic_Puddle_p))

            ;

            // Pass the card to the API.
            CardManager.Add(AcidPuddle);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------------------

    }
}

