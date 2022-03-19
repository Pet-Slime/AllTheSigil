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
		private void AddThickShell()
		{
			// setup ability
			const string rulebookName = "Thick Shell";
			const string rulebookDescription = "When attacked, [creature] takes 1 less damage.";
			const string LearnDialogue = "The thick shell on that creature protected it from one damage!";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_ThickShell);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_ThickShell_a2);
			int powerlevel = 1;
			bool LeshyUsable = Plugin.configThickShell.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_ThickShell.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_ThickShell), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_ThickShell : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

    }
}
