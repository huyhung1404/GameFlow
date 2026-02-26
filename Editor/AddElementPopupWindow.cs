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
        private const string k_uxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/AddElementPopupWindow.uxml";
        private readonly Action _resetView;
        private readonly bool _isUserInterface;
        private readonly GameFlowManager _manager;
        private Button _addButton;
        private VisualElement _debugViewButton;
        private float _debugSizePopup;
        private float _buttonSizePopup;
        private ObjectField _objectField;

        public AddElementPopupWindow(bool isUserInterface, Action reset)
        {
            _isUserInterface = isUserInterface;
            _resetView = reset;
            _manager = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(400, 65 + _buttonSizePopup + _debugSizePopup);
        }

        public override void OnGUI(Rect rect)
        {
        }

        public override void OnOpen()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_uxmlPath);
            visualTreeAsset.CloneTree(editorWindow.rootVisualElement);
            editorWindow.rootVisualElement.Q<Label>("title").text = $"Add {(_isUserInterface ? "User Interface" : "Game")} Flow Element";
            _objectField = editorWindow.rootVisualElement.Q<ObjectField>("instance");
            _objectField.objectType = _isUserInterface ? typeof(UIFlowElement) : typeof(GameFlowElement);
            _objectField.RegisterValueChangedCallback(ObjectCallback);
            _debugViewButton = editorWindow.rootVisualElement.Q<VisualElement>("element_exits");
            _debugViewButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            _addButton = editorWindow.rootVisualElement.Q<Button>("add_button");
            _addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            _addButton.RegisterCallback<ClickEvent>(AddButton);
        }

        private void ObjectCallback(ChangeEvent<Object> evt)
        {
            if (evt.newValue == null)
            {
                _debugViewButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                _addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                _buttonSizePopup = _debugSizePopup = 0;
                return;
            }

            if (_manager.elementCollection.TryGetElement(evt.newValue.GetType(), out _))
            {
                _debugViewButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                _addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                _buttonSizePopup = 0;
                _debugSizePopup = 30;
                return;
            }

            _debugViewButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            _addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            _debugSizePopup = 0;
            _buttonSizePopup = 20;
        }

        private void AddButton(ClickEvent evt)
        {
            AddElement();
            editorWindow.Close();
        }

        private void AddElement()
        {
            _manager.elementCollection.GenerateElement((GameFlowElement)_objectField.value);
            EditorUtility.SetDirty(_manager);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            _resetView?.Invoke();
        }
    }
}