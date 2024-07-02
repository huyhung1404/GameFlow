using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public class GameFlowViewerEditorWindow : EditorWindow
    {
        private const string kUxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/GameFlowViewerEditorWindow.uxml";

        public static void OpenWindow()
        {
            var window = GetWindow<GameFlowViewerEditorWindow>();
            window.titleContent = new GUIContent("Game Flow Event Viewer");
            window.minSize = new Vector2(630, 250);
            window.Show();
        }

        private void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(kUxmlPath);
            VisualElement treeFromUXML = visualTree.Instantiate();
            treeFromUXML.style.height = Length.Percent(100);
            rootVisualElement.Add(treeFromUXML);
        }
    }
}