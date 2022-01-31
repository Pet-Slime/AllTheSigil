using System.Collections.Generic;
using DiskCardGame;
using UnityEngine;
using APIPlugin;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils.Voids_work.Cards
{
	public static class Jackalope
	{
		public static void AddCard()
		{

			List<CardMetaCategory> metaCategories = new List<CardMetaCategory>();

			List<Tribe> Tribes = new List<Tribe>();
			Tribes.Add(Tribe.Hooved);

			List<Ability> Abilities = new List<Ability>();
			Abilities.Add(Ability.Strafe);

			List<Trait> Traits = new List<Trait>();

			List<AbilityIdentifier> customAbilities = new List<AbilityIdentifier>();
			customAbilities.Add(AbilityIdentifier.GetAbilityIdentifier("extraVoid.inscryption.voidSigils", "Draw Jackalope"));

			List<SpecialTriggeredAbility> specialAbilities = new List<SpecialTriggeredAbility>();

			List<CardAppearanceBehaviour.Appearance> appearanceBehaviour = new List<CardAppearanceBehaviour.Appearance>();

			Texture2D DefaultTexture = SigilUtils.LoadTextureFromResource(Artwork.void_Jack);
			Texture2D pixelTexture = SigilUtils.LoadTextureFromResource(Artwork.void_Acid_Puddle_p);

			Texture2D eTexture = SigilUtils.LoadTextureFromResource(Artwork.void_Jack_e);

			IceCubeIdentifier iceCubeId = null;
			EvolveIdentifier evolveId = null;
			TailIdentifier tail = null;

			NewCard.Add(name: "void_Jackalope",
				displayedName: "Jackalope",
				baseAttack: 2,
				baseHealth: 2,
				metaCategories,
				cardComplexity: CardComplexity.Advanced,
				temple: CardTemple.Nature,
				description: "Jacka",
				hideAttackAndHealth: false,
				bloodCost: 2,
				bonesCost: 0,
				energyCost: 0,
				gemsCost: null,
				specialStatIcon: SpecialStatIcon.None,
				Tribes,
				Traits,
				specialAbilities,
				Abilities,
				customAbilities,
				specialAbilitiesIdsParam: null,
				evolveParams: null,
				defaultEvolutionName: null,
				tailParams: null,
				iceCubeParams: null,
				flipPortraitForStrafe: false,
				onePerDeck: false,
				appearanceBehaviour,
				DefaultTexture,
				altTex: null,
				titleGraphic: null,
				pixelTexture,
				eTexture,
				animatedPortrait: null,
				decals: null,
				evolveId,
				iceCubeId,
				tail);
		}
	}
}

