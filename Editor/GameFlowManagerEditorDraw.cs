using UnityEditor;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace GameFlow.Editor
{
    public class GameFlowManagerEditorDraw
    {
        private const string UXML_PATH = "Packages/com.huyhung1404.gameflow/Editor/UXML/GameFlowManagerEditor.uxml";

        private readonly VisualElement root;
        private readonly Generate generateAction;

        public GameFlowManagerEditorDraw(VisualElement rootVisualElement, Generate generate)
        {
            generateAction = generate;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            VisualElement labelFromUXML = visualTree.Instantiate();
            rootVisualElement.Add(labelFromUXML);
            root = rootVisualElement;
            RegisterAddButton();
            RegisterGenerateButton();
        }

        private void RegisterAddButton()
        {
            var addButton = root.Q<Button>("add_button");
            addButton.RegisterCallback<ClickEvent>(_ => { PopupWindow.Show(addButton.worldBound, new AddElementPopupWindow(false, ResetView)); });
            var addInterfaceButton = root.Q<Button>("add_interface_button");
            addInterfaceButton.RegisterCallback<ClickEvent>(_ => { PopupWindow.Show(addInterfaceButton.worldBound, new AddElementPopupWindow(true, ResetView)); });
        }


        private void RegisterGenerateButton()
        {
            var generateButton = root.Q<Button>("generate_button");
            generateButton.RegisterCallback<ClickEvent>(_ =>
            {
                AssetDatabase.Refresh();
                PopupWindow.Show(generateButton.worldBound, new GenerateElementPopupWindow(false, generateAction));
            });

            var generateInterfaceButton = root.Q<Button>("generate_interface_button");
            generateInterfaceButton.RegisterCallback<ClickEvent>(_ =>
            {
                AssetDatabase.Refresh();
                PopupWindow.Show(generateInterfaceButton.worldBound, new GenerateElementPopupWindow(true, generateAction));
            });
        }

        private void ResetView()
        {
            //TODO: Reset View
        }
    }
}