using System.Collections.Generic;
using System.IO;
using GameFlow.Component;
using GameFlow.Internal;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        internal static LoadingController loadingController { get; private set; }
        internal static DisplayLoading imageLoading { get; private set; }
        internal static FadeLoading fadeLoading { get; private set; }
        internal static ProgressLoading progressLoading { get; private set; }

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
            CreateRuntimeControllerIfNeed();
            CreateCameraIfNeed();
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

        private static void CreateRuntimeControllerIfNeed()
        {
            runtimeController = root.GetComponent<GameFlowRuntimeController>();
            if (runtimeController != null)
            {
                imageLoading = runtimeController.GetComponentInChildren<DisplayLoading>();
                fadeLoading = runtimeController.GetComponentInChildren<FadeLoading>();
                progressLoading = runtimeController.GetComponentInChildren<ProgressLoading>();
                return;
            }

            runtimeController = root.AddComponent<GameFlowRuntimeController>();
            loadingController = runtimeController.GetComponentInChildren<LoadingController>();
            imageLoading = new GameObject("Image").AddComponent<DisplayLoading>();
            imageLoading.transform.SetParent(loadingController.transform);
            fadeLoading = new GameObject("Fade").AddComponent<FadeLoading>();
            fadeLoading.transform.SetParent(loadingController.transform);
            progressLoading = new GameObject("Progress").AddComponent<ProgressLoading>();
            progressLoading.transform.SetParent(loadingController.transform);
            progressLoading.progressSlider = progressLoading.gameObject.AddComponent<Slider>();
            loadingController.RegisterControllers(imageLoading, fadeLoading, progressLoading);
            EditorSceneManager.SaveScene(managerScene);
        }

        private static void CreateCameraIfNeed()
        {
            var camera = root.GetComponent<FlowUICamera>();
            if (camera != null) return;
            camera = new GameObject("Camera").AddComponent<FlowUICamera>();
            camera.transform.SetParent(root.transform);
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