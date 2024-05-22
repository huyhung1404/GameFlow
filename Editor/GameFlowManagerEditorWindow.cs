using System;
using System.IO;
using GameFlow.Internal;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public class GameFlowManagerEditorWindow : EditorWindow
    {
        private enum State
        {
            IDLE,
            GENERATING,
            COMPILING,
            END
        }

        private GameFlowManagerEditorDraw managerDraw;
        private State windowState = State.IDLE;
        private AssetReference assetReferenceGenerate;
        private string scriptGeneratePath;
        private GameObject prefabGenerate;

        public static void OpenWindow()
        {
            var window = GetWindow<GameFlowManagerEditorWindow>();
            window.titleContent = new GUIContent("Game Flow Manager");
            window.minSize = new Vector2(430, 250);
            window.Show();
        }

        public void CreateGUI()
        {
            if (AddressableAssetSettingsDefaultObject.Settings == null)
            {
                rootVisualElement.Add(new IMGUIContainer(DrawCreateAddressableAssetGUI));
                return;
            }

            if (GameFlowManagerObject.Instance == null)
            {
                rootVisualElement.Add(new IMGUIContainer(DrawCreateGameFlowGUI));
                return;
            }

            managerDraw = new GameFlowManagerEditorDraw(rootVisualElement, GenerateElement);
        }

        private void GenerateElement(bool isUserInterface, bool isScene, string templatePath, string elementName)
        {
            var unityPath = isUserInterface
                ? PackagePath.AssetsUserInterfaceElementsFolderPath() + "/" + elementName + (isScene ? ".unity" : ".prefab")
                : PackagePath.AssetsElementsFolderPath() + "/" + elementName + (isScene ? ".unity" : ".prefab");

            try
            {
                GenerateAsset(isScene, templatePath, unityPath);
                GenerateScripts(elementName);
                windowState = State.GENERATING;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                windowState = State.IDLE;
                throw;
            }
        }

        private void GenerateAsset(bool isScene, string templatePath, string unityPath)
        {
            prefabGenerate = null;
            GenerateElementUtility.CreateTemplateClone(templatePath, unityPath);

            AssetDatabase.ImportAsset(unityPath);
            if (!isScene)
            {
                prefabGenerate = AssetDatabase.LoadAssetAtPath<GameObject>(unityPath);
            }
            else
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(unityPath);
            }

            assetReferenceGenerate = AddressableUtility.AddAddressableGroup(unityPath, true);
        }

        private void GenerateScripts(string elementName)
        {
            scriptGeneratePath = PackagePath.ScriptsGenerateFolderPath() + "/" + elementName + ".cs";
            var templateText = File.ReadAllText(PackagePath.ProjectTemplateScriptPath(PackagePath.PathType.FullPath));
            templateText = templateText.Replace("%NAME%", elementName);
            templateText = templateText.Replace("%BASE_CLASS_NAME%", nameof(GameFlowElement));
            File.WriteAllText(PackagePath.ScriptsGenerateFolderPath(PackagePath.PathType.FullPath) + "/" + elementName + ".cs", templateText);
            AssetDatabase.ImportAsset(scriptGeneratePath);
        }

        private void OnGUI()
        {
            switch (windowState)
            {
                default:
                case State.IDLE:
                    break;
                case State.GENERATING:
                    if (EditorApplication.isCompiling)
                    {
                        windowState = State.COMPILING;
                    }

                    break;
                case State.COMPILING:
                    if (EditorApplication.isCompiling)
                    {
                        EditorUtility.DisplayProgressBar("Compiling Scripts", "Wait for a few seconds...", 0.5f);
                    }
                    else
                    {
                        EditorUtility.ClearProgressBar();
                        // SetUpAsset();
                        windowState = State.END;
                    }

                    break;
                case State.END:
                    windowState = State.IDLE;
                    SaveGenerateAssets();
                    rootVisualElement.Clear();
                    CreateGUI();
                    break;
            }
        }

        private void SaveGenerateAssets()
        {
            if (prefabGenerate != null)
            {
                Selection.activeObject = prefabGenerate;
                return;
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        }

        private void DrawCreateAddressableAssetGUI()
        {
            if (AddressableAssetSettingsDefaultObject.Settings != null)
            {
                rootVisualElement.Clear();
                CreateGUI();
                return;
            }

            GUILayout.Space(50);
            if (GUILayout.Button("Create Addressables Settings"))
            {
                EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
            }

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            GUI.skin.label.wordWrap = true;
            GUI.skin.label.richText = true;
            GUILayout.Space(20);
            GUILayout.Label("Click the \"Create\" button above or simply drag an asset into this window to start using Addressables." +
                            "  Once you begin, the Addressables system will save some assets to your project to keep up with its data " +
                            "\n <b>Game flow management system using Unity Addressable asset structure.</b>");
            GUILayout.Space(50);
            GUILayout.EndHorizontal();
        }

        private void DrawCreateGameFlowGUI()
        {
            if (GameFlowManagerObject.Instance != null)
            {
                try
                {
                    rootVisualElement.Clear();
                    CreateGUI();
                }
                catch (Exception)
                {
                    return;
                }

                return;
            }

            GUILayout.Space(50);
            if (GUILayout.Button("Create Game Flow Settings"))
            {
                GameFlowManagerObject.CreateDefaultInstance();
            }

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            GUI.skin.label.wordWrap = true;
            GUILayout.Space(20);
            GUILayout.Label("Click the \"Create\" button to start using Game Flow.");
            GUILayout.Space(50);
            GUILayout.EndHorizontal();
        }
    }

    [CustomEditor(typeof(GameFlowManager))]
    internal class GameFlowManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(10);
            if (GUILayout.Button("Open Editor Window"))
            {
                GameFlowManagerEditorWindow.OpenWindow();
            }
        }
    }
}