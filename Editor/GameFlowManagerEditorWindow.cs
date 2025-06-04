using System;
using System.IO;
using System.Linq;
using GameFlow.Component;
using GameFlow.Internal;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public class GameFlowManagerEditorWindow : EditorWindow
    {
        public const string kScriptsElementNameFormat = "{0}Element";
        private const string kScriptsNameSpace = nameof(GameFlow);

        private enum State
        {
            IDLE,
            GENERATING,
            COMPILING,
            END
        }

        private GameFlowManagerEditorDraw editorDraw;
        private State windowState = State.IDLE;
        private AssetReferenceElement assetReferenceGenerate;
        private string scriptGeneratePath;
        private GameObject prefabGenerate;
        private string elementNameGenerate;
        private GameFlowElement elementGenerate;

        public static void OpenWindow()
        {
            EditorViewDataKey.OnEnable();
            var window = GetWindow<GameFlowManagerEditorWindow>();
            window.titleContent = new GUIContent("Game Flow Manager");
            window.minSize = new Vector2(430, 250);
            window.Show();
        }

        private void OnDisable()
        {
            EditorViewDataKey.OnDisable();
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

            editorDraw = new GameFlowManagerEditorDraw(rootVisualElement, GenerateElement);
        }

        private void GenerateElement(bool isUserInterface, bool isScene, string templatePath, string elementName)
        {
            var unityPath = isUserInterface
                ? PackagePath.AssetsUserInterfaceElementsFolderPath() + "/" + elementName + (isScene ? ".unity" : ".prefab")
                : PackagePath.AssetsElementsFolderPath() + "/" + elementName + (isScene ? ".unity" : ".prefab");

            try
            {
                elementNameGenerate = elementName;
                GenerateAsset(isScene, templatePath, unityPath);
                GenerateScripts(elementName, isUserInterface);
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
            EditorElementUtility.CreateTemplateClone(templatePath, unityPath);

            AssetDatabase.ImportAsset(unityPath);
            if (!isScene)
            {
                prefabGenerate = AssetDatabase.LoadAssetAtPath<GameObject>(unityPath);
            }
            else
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(unityPath);
            }

            assetReferenceGenerate = AddressableUtility.AddAddressableGroup(unityPath, true, isScene);
        }

        private void GenerateScripts(string elementName, bool isUserInterface)
        {
            scriptGeneratePath = PackagePath.ScriptsGenerateFolderPath() + "/" + string.Format(kScriptsElementNameFormat, elementName) + ".cs";
            var templateText = File.ReadAllText(PackagePath.ProjectTemplateScriptPath(PackagePath.PathType.FullPath));
            templateText = templateText.Replace("%NAMESPACE%", kScriptsNameSpace);
            templateText = templateText.Replace("%NAME%", string.Format(kScriptsElementNameFormat, elementName));
            templateText = templateText.Replace("%BASE_CLASS_NAME%", isUserInterface ? nameof(UIFlowElement) : nameof(GameFlowElement));
            File.WriteAllText(PackagePath.ScriptsGenerateFolderPath(PackagePath.PathType.FullPath) + "/" + string.Format(kScriptsElementNameFormat, elementName) + ".cs",
                templateText);
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
                        try
                        {
                            GenerateElementInstance();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }

                        windowState = State.END;
                    }

                    break;
                case State.END:
                    windowState = State.IDLE;
                    SaveGenerateAssets();
                    editorDraw.UpdateView();
                    break;
            }
        }

        private void GenerateElementInstance()
        {
            var manager = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
            var type = GetAssemblyType(string.Format(kScriptsElementNameFormat, elementNameGenerate));
            if (type == null) throw new Exception("Type generate not exists");
            elementGenerate = (GameFlowElement)CreateInstance(type);
            elementGenerate.includeInBuild = true;
            elementGenerate.releaseMode = ElementReleaseMode.RELEASE_ON_CLOSE;
            elementGenerate.reference = assetReferenceGenerate;
            manager.elementCollection.GenerateElement(elementGenerate);
            if (!Directory.Exists(PackagePath.AssetsScriptableObjectFolderPath()))
            {
                Directory.CreateDirectory(PackagePath.AssetsScriptableObjectFolderPath());
            }

            AssetDatabase.CreateAsset(elementGenerate,
                PackagePath.AssetsScriptableObjectFolderPath() + $"/{string.Format(kScriptsElementNameFormat, elementNameGenerate)}.asset");
            AddressableUtility.AddAddressableGroupController(AssetDatabase.GetAssetPath(elementGenerate));
            EditorUtility.SetDirty(manager);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static Type GetAssemblyType(string typeName)
        {
            typeName = kScriptsNameSpace + "." + typeName;
            return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly?.GetType(typeName))
                .FirstOrDefault(type => type != null && type.IsSubclassOf(typeof(GameFlowElement)));
        }

        private void SaveGenerateAssets()
        {
            var type = elementGenerate.GetType();
            if (prefabGenerate != null)
            {
                foreach (var child in prefabGenerate.GetComponentsInChildren<ElementMonoBehaviours>(true))
                {
                    child.SetElement(elementGenerate, type);
                }

                EditorUtility.SetDirty(prefabGenerate);
                Selection.activeObject = prefabGenerate;
                return;
            }

            foreach (var child in FindObjectsOfType<ElementMonoBehaviours>(true))
            {
                child.SetElement(elementGenerate, type);
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
                    // ignored
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
        private const string kMenuItemPath = "CONTEXT/GameFlowManager/Draw Default Inspector";
        private const string kPrefsKey = "com.huyhung1404.gameflow_drawDefautInspectorManager";
        private static bool drawDefaultInspector;

        private void OnEnable()
        {
            drawDefaultInspector = EditorPrefs.GetBool(kPrefsKey, false);
            Menu.SetChecked(kMenuItemPath, drawDefaultInspector);
        }

        [MenuItem(kMenuItemPath)]
        public static void Enable_DrawDefaultInspector()
        {
            drawDefaultInspector = !drawDefaultInspector;
            EditorPrefs.SetBool(kPrefsKey, drawDefaultInspector);
            Menu.SetChecked(kMenuItemPath, drawDefaultInspector);
        }

        public override void OnInspectorGUI()
        {
            if (drawDefaultInspector) DrawDefaultInspector();
            GUILayout.Space(10);
            if (GUILayout.Button("Open Editor Window"))
            {
                GameFlowManagerEditorWindow.OpenWindow();
            }
        }
    }
}