using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using DiskCardGame;
using UnityEngine;
using InscryptionAPI.Card;
using static System.IO.File;

namespace voidSigils
{
	public static class SigilUtils
	{

		public static AbilityInfo CreateAbilityWithDefaultSettings(
			string rulebookName, string rulebookDescription, Type behavior, Texture2D text_a1, Texture2D text_a2, 
			string LearnDialogue, bool withDialogue = false, int powerLevel = 0, bool leshyUsable = false, bool part1Modular = true, bool stack = false
		)
		{
			AbilityInfo createdAbilityInfo = AbilityManager.New(
				voidSigils.Plugin.PluginGuid,
				rulebookName,
				rulebookDescription,
				behavior,
				text_a1
			);
			createdAbilityInfo.SetPixelAbilityIcon(text_a2);
			// This sets up the learned Dialog event
			if (withDialogue) { createdAbilityInfo.abilityLearnedDialogue = SetAbilityInfoDialogue(LearnDialogue); }
			// How powerful the ability is
			createdAbilityInfo.powerLevel = powerLevel;
			// Can it show up on totems for leshy?
			createdAbilityInfo.opponentUsable = leshyUsable;
			// If true, allows in shops and in totems. If false, just the rule book
			if (part1Modular)
            { createdAbilityInfo.metaCategories = new List<AbilityMetaCategory>() { AbilityMetaCategory.Part1Modular, AbilityMetaCategory.Part1Rulebook };}
			else 
			{ createdAbilityInfo.metaCategories = new List<AbilityMetaCategory>() { AbilityMetaCategory.Part1Rulebook }; }
			// Does the ability stack?
			createdAbilityInfo.canStack = stack;
			return createdAbilityInfo;
		}

		public static DialogueEvent.LineSet SetAbilityInfoDialogue(string dialogue)
		{
			return new DialogueEvent.LineSet(
				new List<DialogueEvent.Line>()
				{
					new DialogueEvent.Line()
					{
						text = dialogue
					}
				}
			);
		}

		public static Texture2D GetTextureFromPath(string path)
		{
			byte[] imgBytes = File.ReadAllBytes(Path.Combine(voidSigils.Plugin.Directory, path));
			Texture2D tex = new Texture2D(2, 2);
			tex.LoadImage(imgBytes);

			return tex;
		}

		public static Texture2D LoadTextureFromResource(byte[] resourceFile)
		{
			var texture = new Texture2D(2, 2);
			texture.LoadImage(resourceFile);
			texture.filterMode = FilterMode.Point;
			return texture;
		}

		public static Sprite LoadSpriteFromResource(byte[] resourceFile)
		{
			var Yposition = 0.5f;
			var Xposition = 0.5f;
			var vector = new Vector2(Xposition, Yposition);

			var texture = new Texture2D(2, 2);
			texture.LoadImage(resourceFile);
			texture.filterMode = FilterMode.Point;
			var sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), vector, 100.0f);
			return sprite;
		}

		public static int getAbilityCount(PlayableCard card, Ability ability)
		{
			if (!card.temporaryMods.Exists((CardModificationInfo x) => x.negateAbilities.Contains(ability)))
			{
				var positiveCount = Mathf.Max(card.Info.Abilities.FindAll((Ability x) => x == ability).Count, 1);
				return positiveCount;
			}
			else
			{
				return 0;
			}
		}

		public static string GetFullPathOfFile(string fileToLookFor)
		{
			return Directory.GetFiles(Paths.PluginPath, fileToLookFor, SearchOption.AllDirectories)[0];
		}

		public static byte[] ReadArtworkFileAsBytes(string nameOfCardArt)
		{
			return ReadAllBytes(GetFullPathOfFile(nameOfCardArt));
		}

		public static Texture2D LoadImageAndGetTexture(string nameOfCardArt)
		{
			Texture2D texture = new Texture2D(2, 2);
			byte[] imgBytes = ReadArtworkFileAsBytes(nameOfCardArt);
			bool isLoaded = texture.LoadImage(imgBytes);
			return texture;
		}

		/// <summary>
		/// Some cards do not have Card.Slot assigned. So this is a work around
		/// </summary>
		public static CardSlot GetSlot(PlayableCard cardToGetSlot)
		{
			if (cardToGetSlot.Slot != null)
			{
				//Plugin.Log.LogInfo("[SplashDamageAbility][GetSlot] Slot cached");
				return cardToGetSlot.Slot;
			}

			CardSlot cardSlot = cardToGetSlot.transform.parent.GetComponent<CardSlot>();
			if (cardSlot != null)
			{
				//Plugin.Log.LogInfo("[SplashDamageAbility][GetSlot] Found slot in parent");
				return cardSlot;
			}

			int cardToGetSlotID = cardToGetSlot.gameObject.GetInstanceID();
			Plugin.Log.LogInfo("[SplashDamageAbility][GetSlot] Getting slot for " + cardToGetSlotID);

			List<CardSlot> allSlots = new List<CardSlot>();
			allSlots.AddRange(Singleton<BoardManager>.Instance.GetSlots(false));
			allSlots.AddRange(Singleton<BoardManager>.Instance.GetSlots(true));

			for (int i = 0; i < allSlots.Count; i++)
			{
				CardSlot slot = allSlots[i];
				if (slot.Index != 2)
				{
					continue;
				}

				PlayableCard card = slot.Card;
				if (card == null)
					continue;

				//Plugin.Log.LogInfo("[SplashDamageAbility][GetSlot] Slot " + slot.Index + " has " + card.Info.displayedName + " from queue: " + card.OriginatedFromQueue);
				if (card.gameObject == cardToGetSlot.gameObject)
				{
					//Plugin.Log.LogInfo("[SplashDamageAbility][GetSlot] Card is in slot " + slot.Index);
					return slot;
				}
				else
				{
					int slotCardID = card.gameObject.GetInstanceID();
					//Plugin.Log.LogInfo("[SplashDamageAbility][GetSlot] " + cardToGetSlotID + " != " + slotCardID);
				}
			}

			Plugin.Log.LogInfo("[SplashDamageAbility][GetSlot] Could not find slot for " + cardToGetSlotID);
			return null;
		}

		public static String GetLogOfCardInSlot(PlayableCard playableCard)
		{
			return $"Card [{playableCard.Info.name}] Slot [{playableCard.Slot.Index}]";
		}

		
	}
}