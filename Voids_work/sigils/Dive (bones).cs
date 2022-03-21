using System.Collections;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Sire
		private void AddDiveBones()
		{
			// setup ability
			const string rulebookName = "Dive (Bones)";
			const string rulebookDescription = "Pay 2 bones to submerge this card.";
			const string LearnDialogue = "Care for a dive?";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Dive_Bones);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.no_a2);
			int powerlevel = 0;
			bool LeshyUsable = false;
			bool part1Shops = true;
			bool canStack = false;



			var test = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Dive_Bone), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack);

			test.activated = true;

			test.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.void_Dive_Bones_a2);


			// set ability to behaviour class
			void_Dive_Bone.ability = test.ability;
		}
	}

	public class void_Dive_Bone : ActivatedAbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		bool timeToDive = false;

		public override int BonesCost
		{
			get
			{
				return 2;
			}
		}

		public override IEnumerator Activate()
		{
			timeToDive = true;
			base.Card.Anim.StrongNegationEffect();
			yield break;
		}

		public override bool RespondsToUpkeep(bool playerUpkeep)
		{
			return base.Card.OpponentCard != playerUpkeep && base.Card.FaceDown;
		}

		public override IEnumerator OnUpkeep(bool playerUpkeep)
		{
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
			yield return new WaitForSeconds(0.15f);
			yield return base.PreSuccessfulTriggerSequence();
			base.Card.SetFaceDown(false, false);
			base.Card.UpdateFaceUpOnBoardEffects();
			this.OnResurface();
			yield return new WaitForSeconds(0.3f);
			this.triggerPriority = int.MinValue;
			yield break;
		}

		public override bool RespondsToTurnEnd(bool playerTurnEnd)
		{
			return base.Card.OpponentCard != playerTurnEnd && !base.Card.FaceDown && timeToDive == true;
		}

		public override IEnumerator OnTurnEnd(bool playerTurnEnd)
		{
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
			yield return new WaitForSeconds(0.15f);
			base.Card.SetCardbackSubmerged();
			base.Card.SetFaceDown(true, false);
			yield return new WaitForSeconds(0.3f);
			yield return base.LearnAbility(0f);
			this.triggerPriority = int.MaxValue;
			timeToDive = false;
			yield break;
		}

		protected virtual void OnResurface()
		{
		}


		private int triggerPriority = int.MinValue;
	}
}
