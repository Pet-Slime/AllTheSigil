using System.Collections;
using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using UnityEngine;
using Artwork = voidSigils.Voids_work.Resources.Resources;
using System;
using Random = UnityEngine.Random;
using Pixelplacement;

namespace voidSigils
{
	public partial class Plugin
	{
		//Request by Blind
		private void AddBombardier()
		{
			// setup ability
			const string rulebookName = "Bombardier";
			const string rulebookDescription = "[creature] will deal 10 damage to a random creature during the end phase of the owner's turn.";
			const string LearnDialogue = "Boom";
			Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Artwork.void_Bombardier);
			Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Artwork.void_Bombardier_a2);
			int powerlevel = 0;
			bool LeshyUsable = Plugin.configBombardier.Value;
			bool part1Shops = true;
			bool canStack = false;

			// set ability to behaviour class
			void_Bombardier.ability = SigilUtils.CreateAbilityWithDefaultSettings(rulebookName, rulebookDescription, typeof(void_Bombardier), tex_a1, tex_a2, LearnDialogue,
																					true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
		}
	}

	public class void_Bombardier : AbilityBehaviour
	{
		public override Ability Ability => ability;

		public static Ability ability;

		private void Awake()
		{
			this.bombPrefab = ResourceBank.Get<GameObject>("Prefabs/Cards/SpecificCardModels/DetonatorHoloBomb");
		}


		public override bool RespondsToTurnEnd(bool playerTurnEnd)
		{
			return base.Card != null && base.Card.OpponentCard != playerTurnEnd;
		}

		public override IEnumerator OnTurnEnd(bool playerTurnEnd)
		{
			// Get the base card
			PlayableCard card = base.Card;

			// Get all slots
			List<CardSlot> allSlots = Singleton<BoardManager>.Instance.AllSlots;

			// Initalize target list
			List<PlayableCard> targets = new List<PlayableCard>();

			// Go thru all slots to see if there is a card in it, and if there is, add it to the target list
			for (int index = 0; index < allSlots.Count; index++)
			{
				if (allSlots[index].Card != null)
			    {
					targets.Add(allSlots[index].Card);
				}
			}

			// pick a random target from the target list
			PlayableCard target = targets[Random.Range(0, (targets.Count))];

			// Blow them up
			Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
			card.Anim.LightNegationEffect();
			yield return base.PreSuccessfulTriggerSequence();
			yield return this.BombCard(target, card);
			yield return base.LearnAbility(0.25f);
			Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield break;
		}

		private IEnumerator BombCard(PlayableCard target, PlayableCard attacker)
		{
			GameObject bomb = UnityEngine.Object.Instantiate<GameObject>(this.bombPrefab);
			bomb.transform.position = attacker.transform.position + Vector3.up * 0.1f;
			Tween.Position(bomb.transform, target.transform.position + Vector3.up * 0.1f, 0.5f, 0f, Tween.EaseLinear, Tween.LoopType.None, null, null, true);
			yield return new WaitForSeconds(0.5f);
			target.Anim.PlayHitAnimation();
			UnityEngine.Object.Destroy(bomb);
			yield return target.TakeDamage(10, attacker);
			yield break;
		}

		private const string BOMB_PREFAB_PATH = "Prefabs/Cards/SpecificCardModels/DetonatorHoloBomb";

		private GameObject bombPrefab;

	}

}