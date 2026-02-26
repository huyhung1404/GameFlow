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
        public const string k_scriptsElementNameFormat = "{0}Element";
        private const string k_scriptsNameSpace = nameof(GameFlow);

        private enum State
        {
            Idle,
            Generating,
            Compiling,
            End
        }

        private GameFlowManagerEditorDraw _editorDraw;
        private State _windowState = State.Idle;
        private AssetReferenceElement _assetReferenceGenerate;
        private string _scriptGeneratePath;
        private GameObject _prefabGenerate;
        private string _elementNameGenerate;
        private GameFlowElement _elementGenerate;

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

            _editorDraw = new GameFlowManagerEditorDraw(rootVisualElement, GenerateElement);
        }

        private void GenerateElement(bool isUserInterface, bool isScene, string templatePath, string elementName)
        {
            var unityPath = isUserInterface
                ? PackagePath.AssetsUserInterfaceElementsFolderPath() + "/" + elementName + (isScene ? ".unity" : ".prefab")
                : PackagePath.AssetsElementsFolderPath() + "/" + elementName + (isScene ? ".unity" : ".prefab");

            try
            {
                _elementNameGenerate = elementName;
                GenerateAsset(isScene, templatePath, unityPath);
                GenerateScripts(elementName, isUserInterface);
                _windowState = State.Generating;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _windowState = State.Idle;
                throw;
            }
        }

        private void GenerateAsset(bool isScene, string templatePath, string unityPath)
        {
            _prefabGenerate = null;
            EditorElementUtility.CreateTemplateClone(templatePath, unityPath);

            AssetDatabase.ImportAsset(unityPath);
            if (!isScene)
            {
                _prefabGenerate = AssetDatabase.LoadAssetAtPath<GameObject>(unityPath);
            }
            else
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(unityPath);
            }

            _assetReferenceGenerate = AddressableUtility.AddAddressableGroup(unityPath, true, isScene);
        }

        private void GenerateScripts(string elementName, bool isUserInterface)
        {
            _scriptGeneratePath = PackagePath.ScriptsGenerateFolderPath() + "/" + string.Format(k_scriptsElementNameFormat, elementName) + ".cs";
            var templateText = File.ReadAllText(PackagePath.ProjectTemplateScriptPath(PackagePath.PathType.FullPath));
            templateText = templateText.Replace("%NAMESPACE%", k_scriptsNameSpace);
            templateText = templateText.Replace("%NAME%", string.Format(k_scriptsElementNameFormat, elementName));
            templateText = templateText.Replace("%BASE_CLASS_NAME%", isUserInterface ? nameof(UIFlowElement) : nameof(GameFlowElement));
            File.WriteAllText(PackagePath.ScriptsGenerateFolderPath(PackagePath.PathType.FullPath) + "/" + string.Format(k_scriptsElementNameFormat, elementName) + ".cs",
                templateText);
            AssetDatabase.ImportAsset(_scriptGeneratePath);
        }

        private void OnGUI()
        {
            switch (_windowState)
            {
                default:
                case State.Idle:
                    break;
                case State.Generating:
                    if (EditorApplication.isCompiling)
                    {
                        _windowState = State.Compiling;
                    }

                    break;
                case State.Compiling:
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

                        _windowState = State.End;
                    }

                    break;
                case State.End:
                    _windowState = State.Idle;
                    SaveGenerateAssets();
                    _editorDraw.UpdateView();
                    break;
            }
        }

        private void GenerateElementInstance()
        {
            var manager = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
            var type = GetAssemblyType(string.Format(k_scriptsElementNameFormat, _elementNameGenerate));
            if (type == null) throw new Exception("Type generate not exists");
            _elementGenerate = (GameFlowElement)CreateInstance(type);
            _elementGenerate.IncludeInBuild = true;
            _elementGenerate.ReleaseMode = ElementReleaseMode.ReleaseOnClose;
            _elementGenerate.Reference = _assetReferenceGenerate;
            manager.ElementCollection.GenerateElement(_elementGenerate);
            if (!Directory.Exists(PackagePath.AssetsScriptableObjectFolderPath()))
            {
                Directory.CreateDirectory(PackagePath.AssetsScriptableObjectFolderPath());
            }

            AssetDatabase.CreateAsset(_elementGenerate,
                PackagePath.AssetsScriptableObjectFolderPath() + $"/{string.Format(k_scriptsElementNameFormat, _elementNameGenerate)}.asset");
            AddressableUtility.AddAddressableGroupController(AssetDatabase.GetAssetPath(_elementGenerate));
            EditorUtility.SetDirty(manager);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static Type GetAssemblyType(string typeName)
        {
            typeName = k_scriptsNameSpace + "." + typeName;
            return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly?.GetType(typeName))
                .FirstOrDefault(type => type != null && type.IsSubclassOf(typeof(GameFlowElement)));
        }

        private void SaveGenerateAssets()
        {
            var type = _elementGenerate.GetType();
            if (_prefabGenerate != null)
            {
                foreach (var child in _prefabGenerate.GetComponentsInChildren<ElementMonoBehaviours>(true))
                {
                    child.SetElement(_elementGenerate, type);
                }

                EditorUtility.SetDirty(_prefabGenerate);
                Selection.activeObject = _prefabGenerate;
                return;
            }

#if UNITY_6000_0_OR_NEWER
            var children = FindObjectsByType<ElementMonoBehaviours>(
                FindObjectsInactive.Include, 
                FindObjectsSortMode.None);
#else
            var children = FindObjectsOfType<ElementMonoBehaviours>(true);
#endif

            foreach (var child in children)
            {
                child.SetElement(_elementGenerate, type);
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