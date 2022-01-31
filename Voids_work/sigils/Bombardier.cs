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
		//Original
		private NewAbility AddBombardier()
		{
			// setup ability
			const string rulebookName = "Bombardier";
			const string rulebookDescription = "[creature] will deal 10 damage to a random slot with a creature in it during the end phase of every turn";
			const string LearnDialogue = "Boom";
			// const string TextureFile = "Artwork/void_pathetic.png";

			AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue,  true, 7, Plugin.configBombardier.Value);
			info.canStack = false;
			info.pixelIcon = SigilUtils.LoadSpriteFromResource(Artwork.bombardiercool_sigil_a2);

			Texture2D tex = SigilUtils.LoadTextureFromResource(Artwork.void_Bombardier);

			var abIds = SigilUtils.GetAbilityId(info.rulebookName);
			
			NewAbility newAbility = new NewAbility(info, typeof(void_Bombardier), tex, abIds);

			// set ability to behaviour class
			void_Bombardier.ability = newAbility.ability;

			return newAbility;
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