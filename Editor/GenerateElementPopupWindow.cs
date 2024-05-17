using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public class GenerateElementPopupWindow : PopupWindowContent
    {
        private const string kUxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/GenerateElementPopupWindow.uxml";
        private const string kNamePattern = "^[a-zA-Z0-9]+$";

        private readonly bool isUserInterface;
        private readonly Action onClose;
        private readonly Regex nameRegex;
        private readonly List<Type> elementsInProject;

        private TextField textField;
        private bool errorName;
        private VisualElement elementTypeView;

        public GenerateElementPopupWindow(bool isUserInterface, Action onClose)
        {
            this.isUserInterface = isUserInterface;
            this.onClose = onClose;
            nameRegex = new Regex(kNamePattern);
            elementsInProject = GetDerivedTypes(typeof(GameFlowElement));
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(400, 300);
        }

        public override void OnGUI(Rect rect)
        {
        }

        public override void OnOpen()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(kUxmlPath);
            visualTreeAsset.CloneTree(editorWindow.rootVisualElement);
            DrawTitle();
            editorWindow.rootVisualElement.Q<IMGUIContainer>("log").onGUIHandler += OnLogGUI;
            textField = editorWindow.rootVisualElement.Q<TextField>("element_name");
            textField.RegisterCallback<ChangeEvent<string>>(NameChange);
            elementTypeView = editorWindow.rootVisualElement.Q<VisualElement>("template_view");
            elementTypeView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            HandleTemplateView();
        }

        private void DrawTitle()
        {
            editorWindow.rootVisualElement.Q<Label>("title").text = $"Generate {(isUserInterface ? "User Interface" : "Game")} Flow Element";
        }

        private void OnLogGUI()
        {
            if (errorName) EditorGUILayout.HelpBox("Element name is exits", MessageType.Error);
        }

        private void NameChange(ChangeEvent<string> evt)
        {
            if (string.IsNullOrEmpty(evt.newValue))
            {
                elementTypeView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                errorName = false;
                return;
            }

            if (nameRegex.IsMatch(evt.newValue))
            {
                if (elementsInProject.FindIndex(item => item.Name == evt.newValue) >= 0)
                {
                    elementTypeView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    errorName = true;
                    return;
                }

                elementTypeView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                errorName = false;
                return;
            }

            textField.value = evt.previousValue;
            errorName = false;
            elementTypeView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        public override void OnClose()
        {
            onClose.Invoke();
        }

        public static List<Type> GetDerivedTypes(Type baseType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var derivedTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                derivedTypes.AddRange(assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(baseType)));
            }

            return derivedTypes;
        }

        private void HandleTemplateView()
        {
            elementTypeView = editorWindow.rootVisualElement.Q<Toggle>("template_view");

        }
    }
}