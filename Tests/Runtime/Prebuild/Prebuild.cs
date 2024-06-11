using System.Collections.Generic;
using System.IO;
using GameFlow.Internal;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFlow.Tests.Build
{
    internal static class Prebuild
    {
        internal const string kFolderParentName = "GameFlowTestResources";
        internal static readonly string kScenePath = $"Assets/{kFolderParentName}/TestManager.unity";
        internal static Scene managerScene { get; private set; }
        internal static GameFlowManager manager { get; private set; }
        internal static GameObject root { get; private set; }
        internal static GameFlowRuntimeController runtimeController { get; private set; }

        public static void PresetResources()
        {
            EditorPrefs.SetString("com.huyhung1404.gameflow.folderParentName", kFolderParentName + "/");
#if UNITY_EDITOR
            AddressableAssetIsAvailable();
#endif
            CreateResourcesIfNeed();
        }

#if UNITY_EDITOR
        private static void AddressableAssetIsAvailable()
        {
            if (AddressableAssetSettingsDefaultObject.Settings == null) Debug.LogError("Set Up Addressable Asset First");
        }
#endif

        public static void CleanupResources()
        {
            EditorPrefs.SetString("com.huyhung1404.gameflow.folderParentName", string.Empty);
            AssetDatabase.Refresh();
        }

        private static void CreateResourcesIfNeed()
        {
            CreateSceneManager();
            CreateManager();
            RuntimeControllerIfNeed();
        }

        private static void CreateSceneManager()
        {
            if (File.Exists(kScenePath))
            {
                managerScene = EditorSceneManager.OpenScene(kScenePath, OpenSceneMode.Additive);
                root = managerScene.GetRootGameObjects()[0];
                return;
            }

            managerScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
            if (!Directory.Exists($"Assets/{kFolderParentName}"))
            {
                Directory.CreateDirectory($"Assets/{kFolderParentName}");
            }

            foreach (var o in managerScene.GetRootGameObjects())
            {
                Object.DestroyImmediate(o);
            }

            root = new GameObject("root");
            SceneManager.MoveGameObjectToScene(root, managerScene);
            var success = EditorSceneManager.SaveScene(managerScene, kScenePath);
            if (success)
            {
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Failed to save the scene.");
            }
        }

        private static void CreateManager()
        {
            manager = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
            if (manager != null) return;
            Directory.CreateDirectory(PackagePath.ProjectFolderPath());
            manager = ScriptableObject.CreateInstance<GameFlowManager>();
            AssetDatabase.CreateAsset(manager, PackagePath.ManagerPath());
            AddAddressableGroup(PackagePath.ManagerPath());
        }

        private static void RuntimeControllerIfNeed()
        {
            runtimeController = root.GetComponent<GameFlowRuntimeController>();
            if (runtimeController != null) return;
            runtimeController = root.AddComponent<GameFlowRuntimeController>();
            EditorSceneManager.SaveScene(managerScene);
        }

        internal static void AddAddressableGroup(string assetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (SetGroup(guid)) return;
            Debug.LogError("Set Group Fail");
        }

        private static bool SetGroup(string guid)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            const string groupName = "GameFlowTest";
            if (!settings) return false;
            var group = settings.FindGroup(groupName);
            if (!group)
            {
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
                var bundledAssetGroupSchema = group.GetSchema<BundledAssetGroupSchema>();
                bundledAssetGroupSchema.IncludeInBuild = true;
                bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            }

            var e = settings.CreateOrMoveEntry(guid, group, false, false);
            var entriesAdded = new List<AddressableAssetEntry> { e };
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
            return true;
        }
    }
}