using System;
using GameFlow.Internal;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public class GameFlowViewerEditorWindow : EditorWindow
    {
        private const string k_uxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/GameFlowViewerEditorWindow.uxml";
        private static Texture2D s_evenTexture;
        private static Texture2D s_oddTexture;
        internal static GUIStyle s_EvenStyle;
        internal static GUIStyle s_OddStyle;
        internal static GUIStyle s_LabelStyle;
        private IMGUIContainer _listView;
        private IMGUIContainer _infoView;
        private ViewType _currentViewType;

        public enum ViewType
        {
            Command,
            Event,
            Element
        }

        public static void OpenWindow()
        {
            var window = GetWindow<GameFlowViewerEditorWindow>();
            window.titleContent = new GUIContent("Game Flow Event Viewer");
            window.minSize = new Vector2(630, 250);
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.update += OnUpdate;
            s_evenTexture = new Texture2D(1, 1);
            s_evenTexture.SetPixel(0, 0, new Color(0.4f, 0.4f, 0.4f, 0.4f));
            s_evenTexture.Apply();

            s_oddTexture = new Texture2D(1, 1);
            s_oddTexture.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 0.6f));
            s_oddTexture.Apply();
        }

        private void OnUpdate()
        {
            if (!Application.isPlaying) return;
            if (Time.frameCount % 3 != 0) return;
            if (_listView == null) return;
            if (_infoView == null) return;
            _listView.MarkDirtyRepaint();
            _infoView.MarkDirtyRepaint();
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
            DestroyImmediate(s_evenTexture);
            DestroyImmediate(s_oddTexture);
        }

        private void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_uxmlPath);
            VisualElement treeFromUXML = visualTree.Instantiate();
            treeFromUXML.style.height = Length.Percent(100);
            _listView = treeFromUXML.Q<IMGUIContainer>("list_view");
            _listView.onGUIHandler += ListViewGUI;
            _infoView = treeFromUXML.Q<IMGUIContainer>("element_content");
            _infoView.onGUIHandler += InfoGUI;
            rootVisualElement.Add(treeFromUXML);
            InitToggle();
        }

        private void InitToggle()
        {
            _currentViewType = (ViewType)EditorPrefs.GetInt("com.huyhung1404.gameflow.viewerViewType", 0);
            var commandToggle = rootVisualElement.Q<ToolbarToggle>("command_toggle");
            commandToggle.value = _currentViewType == ViewType.Command;
            var eventToggle = rootVisualElement.Q<ToolbarToggle>("event_toggle");
            eventToggle.value = _currentViewType == ViewType.Event;
            var elementToggle = rootVisualElement.Q<ToolbarToggle>("element_toggle");
            elementToggle.value = _currentViewType == ViewType.Element;
            commandToggle.RegisterCallback(ToggleCallback(commandToggle, eventToggle, elementToggle, ViewType.Command));
            eventToggle.RegisterCallback(ToggleCallback(eventToggle, commandToggle, elementToggle, ViewType.Event));
            elementToggle.RegisterCallback(ToggleCallback(elementToggle, commandToggle, eventToggle, ViewType.Element));
        }

        private EventCallback<ClickEvent> ToggleCallback(ToolbarToggle currentToggle, ToolbarToggle otherToggle, ToolbarToggle otherToggle2, ViewType viewType)
        {
            return _ =>
            {
                currentToggle.value = true;
                if (!otherToggle.value && !otherToggle2.value) return;
                otherToggle.value = false;
                otherToggle2.value = false;
                _currentViewType = viewType;
                EditorPrefs.SetInt("com.huyhung1404.gameflow.viewerViewType", (int)_currentViewType);
            };
        }

        private void ListViewGUI()
        {
            if (!Application.isPlaying) return;

            s_EvenStyle ??= new GUIStyle
            {
                normal = new GUIStyleState
                {
                    background = s_evenTexture
                }
            };

            s_OddStyle ??= new GUIStyle
            {
                normal = new GUIStyleState
                {
                    background = s_oddTexture
                }
            };
            s_LabelStyle ??= new GUIStyle(GUI.skin.label)
            {
                richText = true
            };

            switch (_currentViewType)
            {
                default:
                case ViewType.Command:
                    DrawCommandViewerUtility.OnGUIList();
                    break;
                case ViewType.Event:
                    DrawListEventViewerUtility.OnGUIList();
                    break;
                case ViewType.Element:
                    DrawElementViewerUtility.OnGUIList();
                    break;
            }
        }

        private void InfoGUI()
        {
            if (!Application.isPlaying) return;
            switch (_currentViewType)
            {
                default:
                case ViewType.Command:
                    DrawCommandViewerUtility.OnGUIInfo();
                    break;
                case ViewType.Event:
                    DrawListEventViewerUtility.OnGUIInfo();
                    break;
                case ViewType.Element:
                    DrawElementViewerUtility.OnGUIInfo();
                    break;
            }
        }
    }


    internal static class DrawCommandViewerUtility
    {
        private static Vector2 scrollPosition;
        private static Vector2 scrollInfoPosition;
        private static Command currentCommand;

        internal static void OnGUIList()
        {
            var commands = GameFlowRuntimeController.GetInfo(out var current);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            int index;
            if (current != null)
            {
                DrawElement(0, current, true, false);
                index = 1;
            }
            else
            {
                index = 0;
            }

            foreach (var command in commands)
            {
                if (command == null) continue;
                DrawElement(index, command, false, false);
                index++;
            }

            foreach (var command in Command.waitBuildCommands)
            {
                if (command == null) continue;
                DrawElement(index, command, false, true);
                index++;
            }

            EditorGUILayout.EndScrollView();
        }

        internal static void OnGUIInfo()
        {
            if (currentCommand == null) return;
            GameFlowRuntimeController.GetInfo(out var current);
            var isCurrent = currentCommand == current;
            var isWaitBuild = Command.waitBuildCommands.Contains(currentCommand);
            scrollInfoPosition = EditorGUILayout.BeginScrollView(scrollInfoPosition);
            EditorGUILayout.LabelField(GetTitle(isCurrent, isWaitBuild, currentCommand).Trim(), GameFlowViewerEditorWindow.s_LabelStyle);
            EditorGUILayout.Space(2);
            EditorGUILayout.TextArea(currentCommand.GetFullInfo(), GameFlowViewerEditorWindow.s_LabelStyle);
            EditorGUILayout.EndScrollView();
        }

        private static void DrawElement(int index, Command command, bool isCurrent, bool isWaitBuild)
        {
            EditorGUILayout.BeginHorizontal(index % 2 == 0 ? GameFlowViewerEditorWindow.s_EvenStyle : GameFlowViewerEditorWindow.s_OddStyle);
            EditorGUILayout.LabelField(GetTitle(isCurrent, isWaitBuild, command), GameFlowViewerEditorWindow.s_LabelStyle);
            HandleMouseClick(GUILayoutUtility.GetLastRect(), command);
            EditorGUILayout.EndHorizontal();
        }

        private static string GetTitle(bool isCurrent, bool isWaitBuild, Command command)
        {
            if (isWaitBuild) return $"<b><size=9>B     </size></b> {command.GetInfo()}";
            return isCurrent
                ? $"<b><size=9>C     </size></b> {command.GetInfo()}"
                : $"       {command.GetInfo()}";
        }

        private static void HandleMouseClick(Rect rect, Command command)
        {
            var e = Event.current;
            if (e.type != EventType.MouseDown || e.button != 0 || !rect.Contains(e.mousePosition)) return;
            currentCommand = command;
        }
    }

    internal static class DrawListEventViewerUtility
    {
        private static Vector2 scrollPosition;
        private static Vector2 scrollInfoPosition;
        private static Type currentInfoType;
        private static ElementCallbackEvent currentCallback;

        internal static void OnGUIList()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            var index = 0;
            foreach (var keyValue in FlowObservable.callbackEvents)
            {
                DrawElement(index, keyValue.Key, keyValue.Value);
                index++;
            }

            EditorGUILayout.EndScrollView();
        }

        internal static void OnGUIInfo()
        {
            if (currentInfoType == null) return;
            if (currentCallback == null) return;
            scrollInfoPosition = EditorGUILayout.BeginScrollView(scrollInfoPosition);
            EditorGUILayout.LabelField(GetTitle(currentCallback is UIElementCallbackEvent, currentInfoType).Trim(), GameFlowViewerEditorWindow.s_LabelStyle);
            EditorGUILayout.Space(2);
            EditorGUILayout.TextArea(currentCallback.ToString(), GameFlowViewerEditorWindow.s_LabelStyle);
            EditorGUILayout.EndScrollView();
        }

        private static void DrawElement(int index, Type type, ElementCallbackEvent callbackEvent)
        {
            EditorGUILayout.BeginHorizontal(index % 2 == 0 ? GameFlowViewerEditorWindow.s_EvenStyle : GameFlowViewerEditorWindow.s_OddStyle);
            callbackEvent.GetInfo(out var isUserInterface, out var eventCount, out var listenerCount);
            EditorGUILayout.LabelField(GetTitle(isUserInterface, type), GameFlowViewerEditorWindow.s_LabelStyle);
            HandleMouseClick(GUILayoutUtility.GetLastRect(), type, callbackEvent);
            EditorGUILayout.LabelField($"Enable: {eventCount} | {listenerCount}", GameFlowViewerEditorWindow.s_LabelStyle, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
        }

        private static string GetTitle(bool isUserInterface, Type type)
        {
            return isUserInterface
                ? $"<b><size=9>UI    </size></b> {type.Namespace}.{type.Name}"
                : $"       {type.Namespace}.{type.Name}";
        }

        private static void HandleMouseClick(Rect rect, Type type, ElementCallbackEvent callbackEvent)
        {
            var e = Event.current;
            if (e.type != EventType.MouseDown || e.button != 0 || !rect.Contains(e.mousePosition)) return;
            currentInfoType = type;
            currentCallback = callbackEvent;
        }
    }

    internal static class DrawElementViewerUtility
    {
        private static Vector2 scrollPosition;
        private static Vector2 scrollInfoPosition;
        private static GameFlowElement currentElement;

        internal static void OnGUIList()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            var index = 0;
            foreach (var element in UIElementsRuntimeManager.elementsRuntime)
            {
                DrawElement(index, element, true);
                index++;
            }

            foreach (var element in ElementsRuntimeManager.elementsRuntime)
            {
                DrawElement(index, element, false);
                index++;
            }

            EditorGUILayout.EndScrollView();
        }

        internal static void OnGUIInfo()
        {
            if (currentElement == null) return;
            scrollInfoPosition = EditorGUILayout.BeginScrollView(scrollInfoPosition);
            EditorGUILayout.LabelField(GetTitle(currentElement is UIFlowElement, currentElement).Trim(), GameFlowViewerEditorWindow.s_LabelStyle);
            EditorGUILayout.Space(2);
            EditorGUILayout.TextArea(currentElement.GetFullInfo(), GameFlowViewerEditorWindow.s_LabelStyle);
            EditorGUILayout.EndScrollView();
        }

        private static void DrawElement(int index, GameFlowElement element, bool isUI)
        {
            EditorGUILayout.BeginHorizontal(index % 2 == 0 ? GameFlowViewerEditorWindow.s_EvenStyle : GameFlowViewerEditorWindow.s_OddStyle);
            EditorGUILayout.LabelField(GetTitle(isUI, element), GameFlowViewerEditorWindow.s_LabelStyle);
            HandleMouseClick(GUILayoutUtility.GetLastRect(), element);
            EditorGUILayout.EndHorizontal();
        }

        private static string GetTitle(bool isUserInterface, GameFlowElement element)
        {
            return isUserInterface
                ? $"<b><size=9>UI    </size></b> {element.GetInfo()}"
                : $"       {element.GetInfo()}";
        }

        private static void HandleMouseClick(Rect rect, GameFlowElement element)
        {
            var e = Event.current;
            if (e.type != EventType.MouseDown || e.button != 0 || !rect.Contains(e.mousePosition)) return;
            currentElement = element;
        }
    }
}