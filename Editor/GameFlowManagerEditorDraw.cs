using UnityEditor;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace GameFlow.Editor
{
    public class GameFlowManagerEditorDraw
    {
        private const string UXML_PATH = "Packages/com.huyhung1404.gameflow/Editor/UXML/GameFlowManagerEditor.uxml";

        private readonly VisualElement root;

        public GameFlowManagerEditorDraw(VisualElement rootVisualElement)
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            VisualElement labelFromUXML = visualTree.Instantiate();
            rootVisualElement.Add(labelFromUXML);
            root = rootVisualElement;
            RegisterGenerateButton();
        }

        private void RegisterGenerateButton()
        {
            var addButton = root.Q<Button>("add_button");
            addButton.RegisterCallback<ClickEvent>(_ =>
            {
                AssetDatabase.Refresh();
                PopupWindow.Show(addButton.worldBound, new GenerateElementPopupWindow(false, RefreshList));
            });

            var addInterfaceButton = root.Q<Button>("add_interface_button");
            addInterfaceButton.RegisterCallback<ClickEvent>(_ =>
            {
                AssetDatabase.Refresh();
                PopupWindow.Show(addInterfaceButton.worldBound, new GenerateElementPopupWindow(true, RefreshList));
            });
        }

        private void RefreshList()
        {
        }
    }
}