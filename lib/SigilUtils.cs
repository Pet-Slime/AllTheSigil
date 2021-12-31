using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using APIPlugin;
using BepInEx;
using DiskCardGame;
using UnityEngine;
using static System.IO.File;

namespace voidSigils
{
	public static class SigilUtils
	{
		public static AbilityInfo CreateInfoWithDefaultSettings(
			string rulebookName, string rulebookDescription, string LearnDialogue, bool withDialogue = false, int powerLevel = 0, bool leshyUsable = false
		)
		{
			AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
			info.powerLevel = powerLevel;
			info.rulebookName = rulebookName;
			info.rulebookDescription = rulebookDescription;
			info.metaCategories = new List<AbilityMetaCategory>()			
			{
				AbilityMetaCategory.Part1Modular, AbilityMetaCategory.Part1Rulebook
			};
			info.opponentUsable = leshyUsable;
			if (withDialogue)
			{
				info.abilityLearnedDialogue = SetAbilityInfoDialogue(LearnDialogue);
			}

			return info;
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

		public static int getAbilityCount(PlayableCard card, Ability ability)
		{
			if  (!card.temporaryMods.Exists((CardModificationInfo x) => x.negateAbilities.Contains(ability)))
            {
				var positiveCount = Mathf.Max(card.Info.Abilities.FindAll((Ability x) => x == ability).Count, 1);
				return positiveCount;
			} else
            {
				return 0;
            }
		}

		public static AbilityIdentifier GetAbilityId(string rulebookName)
		{
			return AbilityIdentifier.GetAbilityIdentifier(voidSigils.Plugin.PluginGuid, rulebookName);
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