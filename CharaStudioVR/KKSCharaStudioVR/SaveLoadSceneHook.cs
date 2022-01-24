using System;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using Manager;
using Studio;
using UnityEngine;
using VRGIN.Core;

namespace KKSCharaStudioVR
{
	public static class SaveLoadSceneHook
	{
		private static Camera[] backupRenderCam;

		private static Sprite sceneLoadScene_spriteLoad;

		public static void InstallHook()
		{
			KKSCharaStudioVRPlugin.PluginLogger.Log(LogLevel.Info, "Install SaveLoadSceneHook");
			new Harmony("KKSCharaStudioVR.SaveLoadSceneHook").PatchAll(typeof(SaveLoadSceneHook));
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(global::Studio.Studio), "SaveScene", new Type[] { })]
		public static bool SaveScenePreHook(global::Studio.Studio __instance, ref Camera[] __state)
		{
			KKSCharaStudioVRPlugin.PluginLogger.Log(LogLevel.Debug, "Update Camera position and rotation for Scene Capture and last Camera data.");
			try
			{
				VRCameraMoveHelper.Instance.CurrentToCameraCtrl();
				FieldInfo field = typeof(Studio.GameScreenShot).GetField("renderCam", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				Camera[] obj = field.GetValue(Singleton<global::Studio.Studio>.Instance.gameScreenShot) as Camera[];
				KKSCharaStudioVRPlugin.PluginLogger.Log(LogLevel.Debug, "Backup Screenshot render cam.");
				backupRenderCam = obj;
				Camera[] value = new Camera[1] { VR.Camera.SteamCam.camera };
				__state = backupRenderCam;
				field.SetValue(Singleton<global::Studio.Studio>.Instance.gameScreenShot, value);
			}
			catch (Exception obj2)
			{
				VRLog.Error("Error in SaveScenePreHook. Force continue.");
				VRLog.Error(obj2);
			}
			return true;
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(global::Studio.Studio), "SaveScene", new Type[] { })]
		public static void SaveScenePostHook(global::Studio.Studio __instance, Camera[] __state)
		{
			KKSCharaStudioVRPlugin.PluginLogger.Log(LogLevel.Debug, "Restore backup render cam.");
			try
			{
				typeof(Studio.GameScreenShot).GetField("renderCam", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(Singleton<global::Studio.Studio>.Instance.gameScreenShot, __state);
			}
			catch (Exception obj)
			{
				VRLog.Error("Error in SaveScenePostHook. Force continue.");
				VRLog.Error(obj);
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(SceneLoadScene), "OnClickLoad", new Type[] { })]
		public static void SceneLoadSceneOnClickLoadPreHook(SceneLoadScene __instance)
		{
			try
			{
				sceneLoadScene_spriteLoad = typeof(SceneLoadScene).GetField("spriteLoad", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance) as Sprite;
			}
			catch (Exception obj)
			{
				VRLog.Error(obj);
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Scene), "LoadReserve", new Type[]
		{
			typeof(Scene.Data),
			typeof(bool)
		})]
		public static void LoadScenePostHook(Scene.Data data, bool isLoadingImageDraw)
		{
			try
			{
				if (data.levelName == "StudioNotification" && data.isAdd && NotificationScene.spriteMessage == sceneLoadScene_spriteLoad)
				{
					VRCameraMoveHelper.Instance.MoveToCurrent();
				}
			}
			catch (Exception obj)
			{
				VRLog.Error(obj);
			}
		}
	}
}
