using System.Collections;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Sire
		private void AddHourglass()
		{
			// setup ability
			const string rulebookName = "Hourglass";
			const string rulebookDescription = "[creature] will cause the opponant to skip their turn when played.";
			const string LearnDialogue = "The sands of time tic away";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Hourglass);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = 6;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Hourglass.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Hourglass), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Hourglass : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		public override bool RespondsToResolveOnBoard()
		{
			return true;
		}

		public override IEnumerator OnResolveOnBoard()
		{
			yield return base.PreSuccessfulTriggerSequence();
			yield return new WaitForSeconds(0.2f);
			Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
			yield return new WaitForSeconds(0.25f);
			Singleton<TurnManager>.Instance.Opponent.SkipNextTurn = true;
			Singleton<TextDisplayer>.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowUntilInput("I'll pass my next turn.", -0.65f, 0.4f, Emotion.Neutral, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null, true));
			yield return new WaitForSeconds(0.2f);
			yield return base.LearnAbility(0f);
			yield break;
		}


	}
}