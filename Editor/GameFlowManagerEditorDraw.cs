using System;
using System.Collections.Generic;
using GameFlow.Internal;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace GameFlow.Editor
{
    public class ElementProperty
    {
        public List<SerializedProperty> Properties;
    }

    public class GameFlowManagerEditorDraw
    {
        private const string k_uxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/GameFlowManagerEditor.uxml";
        private const string k_noElementText = "No elements were found in the manager.";
        private const string k_hasElementTextFormat = "Element count:  <b>{0}</b>";

        private readonly VisualElement _root;
        private readonly Generate _generateAction;
        private readonly SerializedObject _serializedObject;
        private readonly List<GameFlowVisualElement> _gameFlowElements;
        private readonly Dictionary<Type, ElementProperty> _gameFlowProperties;
        private readonly List<GameFlowVisualElement> _userInterfaceFlowElements;
        private readonly Dictionary<Type, ElementProperty> _userInterfaceFlowProperties;

        private Label _gameFlowCountTitle;
        private Label _userInterfaceFlowCountTitle;
        private VisualElement _gameFlowContainer;
        private VisualElement _userInterfaceContainer;
        private ToolbarSearchField _searchField;

        public GameFlowManagerEditorDraw(VisualElement rootVisualElement, Generate generate)
        {
            _generateAction = generate;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_uxmlPath);
            VisualElement labelFromUXML = visualTree.Instantiate();
            rootVisualElement.Add(labelFromUXML);
            _root = rootVisualElement;
            var manager = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
            if (manager.elementCollection.VerifyData())
            {
                EditorUtility.SetDirty(manager);
            }

            _serializedObject = new SerializedObject(manager);
            _gameFlowElements = new List<GameFlowVisualElement>();
            _userInterfaceFlowElements = new List<GameFlowVisualElement>();
            _gameFlowProperties = new Dictionary<Type, ElementProperty>();
            _userInterfaceFlowProperties = new Dictionary<Type, ElementProperty>();
            Register();
            RegisterAddButton();
            RegisterGenerateButton();
            UpdateView();
        }

        private void Register()
        {
            foreach (var foldout in _root.Query<Foldout>("sub_element_title").ToList())
            {
                foldout?.BindToViewDataKey($"title_{foldout.text}");
            }

            _gameFlowCountTitle = _root.Q<Label>("game_flow_content");
            _userInterfaceFlowCountTitle = _root.Q<Label>("ui_flow_content");
            _gameFlowContainer = _root.Q<VisualElement>("game_flow_container");
            _userInterfaceContainer = _root.Q<VisualElement>("ui_flow_container");
            _searchField = _root.Q<ToolbarSearchField>("search_field");
            _root.Q<IntegerField>("plane_distance").BindProperty(_serializedObject.FindProperty(nameof(GameFlowManager.planeDistance)));
            _root.Q<IntegerField>("sorting_order_offset").BindProperty(_serializedObject.FindProperty(nameof(GameFlowManager.sortingOrderOffset)));
            _root.Q<IntegerField>("loading_shield_sorting_order").BindProperty(_serializedObject.FindProperty(nameof(GameFlowManager.loadingShieldSortingOrder)));
            _root.Q<Vector2Field>("reference_resolution").BindProperty(_serializedObject.FindProperty(nameof(GameFlowManager.referenceResolution)));
            _searchField.RegisterValueChangedCallback(_ => UpdateView());
        }

        private void RegisterAddButton()
        {
            var addButton = _root.Q<Button>("add_button");
            addButton.RegisterCallback<ClickEvent>(_ => { PopupWindow.Show(addButton.worldBound, new AddElementPopupWindow(false, UpdateView)); });
            var addInterfaceButton = _root.Q<Button>("add_interface_button");
            addInterfaceButton.RegisterCallback<ClickEvent>(_ => { PopupWindow.Show(addInterfaceButton.worldBound, new AddElementPopupWindow(true, UpdateView)); });
        }


        private void RegisterGenerateButton()
        {
            var generateButton = _root.Q<Button>("generate_button");
            generateButton.RegisterCallback<ClickEvent>(_ =>
            {
                AssetDatabase.Refresh();
                PopupWindow.Show(generateButton.worldBound, new GenerateElementPopupWindow(false, _generateAction));
            });

            var generateInterfaceButton = _root.Q<Button>("generate_interface_button");
            generateInterfaceButton.RegisterCallback<ClickEvent>(_ =>
            {
                AssetDatabase.Refresh();
                PopupWindow.Show(generateInterfaceButton.worldBound, new GenerateElementPopupWindow(true, _generateAction));
            });
        }

        internal void UpdateView()
        {
            ItemGameFlowContentElement.s_IsDrawGUI = false;
            UpdateData();
            _gameFlowCountTitle.text = SetCountTitle(_gameFlowProperties.Count);
            _userInterfaceFlowCountTitle.text = SetCountTitle(_userInterfaceFlowProperties.Count);
            FillData(false, _gameFlowProperties, _gameFlowElements, _gameFlowContainer);
            FillData(true, _userInterfaceFlowProperties, _userInterfaceFlowElements, _userInterfaceContainer);
            ItemGameFlowContentElement.s_IsDrawGUI = true;
        }

        private void UpdateData()
        {
            _serializedObject.Update();
            _gameFlowProperties.Clear();
            _userInterfaceFlowProperties.Clear();
            var property = _serializedObject?.FindProperty(nameof(GameFlowManager.elementCollection)).FindPropertyRelative("elements");
            if (property == null || property.arraySize == 0) return;
            var searchKey = _searchField.value;
            var hasSearchKey = !string.IsNullOrEmpty(searchKey);
            for (var i = 0; i < property.arraySize; i++)
            {
                var elementProperty = property.GetArrayElementAtIndex(i);
                if (elementProperty.objectReferenceValue == null) continue;
                var type = elementProperty.objectReferenceValue.GetType();
                var isUserInterface = type.IsSubclassOf(typeof(UIFlowElement));
                if (hasSearchKey)
                {
                    if (!type.Name.Contains(searchKey)) continue;
                }

                AddDictionaryProperty(type, elementProperty, !isUserInterface ? _gameFlowProperties : _userInterfaceFlowProperties);
            }
        }

        private static void AddDictionaryProperty(Type type, SerializedProperty serializedProperty, IDictionary<Type, ElementProperty> dictionary)
        {
            if (dictionary.TryGetValue(type, out var element))
            {
                element.Properties.Add(serializedProperty);
                return;
            }

            dictionary.Add(type, new ElementProperty
            {
                Properties = new List<SerializedProperty> { serializedProperty }
            });
        }

        private string SetCountTitle(int count)
        {
            var countText = count switch
            {
                0 => k_noElementText,
                _ => string.Format(k_hasElementTextFormat, count)
            };

            return string.IsNullOrEmpty(_searchField.value) ? countText : $"[ Key: <b>{_searchField.value}</b> ]  {countText}";
        }

        private void FillData(bool isUserInterface, Dictionary<Type, ElementProperty> properties, List<GameFlowVisualElement> elements, VisualElement container)
        {
            var index = 0;
            foreach (var keyValue in properties)
            {
                if (index >= elements.Count)
                {
                    var visual = new GameFlowVisualElement();
                    container.hierarchy.Add(visual);
                    elements.Add(visual);
                }

                var visualElement = elements[index];
                visualElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                visualElement.UpdateGraphic(isUserInterface, keyValue.Key, keyValue.Value, RemoveAt);
                index++;
            }

            for (; index < elements.Count; index++)
            {
                elements[index].style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
        }

        private void RemoveAt(int index)
        {
            try
            {
                _serializedObject?.FindProperty(nameof(GameFlowManager.elementCollection)).FindPropertyRelative("elements").DeleteArrayElementAtIndex(index);
                _serializedObject?.ApplyModifiedProperties();
                UpdateView();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}