using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameFlowManagerEditorWindow : EditorWindow
{
    public static void OpenWindow()
    {
        var window = GetWindow<GameFlowManagerEditorWindow>();
        window.titleContent = new GUIContent("Game Flow Manager");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.huyhung1404.gameflow/Editor/UXML/GameFlowManagerEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.huyhung1404.gameflow/Editor/USS/GameFlowManagerEditor.uss");
        VisualElement labelWithStyle = new Label("Hello World! With Style");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);
    }
}