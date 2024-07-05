using System;
using GameFlow.Internal;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GameFlow.Editor
{
    public class AddElementPopupWindow : PopupWindowContent
    {
        private const string kUxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/AddElementPopupWindow.uxml";
        private readonly Action resetView;
        private readonly bool isUserInterface;
        private readonly GameFlowManager manager;
        private Button addButton;
        private VisualElement debugViewButton;
        private float debugSizePopup;
        private float buttonSizePopup;
        private ObjectField objectField;

        public AddElementPopupWindow(bool isUserInterface, Action reset)
        {
            this.isUserInterface = isUserInterface;
            resetView = reset;
            manager = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(400, 65 + buttonSizePopup + debugSizePopup);
        }

        public override void OnGUI(Rect rect)
        {
        }

        public override void OnOpen()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(kUxmlPath);
            visualTreeAsset.CloneTree(editorWindow.rootVisualElement);
            editorWindow.rootVisualElement.Q<Label>("title").text = $"Add {(isUserInterface ? "User Interface" : "Game")} Flow Element";
            objectField = editorWindow.rootVisualElement.Q<ObjectField>("instance");
            objectField.objectType = isUserInterface ? typeof(UIFlowElement) : typeof(GameFlowElement);
            objectField.RegisterValueChangedCallback(ObjectCallback);
            debugViewButton = editorWindow.rootVisualElement.Q<VisualElement>("element_exits");
            debugViewButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            addButton = editorWindow.rootVisualElement.Q<Button>("add_button");
            addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            addButton.RegisterCallback<ClickEvent>(AddButton);
        }

        private void ObjectCallback(ChangeEvent<Object> evt)
        {
            if (evt.newValue == null)
            {
                debugViewButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                buttonSizePopup = debugSizePopup = 0;
                return;
            }

            if (manager.elementCollection.TryGetElement(evt.newValue.GetType(), out _))
            {
                debugViewButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                buttonSizePopup = 0;
                debugSizePopup = 30;
                return;
            }

            debugViewButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            debugSizePopup = 0;
            buttonSizePopup = 20;
        }

        private void AddButton(ClickEvent evt)
        {
            AddElement();
            editorWindow.Close();
        }

        private void AddElement()
        {
            manager.elementCollection.GenerateElement((GameFlowElement)objectField.value);
            EditorUtility.SetDirty(manager);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            resetView?.Invoke();
        }
    }
}