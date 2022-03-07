using System.Collections.Generic;
using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections;
using Pixelplacement;
using System.Linq;
using GBC;
using Artwork = voidSigils.Voids_work.Resources.Resources;


namespace voidSigils
{

    internal class SigilPatches
    {

		[HarmonyPatch(typeof(PackMule), nameof(PackMule.RespondsToResolveOnBoard))]
		public class PackMulePatch
		{
			[HarmonyPostfix]
			public static void Postfix(ref bool __result)
			{
				__result = true;
			}
		}

		[HarmonyPatch(typeof(CombatPhaseManager), "DoCombatPhase", MethodType.Normal)]
		public class Shove_Combatphase_Startpatch
		{
			[HarmonyPrefix]
			public static void DoCombatPhase()
			{
				Plugin.voidCombatPhase = true;
			}
		}

		[HarmonyPatch(typeof(TurnManager), "DoUpkeepPhase", MethodType.Normal)]
		public class Shove_Combatphase_Endpatch
		{
			[HarmonyPrefix]
			public static void DoUpkeepPhase()
			{
				Plugin.voidCombatPhase = false;
			}
		}

		[HarmonyPatch(typeof(BuildTotemSequencer))]
		public class void_TeethPatch_CostChoiceSequencer
		{
			[HarmonyPostfix, HarmonyPatch(nameof(BuildTotemSequencer.NewPiecePhase))]
			public static IEnumerator Postfix(
			IEnumerator enumerator,
			BuildTotemSequencer __instance,
			BuildTotemNodeData nodeData
			)
			{
				var negativeTotemBottom = false;
				foreach (Ability item in RunState.Run.totemBottoms)
				{
					var test2 = AbilitiesUtil.GetInfo(item).powerLevel;
					if (test2 <= -1)
                    {
						negativeTotemBottom = true;
					}
				}
				yield return new WaitForSeconds(0.25f);
				if (negativeTotemBottom)
				{
					SelectableItemSlot selectedSlot = null;
					List<ItemData> totemChoices = __instance.GenerateTotemChoices(nodeData, SaveManager.SaveFile.GetCurrentRandomSeed());
					yield return new WaitForSeconds(0.25f);
					foreach (SelectableItemSlot slot in __instance.slots)
					{
						int choiceIndex = __instance.slots.IndexOf(slot);
						if (choiceIndex < totemChoices.Count)
						{
							__instance.CreatePieceInSlot(totemChoices[choiceIndex], slot);
							slot.CursorSelectStarted = (Action<MainInputInteractable>)Delegate.Combine(slot.CursorSelectStarted, (Action<MainInputInteractable>)delegate (MainInputInteractable i)
							{
								selectedSlot = i as SelectableItemSlot;
							});
							slot.CursorEntered = (Action<MainInputInteractable>)Delegate.Combine(slot.CursorEntered, (Action<MainInputInteractable>)delegate (MainInputInteractable i)
							{
								Singleton<OpponentAnimationController>.Instance.SetLookTarget(i.transform, Vector3.up * 2f);
							});
							yield return new WaitForSeconds(0.1f);
						}
					}
					yield return new WaitForSeconds(0.5f);
					__instance.SetSlotCollidersEnabled(collidersEnabled: true);
					yield return new WaitUntil(() => selectedSlot != null);
					Singleton<RuleBookController>.Instance.SetShown(shown: false);
					Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Locked;
					if (selectedSlot.Item.Data is TotemTopData)
					{
						RunState.Run.totemTops.Add((selectedSlot.Item.Data as TotemTopData).prerequisites.tribe);
					}
					else
					{
						RunState.Run.totemBottoms.Add((selectedSlot.Item.Data as TotemBottomData).effectParams.ability);
					}
					Plugin.Log.LogError("patch Fired");
					__instance.CreatePieceInSlot(selectedSlot.Item.Data, __instance.GetFirstEmptyInventorySlot());
					__instance.SetSlotsActive(active: false);
					yield return new WaitForSeconds(0.5f);
					yield return Singleton<TextDisplayer>.Instance.ShowUntilInput("The Woodcarver reached out to offer another piece...", -0.65f, 0.4f, Emotion.Neutral, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null, true);
					yield return new WaitForSeconds(0.5f);
					selectedSlot = null;
					totemChoices = __instance.GenerateTotemChoices(nodeData, SaveManager.SaveFile.GetCurrentRandomSeed());
					yield return new WaitForSeconds(0.25f);
					foreach (SelectableItemSlot slot in __instance.slots)
					{
						int choiceIndex = __instance.slots.IndexOf(slot);
						if (choiceIndex < totemChoices.Count)
						{
							__instance.CreatePieceInSlot(totemChoices[choiceIndex], slot);
							slot.CursorSelectStarted = (Action<MainInputInteractable>)Delegate.Combine(slot.CursorSelectStarted, (Action<MainInputInteractable>)delegate (MainInputInteractable i)
							{
								selectedSlot = i as SelectableItemSlot;
							});
							slot.CursorEntered = (Action<MainInputInteractable>)Delegate.Combine(slot.CursorEntered, (Action<MainInputInteractable>)delegate (MainInputInteractable i)
							{
								Singleton<OpponentAnimationController>.Instance.SetLookTarget(i.transform, Vector3.up * 2f);
							});
							yield return new WaitForSeconds(0.1f);
						}
					}
					yield return new WaitForSeconds(0.5f);
					__instance.SetSlotCollidersEnabled(collidersEnabled: true);
					yield return new WaitUntil(() => selectedSlot != null);
					Singleton<RuleBookController>.Instance.SetShown(shown: false);
					Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Locked;
					if (selectedSlot.Item.Data is TotemTopData)
					{
						RunState.Run.totemTops.Add((selectedSlot.Item.Data as TotemTopData).prerequisites.tribe);
					}
					else
					{
						RunState.Run.totemBottoms.Add((selectedSlot.Item.Data as TotemBottomData).effectParams.ability);
					}
					
					__instance.DisableSlotsAndExitItems(selectedSlot);
					Singleton<OpponentAnimationController>.Instance.ClearLookTarget();
					yield return new WaitForSeconds(0.2f);
					selectedSlot.Item.PlayExitAnimation();
					yield return new WaitForSeconds(0.15f);
					__instance.CreatePieceInSlot(selectedSlot.Item.Data, __instance.GetFirstEmptyInventorySlot());
					__instance.SetSlotsActive(active: false);
					yield break;
				}
				else
				{
					SelectableItemSlot selectedSlot = null;
					List<ItemData> totemChoices = __instance.GenerateTotemChoices(nodeData, SaveManager.SaveFile.GetCurrentRandomSeed());
					yield return new WaitForSeconds(0.25f);
					foreach (SelectableItemSlot slot in __instance.slots)
					{
						int choiceIndex = __instance.slots.IndexOf(slot);
						if (choiceIndex < totemChoices.Count)
						{
							__instance.CreatePieceInSlot(totemChoices[choiceIndex], slot);
							slot.CursorSelectStarted = (Action<MainInputInteractable>)Delegate.Combine(slot.CursorSelectStarted, (Action<MainInputInteractable>)delegate (MainInputInteractable i)
							{
								selectedSlot = i as SelectableItemSlot;
							});
							slot.CursorEntered = (Action<MainInputInteractable>)Delegate.Combine(slot.CursorEntered, (Action<MainInputInteractable>)delegate (MainInputInteractable i)
							{
								Singleton<OpponentAnimationController>.Instance.SetLookTarget(i.transform, Vector3.up * 2f);
							});
							yield return new WaitForSeconds(0.1f);
						}
					}
					yield return new WaitForSeconds(0.5f);
					__instance.SetSlotCollidersEnabled(collidersEnabled: true);
					yield return new WaitUntil(() => selectedSlot != null);
					Singleton<RuleBookController>.Instance.SetShown(shown: false);
					Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Locked;
					var test3 = false;
					if (selectedSlot.Item.Data is TotemTopData)
					{
						RunState.Run.totemTops.Add((selectedSlot.Item.Data as TotemTopData).prerequisites.tribe);
					}
					else
					{
						RunState.Run.totemBottoms.Add((selectedSlot.Item.Data as TotemBottomData).effectParams.ability);
						if (AbilitiesUtil.GetInfo((selectedSlot.Item.Data as TotemBottomData).effectParams.ability).powerLevel <= -1)
						{
							test3 = true;
						}
					}

					__instance.DisableSlotsAndExitItems(selectedSlot);
					Singleton<OpponentAnimationController>.Instance.ClearLookTarget();
					yield return new WaitForSeconds(0.2f);
					selectedSlot.Item.PlayExitAnimation();
					yield return new WaitForSeconds(0.15f);
					__instance.CreatePieceInSlot(selectedSlot.Item.Data, __instance.GetFirstEmptyInventorySlot());
					__instance.SetSlotsActive(active: false);
					if (test3)
                    {
						yield return Singleton<TextDisplayer>.Instance.ShowUntilInput("The Woodcarver is amused by your choice...", -0.65f, 0.4f, Emotion.Neutral, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null, true);
						yield return Singleton<TextDisplayer>.Instance.ShowUntilInput("[c:bR]She[c:] [c:bR]will[c:] [c:bR]now[c:] [c:bR]offer[c:] [c:bR]you[c:] [c:bR]more[c:] [c:bR]totem[c:] [c:bR]pieces[c:] [c:bR]each[c:] [c:bR]time[c:] [c:bR]you[c:] [c:bR]visit[c:] [c:bR]her[c:][c:bR].[c:]", -0.65f, 0.4f, Emotion.Neutral, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null, true);
					}
					yield break;

				}
			}
		}
	}
}
