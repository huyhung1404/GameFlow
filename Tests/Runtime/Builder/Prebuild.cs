using System;
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
using Object = UnityEngine.Object;

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
            manager = null;
            root = null;
            runtimeController = null;
            loadingController = null;
            imageLoading = null;
            fadeLoading = null;
            progressLoading = null;
            if (File.Exists(kScenePath)) return;
            CreateSceneManager();
            CreateManager();
            CreateRuntimeController();
            CreateCamera();
            AssetDatabase.Refresh();
            EditorSceneManager.SaveScene(managerScene, kScenePath);
            AddSceneToBuild(kScenePath);
            EditorSceneManager.CloseScene(managerScene, false);
            CreateTestElements();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static void CreateSceneManager()
        {
            managerScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            if (!Directory.Exists($"Assets/{kFolderParentName}"))
            {
                Directory.CreateDirectory($"Assets/{kFolderParentName}");
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
            AddAddressableGroup(PackagePath.ManagerPath(), false);
            manager = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
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
            loadingController.RegisterControllers(false, imageLoading, fadeLoading, progressLoading);
        }

        private static void CreateCamera()
        {
            var camera = new GameObject("Camera").AddComponent<FlowUICamera>();
            camera.transform.SetParent(root.transform);
        }

        internal static AssetReferenceElement AddAddressableGroup(string assetPath, bool isScene)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (SetGroup(guid)) return new AssetReferenceElement(guid, isScene);
            Debug.LogError("Set Group Fail");
            return null;
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

        private static void CreateTestElements()
        {
            CreateTestSimpleElement();
            CreateTestSimpleSceneElement();
        }

        private static void CreateTestSimpleElement()
        {
            const string elementName = "SimpleGameFlowElement";
            var unityPath = GetPath(false, false, elementName);
            var instance = new GameObject(elementName);
            var element = GenerateElementInstance(typeof(TestScript___SimpleElement), elementName);
            var callback = instance.AddComponent<FlowCallbackMonoBehaviour>();
            callback.element = element;
            element.reference = GenerateAsset(instance, false, unityPath);
        }

        private static void CreateTestSimpleSceneElement()
        {
            const string elementName = "SimpleSceneGameFlowElement";
            var unityPath = GetPath(false, true, elementName);
            var instance = new GameObject(elementName);
            var element = GenerateElementInstance(typeof(TestScript___SimpleSceneElement), elementName);
            var callback = instance.AddComponent<FlowCallbackMonoBehaviour>();
            callback.element = element;
            element.reference = GenerateAsset(instance, true, unityPath);
        }

        private static string GetPath(bool isUserInterface, bool isScene, string elementName)
        {
            return isUserInterface
                ? PackagePath.AssetsUserInterfaceElementsFolderPath() + "/" + elementName + (isScene ? ".unity" : ".prefab")
                : PackagePath.AssetsElementsFolderPath() + "/" + elementName + (isScene ? ".unity" : ".prefab");
        }

        private static AssetReferenceElement GenerateAsset(GameObject create, bool isScene, string unityPath)
        {
            var folder = Path.GetDirectoryName(unityPath);
            var parentFolder = Path.GetDirectoryName(Path.GetDirectoryName(unityPath));

            if (!Directory.Exists(parentFolder))
            {
                if (parentFolder != null) Directory.CreateDirectory(parentFolder);
            }

            if (!Directory.Exists(folder))
            {
                if (folder != null) Directory.CreateDirectory(folder);
            }

            if (isScene)
            {
                var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                SceneManager.MoveGameObjectToScene(create, newScene);
                EditorSceneManager.SaveScene(newScene, unityPath);
                EditorSceneManager.CloseScene(newScene, false);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(create, unityPath);
                Object.DestroyImmediate(create);
            }

            return AddAddressableGroup(unityPath, isScene);
        }

        private static GameFlowElement GenerateElementInstance(Type type, string name)
        {
            var instance = (GameFlowElement)ScriptableObject.CreateInstance(type);
            instance.includeInBuild = true;
            manager.elementCollection.GenerateElement(instance);
            if (!Directory.Exists(PackagePath.AssetsScriptableObjectFolderPath()))
            {
                Directory.CreateDirectory(PackagePath.AssetsScriptableObjectFolderPath());
            }

            AssetDatabase.CreateAsset(instance, PackagePath.AssetsScriptableObjectFolderPath() + $"/{name}.asset");
            AddAddressableGroup(AssetDatabase.GetAssetPath(instance), false);
            return AssetDatabase.LoadAssetAtPath<GameFlowElement>(PackagePath.AssetsScriptableObjectFolderPath() + $"/{name}.asset");
        }
    }
}