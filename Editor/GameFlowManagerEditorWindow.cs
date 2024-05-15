using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameFlowManagerEditorWindow : EditorWindow
{
    private const string UXML_PATH = "Packages/com.huyhung1404.gameflow/Editor/UXML/GameFlowManagerEditor.uxml";

    public static void OpenWindow()
    {
        var window = GetWindow<GameFlowManagerEditorWindow>();
        window.titleContent = new GUIContent("Game Flow Manager");
    }

    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
        VisualElement labelFromUXML = visualTree.Instantiate();
        rootVisualElement.Add(labelFromUXML);
    }
}