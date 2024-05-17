using System;
using GameFlow.Internal;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
            COMPILING_AGAIN,
            END
        }

        private GameFlowManagerEditorDraw managerDraw;
        private State windowState = State.IDLE;
        private AssetReference assetReferenceGenerate;
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

            managerDraw = new GameFlowManagerEditorDraw(rootVisualElement, GeneratePoint);
        }

        private void GeneratePoint(bool isUserInterface, bool isScene, string templatePath, string elementName)
        {
            var path = "/" + GameFlowManagerObject.kDefaultConfigFolderName +
                       "/" + GameFlowManagerObject.kDefaultElementsFolderName +
                       "/" + (isUserInterface
                           ? GameFlowManagerObject.kDefaultUserInterfaceFlowElementsFolderName
                           : GameFlowManagerObject.kDefaultGameFlowElementsFolderName) +
                       "/" + elementName + (isScene ? ".unity" : ".prefab");

            var targetPath = Application.dataPath + path;
            GenerateElementUtility.CreateTemplateClone(templatePath, targetPath);
            var unityPath = "Assets" + path;
            AssetDatabase.ImportAsset(unityPath);
            if (isScene)
            {
                prefabGenerate = AssetDatabase.LoadAssetAtPath<GameObject>(unityPath);
            }
            else
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(targetPath);
            }

            assetReferenceGenerate = AddressableUtility.AddAddressableGroup(unityPath, true);
            windowState = State.GENERATING;
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
                        EditorUtility.DisplayProgressBar("Compiling Scripts", "Wait for a few seconds...", 0.33f);
                    }
                    else
                    {
                        EditorUtility.ClearProgressBar();
                        // CreateScriptableObject();
                        windowState = State.COMPILING_AGAIN;
                    }

                    break;
                case State.COMPILING_AGAIN:
                    if (EditorApplication.isCompiling)
                    {
                        EditorUtility.DisplayProgressBar("Compiling Scripts", "Wait for a few seconds...", 0.66f);
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
                    // SaveAsset();
                    rootVisualElement.Clear();
                    CreateGUI();
                    break;
            }
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