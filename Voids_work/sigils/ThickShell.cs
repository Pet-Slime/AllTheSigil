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
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.thickshell_sigil_a2);

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

    }
}
