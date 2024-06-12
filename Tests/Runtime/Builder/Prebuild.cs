using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFlow.Component;
using GameFlow.Internal;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;

namespace GameFlow.Tests.Build
{
    internal static class Prebuild
    {
        internal const string kFolderParentName = "__TestResources";
        internal const string kSceneName = "TestManager";
        internal static readonly string kSceneBuildPath = $"{kFolderParentName}/{kSceneName}";
        internal static readonly string kScenePath = $"Assets/{kSceneBuildPath}.unity";
        private static Scene managerScene;
        private static GameFlowManager manager;
        private static GameObject root;
        private static GameFlowRuntimeController runtimeController;
        private static LoadingController loadingController;
        private static DisplayLoading imageLoading;
        private static FadeLoading fadeLoading;
        private static ProgressLoading progressLoading;

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
            if (File.Exists(kScenePath)) return;
            CreateSceneManager();
            CreateManager();
            CreateRuntimeController();
            CreateCamera();
            AssetDatabase.Refresh();
            EditorSceneManager.SaveScene(managerScene, kScenePath);
            AddSceneToBuild(kScenePath);
            EditorSceneManager.CloseScene(managerScene, false);
        }

        private static void CreateSceneManager()
        {
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
        }

        private static void AddSceneToBuild(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes;
            var sceneAlreadyAdded = scenes.Any(scene => scene.path == scenePath);
            if (sceneAlreadyAdded) return;
            ArrayUtility.Add(ref scenes, new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes;
        }

        private static void CreateManager()
        {
            Directory.CreateDirectory(PackagePath.ProjectFolderPath());
            manager = ScriptableObject.CreateInstance<GameFlowManager>();
            AssetDatabase.CreateAsset(manager, PackagePath.ManagerPath());
            AddAddressableGroup(PackagePath.ManagerPath());
        }

        private static void CreateRuntimeController()
        {
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
        }

        private static void CreateCamera()
        {
            var camera = new GameObject("Camera").AddComponent<FlowUICamera>();
            camera.transform.SetParent(root.transform);
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