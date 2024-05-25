using System;
using System.Collections.Generic;
using System.Reflection;
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
        public List<SerializedProperty> properties;
    }

    public class GameFlowManagerEditorDraw
    {
        private const string kUxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/GameFlowManagerEditor.uxml";
        private const string KNoElementText = "No elements were found in the manager.";
        private const string KHasElementTextFormat = "Element count:  <b>{0}</b>";

        private readonly VisualElement root;
        private readonly Generate generateAction;
        private readonly SerializedObject serializedObject;
        private readonly List<GameFlowVisualElement> gameFlowElements;
        private readonly Dictionary<Type, ElementProperty> gameFlowProperties;
        private readonly List<GameFlowVisualElement> userInterfaceFlowElements;
        private readonly Dictionary<Type, ElementProperty> userInterfaceFlowProperties;

        private Label gameFlowCountTitle;
        private Label userInterfaceFlowCountTitle;
        private VisualElement gameFlowContainer;
        private VisualElement userInterfaceContainer;
        private ToolbarSearchField searchField;

        public GameFlowManagerEditorDraw(VisualElement rootVisualElement, Generate generate)
        {
            generateAction = generate;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(kUxmlPath);
            VisualElement labelFromUXML = visualTree.Instantiate();
            rootVisualElement.Add(labelFromUXML);
            root = rootVisualElement;
            var manager = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
            if (manager.elementCollection.VerifyData())
            {
                EditorUtility.SetDirty(manager);
            }

            serializedObject = new SerializedObject(manager);
            gameFlowElements = new List<GameFlowVisualElement>();
            userInterfaceFlowElements = new List<GameFlowVisualElement>();
            gameFlowProperties = new Dictionary<Type, ElementProperty>();
            userInterfaceFlowProperties = new Dictionary<Type, ElementProperty>();
            Register();
            RegisterAddButton();
            RegisterGenerateButton();
            UpdateView();
        }

        private void Register()
        {
            gameFlowCountTitle = root.Q<Label>("game_flow_content");
            userInterfaceFlowCountTitle = root.Q<Label>("ui_flow_content");
            gameFlowContainer = root.Q<VisualElement>("game_flow_container");
            userInterfaceContainer = root.Q<VisualElement>("ui_flow_container");
            searchField = root.Q<ToolbarSearchField>("search_field");
            searchField.RegisterValueChangedCallback(_ => UpdateView());
        }

        private void RegisterAddButton()
        {
            var addButton = root.Q<Button>("add_button");
            addButton.RegisterCallback<ClickEvent>(_ => { PopupWindow.Show(addButton.worldBound, new AddElementPopupWindow(false, UpdateView)); });
            var addInterfaceButton = root.Q<Button>("add_interface_button");
            addInterfaceButton.RegisterCallback<ClickEvent>(_ => { PopupWindow.Show(addInterfaceButton.worldBound, new AddElementPopupWindow(true, UpdateView)); });
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

        internal void UpdateView()
        {
            UpdateData();
            gameFlowCountTitle.text = SetCountTitle(gameFlowProperties.Count);
            userInterfaceFlowCountTitle.text = SetCountTitle(userInterfaceFlowProperties.Count);
            FillData(false, gameFlowProperties, gameFlowElements, gameFlowContainer);
            FillData(true, userInterfaceFlowProperties, userInterfaceFlowElements, userInterfaceContainer);
        }

        private void UpdateData()
        {
            gameFlowProperties.Clear();
            userInterfaceFlowProperties.Clear();
            var property = serializedObject?.FindProperty("elementCollection").FindPropertyRelative("elements");
            var assembly = AppDomain.CurrentDomain.GetAssemblies();
            if (property == null || property.arraySize == 0) return;
            var searchKey = searchField.value;
            var hasSearchKey = !string.IsNullOrEmpty(searchKey);
            for (var i = 0; i < property.arraySize; i++)
            {
                var elementProperty = property.GetArrayElementAtIndex(i);
                var type = GetAssemblyType(assembly, elementProperty.managedReferenceFullTypename);
                var isUserInterface = type.IsSubclassOf(typeof(UserInterfaceFlowElement));
                var instanceIDProperty = elementProperty.FindPropertyRelative("instanceID");
                if (hasSearchKey)
                {
                    if (!type.Name.Contains(searchKey)
                        && (instanceIDProperty == null
                            || (instanceIDProperty.stringValue != null
                                && !instanceIDProperty.stringValue.Contains(searchKey)))) continue;
                }

                AddDictionaryProperty(type, elementProperty, !isUserInterface ? gameFlowProperties : userInterfaceFlowProperties);
            }
        }

        private static void AddDictionaryProperty(Type type, SerializedProperty serializedProperty, IDictionary<Type, ElementProperty> dictionary)
        {
            if (dictionary.TryGetValue(type, out var element))
            {
                element.properties.Add(serializedProperty);
                return;
            }

            dictionary.Add(type, new ElementProperty
            {
                properties = new List<SerializedProperty> { serializedProperty }
            });
        }

        private static Type GetAssemblyType(IEnumerable<Assembly> assemblies, string typeName)
        {
            var key = typeName.Split(" ");
            foreach (var assembly in assemblies)
            {
                var nameIndex = 0;
                if (key.Length == 2)
                {
                    if (!assembly.FullName.Contains(key[0])) continue;
                    nameIndex = 1;
                }

                var type = assembly.GetType(key[nameIndex]);
                if (type == null) continue;
                return type;
            }

            return null;
        }

        private string SetCountTitle(int count)
        {
            var countText = count switch
            {
                0 => KNoElementText,
                _ => string.Format(KHasElementTextFormat, count)
            };

            return string.IsNullOrEmpty(searchField.value) ? countText : $"[ Key: <b>{searchField.value}</b> ]  {countText}";
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
                visualElement.UpdateGraphic(isUserInterface, keyValue.Key, keyValue.Value);
                index++;
            }

            for (; index < elements.Count; index++)
            {
                elements[index].style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
        }
    }
}