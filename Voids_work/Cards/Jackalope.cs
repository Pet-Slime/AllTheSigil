using DiskCardGame;
using InscryptionAPI.Card;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils.Voids_work.Cards
{
	public static class Jackalope
	{
		public static void AddCard()
		{


			// This builds our card information.
			CardInfo Jackalope = CardManager.New(
				// Card internal name.
				"void_Jackalope",

				// Card display name.
				"Jackalope",

				// Attack.
				2,

				// Health.
				2,

				// Descryption.
				description: "A Jackalope",

				// Card ID Prefix
				modPrefix: "void"
			)

			// These are the abilities this card will have.
			// The format for custom abilities is 'CustomAbilityClass.ability'.
			// The format for vanilla abilitites is Ability.Ability'.
			.AddAbilities(Ability.Strafe, void_DrawJack.ability)

			.AddTribes(Tribe.Hooved)

			.SetCost(bloodCost: 2)

			// These are the special abilities this card will have.
			// These do not show up like other abilities; They are invisible to the player.
			// The format for custom special abilities is 'CustomSpecialAbilityClass.CustomSpecialAbilityID'.
			// The format for vanilla special abilities is SpecialTriggeredAbility.Ability'.
			//.AddSpecialAbilities(NewTestSpecialAbility.TestSpecialAbility, SpecialTriggeredAbility.CardsInHand)

			// MetaCategories tell the game where this card should be available as a rewward or for other purposes.
			// In this case, CardMetaCategory.Rare tells the game to put this card in the post-boss reward event.

			// The first image is the card portrait.
			// The second image is the emissive portrait.
			.SetPortrait(SigilUtils.LoadTextureFromResource(Artwork.void_Jack), SigilUtils.LoadTextureFromResource(Artwork.void_Jack_e))

			;

			// Pass the card to the API.
			CardManager.Add(Jackalope);
		}

		// --------------------------------------------------------------------------------------------------------------------------------------------------

	}
}

