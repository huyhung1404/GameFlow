using System.Reflection;
using GameFlow.Editor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace GameFlow.Tests.Build
{
    public static class PrebuildSetup
    {
        public static void CheckingResource()
        {
#if UNITY_EDITOR
            AddressableAssetIsAvailable();
            GameFlowManagerIsAvailable();
#endif
        }

#if UNITY_EDITOR
        private static void AddressableAssetIsAvailable()
        {
            var type = typeof(AddressableAssetSettingsDefaultObject);
            var setting = type.GetProperty("Settings", BindingFlags.Public | BindingFlags.Static);
            if (setting == null || setting.GetValue(null) == null) Debug.LogError("Set Up Addressable Asset First");
        }

        private static void GameFlowManagerIsAvailable()
        {
            if (GameFlowManagerObject.Instance == null) Debug.LogError("Create Game Flow Folder");
        }
#endif
    }
}