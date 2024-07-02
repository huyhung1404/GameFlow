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
        private static Texture2D evenTexture;
        private static Texture2D oddTexture;
        internal static GUIStyle evenStyle;
        internal static GUIStyle oddStyle;
        internal static GUIStyle labelStyle;
        internal static IMGUIContainer listView;
        internal static IMGUIContainer infoView;

        public enum ViewType
        {
            Command,
            Event,
            Element
        }

        private const string kUxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/GameFlowViewerEditorWindow.uxml";
        private ViewType currentViewType;

        public static void OpenWindow()
        {
            var window = GetWindow<GameFlowViewerEditorWindow>();
            window.titleContent = new GUIContent("Game Flow Event Viewer");
            window.minSize = new Vector2(630, 250);
            window.Show();
        }

        private void OnEnable()
        {
            evenTexture = new Texture2D(1, 1);
            evenTexture.SetPixel(0, 0, new Color(0.4f, 0.4f, 0.4f, 0.4f));
            evenTexture.Apply();

            oddTexture = new Texture2D(1, 1);
            oddTexture.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 0.6f));
            oddTexture.Apply();
        }

        private void OnDisable()
        {
            DestroyImmediate(evenTexture);
            DestroyImmediate(oddTexture);
        }

        private void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(kUxmlPath);
            VisualElement treeFromUXML = visualTree.Instantiate();
            treeFromUXML.style.height = Length.Percent(100);
            listView = treeFromUXML.Q<IMGUIContainer>("list_view");
            listView.onGUIHandler += ListViewGUI;
            infoView = treeFromUXML.Q<IMGUIContainer>("element_content");
            infoView.onGUIHandler += InfoGUI;
            rootVisualElement.Add(treeFromUXML);
            InitToggle();
        }

        private void InitToggle()
        {
            currentViewType = (ViewType)EditorPrefs.GetInt("com.huyhung1404.gameflow.viewerViewType", 0);
            var commandToggle = rootVisualElement.Q<ToolbarToggle>("command_toggle");
            commandToggle.value = currentViewType == ViewType.Command;
            var eventToggle = rootVisualElement.Q<ToolbarToggle>("event_toggle");
            eventToggle.value = currentViewType == ViewType.Event;
            var elementToggle = rootVisualElement.Q<ToolbarToggle>("element_toggle");
            elementToggle.value = currentViewType == ViewType.Element;
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
                currentViewType = viewType;
                EditorPrefs.SetInt("com.huyhung1404.gameflow.viewerViewType", (int)currentViewType);
            };
        }

        private void ListViewGUI()
        {
            if (!Application.isPlaying) return;

            evenStyle ??= new GUIStyle
            {
                normal = new GUIStyleState
                {
                    background = evenTexture
                }
            };

            oddStyle ??= new GUIStyle
            {
                normal = new GUIStyleState
                {
                    background = oddTexture
                }
            };
            labelStyle ??= new GUIStyle(GUI.skin.label)
            {
                richText = true
            };

            switch (currentViewType)
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
            switch (currentViewType)
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
            EditorGUILayout.LabelField(GetTitle(isCurrent, isWaitBuild, currentCommand).Trim(), GameFlowViewerEditorWindow.labelStyle);
            EditorGUILayout.Space(2);
            EditorGUILayout.TextArea(currentCommand.GetFullInfo(), GameFlowViewerEditorWindow.labelStyle);
            EditorGUILayout.EndScrollView();
        }

        private static void DrawElement(int index, Command command, bool isCurrent, bool isWaitBuild)
        {
            EditorGUILayout.BeginHorizontal(index % 2 == 0 ? GameFlowViewerEditorWindow.evenStyle : GameFlowViewerEditorWindow.oddStyle);
            EditorGUILayout.LabelField(GetTitle(isCurrent, isWaitBuild, command), GameFlowViewerEditorWindow.labelStyle);
            HandleMouseClick(GUILayoutUtility.GetLastRect());
            EditorGUILayout.EndHorizontal();
        }

        private static string GetTitle(bool isCurrent, bool isWaitBuild, Command command)
        {
            if (isWaitBuild) return $"<b><size=9>B     </size></b> {command.GetInfo()}";
            return isCurrent
                ? $"<b><size=9>C     </size></b> {command.GetInfo()}"
                : $"       {command.GetInfo()}";
        }

        private static void HandleMouseClick(Rect rect)
        {
            var e = Event.current;
            if (e.type != EventType.MouseDown || e.button != 0 || !rect.Contains(e.mousePosition)) return;
            GameFlowViewerEditorWindow.infoView.MarkDirtyRepaint();
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
            EditorGUILayout.LabelField(GetTitle(currentCallback is UIElementCallbackEvent, currentInfoType).Trim(), GameFlowViewerEditorWindow.labelStyle);
            EditorGUILayout.Space(2);
            EditorGUILayout.TextArea(currentCallback.ToString(), GameFlowViewerEditorWindow.labelStyle);
            EditorGUILayout.EndScrollView();
        }

        private static void DrawElement(int index, Type type, ElementCallbackEvent callbackEvent)
        {
            EditorGUILayout.BeginHorizontal(index % 2 == 0 ? GameFlowViewerEditorWindow.evenStyle : GameFlowViewerEditorWindow.oddStyle);
            callbackEvent.GetInfo(out var isUserInterface, out var eventCount);
            EditorGUILayout.LabelField(GetTitle(isUserInterface, type), GameFlowViewerEditorWindow.labelStyle);
            HandleMouseClick(GUILayoutUtility.GetLastRect(), type, callbackEvent);
            EditorGUILayout.LabelField($"Enable: {eventCount}", GameFlowViewerEditorWindow.labelStyle, GUILayout.Width(60));
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
            GameFlowViewerEditorWindow.infoView.MarkDirtyRepaint();
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
            EditorGUILayout.LabelField(GetTitle(currentElement is UserInterfaceFlowElement, currentElement).Trim(), GameFlowViewerEditorWindow.labelStyle);
            EditorGUILayout.Space(2);
            EditorGUILayout.TextArea(currentElement.GetFullInfo(), GameFlowViewerEditorWindow.labelStyle);
            EditorGUILayout.EndScrollView();
        }

        private static void DrawElement(int index, GameFlowElement element, bool isUI)
        {
            EditorGUILayout.BeginHorizontal(index % 2 == 0 ? GameFlowViewerEditorWindow.evenStyle : GameFlowViewerEditorWindow.oddStyle);
            EditorGUILayout.LabelField(GetTitle(isUI, element), GameFlowViewerEditorWindow.labelStyle);
            HandleMouseClick(GUILayoutUtility.GetLastRect());
            EditorGUILayout.EndHorizontal();
        }

        private static string GetTitle(bool isUserInterface, GameFlowElement element)
        {
            return isUserInterface
                ? $"<b><size=9>UI    </size></b> {element.GetInfo()}"
                : $"       {element.GetInfo()}";
        }

        private static void HandleMouseClick(Rect rect)
        {
            var e = Event.current;
            if (e.type != EventType.MouseDown || e.button != 0 || !rect.Contains(e.mousePosition)) return;
            GameFlowViewerEditorWindow.infoView.MarkDirtyRepaint();
        }
    }
}