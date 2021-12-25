using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using DiskCardGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using APIPlugin;
using System.Linq;
using Resources = voidSigils.Resources.Resources;

namespace voidSigils
{
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	[BepInDependency(APIGUID, BepInDependency.DependencyFlags.HardDependency)]

	public partial class Plugin : BaseUnityPlugin
	{
		public const string APIGUID = "cyantist.inscryption.api";
		public const string PluginGuid = "extraVoid.inscryption.voidSigils";
		private const string PluginName = "Extra Sigils";
		private const string PluginVersion = "1.8.0";

		public static string Directory;
		internal static ManualLogSource Log;


		private void Awake()
		{
			Log = base.Logger;

			Harmony harmony = new(PluginGuid);
			harmony.PatchAll();

			//Attack sigils
			AddAcidTrail();
			AddAmbush();
			AddDeathburst();
			AddEletric();
			AddInsectKiller();
			AddFamiliar();
			AddDoubleAttack();
			AddPierce();
			AddTrample();

			//Buff Attack Sigils
			AddZapper();
			AddVicious();

			//Defensive sigils
			AddAgile();
			AddBodyguard();
			AddRegenFull();
			AddRegen1();
			AddRegen2();
			AddRegen3();
			AddResistant();
			AddProtector();
			AddPoisonous();
			AddThickShell();

			//Debuff sigils
			AddToxin();
			AddToxinStrength();
			AddToxinVigor();
			AddToxinDeadly();
			AddToxinSickly();

			//Negative Sigils

			AddBlight();
			AddBroken();
			AddBombardier();
			AddDying();
			AddPathetic();
			AddSickness();
			AddToothpicker();
			AddTransient();

			//Utility Sigils
			AddBloodGuzzler();
			AddBonePicker();
			AddFishHook();
			AddLeech();
			AddFisher();
			AddNutritious();
			AddShove();
			AddRepellant();
			AddScissors();
			AddThief();
			AddTribalAlly();
			AddTribalTutor();
			AddPatchedBoneDigger();
			AddPatchedBeesOnHit();
		}

		private void Start()
		{
			RemoveVanillaBeesOnHit();
		}
	}
}