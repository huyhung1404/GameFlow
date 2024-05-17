﻿using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly List<string> prefabTemplatePath;
        private readonly List<string> prefabTemplateChoices;
        private readonly List<string> sceneTemplatePath;
        private readonly List<string> sceneTemplateChoices;

        private TextField textField;
        private bool errorName;
        private VisualElement elementTypeView;
        private bool isScene;
        private RadioButton sceneRadio;
        private RadioButton prefabRadio;
        private DropdownField template;

        public GenerateElementPopupWindow(bool isUserInterface, Action onClose)
        {
            this.isUserInterface = isUserInterface;
            this.onClose = onClose;
            nameRegex = new Regex(kNamePattern);
            elementsInProject = GetDerivedTypes(typeof(GameFlowElement));
            isScene = true;
            prefabTemplatePath = SearchTemplate(isUserInterface ? "*UserInterfaceFlow" : "*GameFlow", isUserInterface ? "UserInterfaceFlowElements" : "GameFlowElements", ".prefab",
                out prefabTemplateChoices);
            sceneTemplatePath = SearchTemplate(isUserInterface ? "*UserInterfaceFlow" : "*GameFlow", isUserInterface ? "UserInterfaceFlowElements" : "GameFlowElements", ".unity",
                out sceneTemplateChoices);
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(400, 125);
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
            template = editorWindow.rootVisualElement.Q<DropdownField>("template");
            template.choices = isScene ? sceneTemplateChoices : prefabTemplateChoices;
            template.index = 0;
            editorWindow.rootVisualElement.Q<Button>("generate_button").RegisterCallback<ClickEvent>(GenerateButton);
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
            try
            {
                onClose?.Invoke();
            }
            catch (Exception)
            {
                //
            }
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

        private static List<string> SearchTemplate(string templatePattern, string folder, string extension, out List<string> choices)
        {
            var path = new List<string>();
            try
            {
                path.AddRange(Directory.GetFiles(Application.dataPath + "/GameFlow/Templates", templatePattern + "*" + extension, SearchOption.AllDirectories));
            }
            catch (Exception)
            {
                //Ignore
            }

            try
            {
                path.AddRange(Directory.GetFiles(Application.dataPath + "/GameFlow/Elements/" + folder, "*" + extension, SearchOption.AllDirectories));
            }
            catch (Exception)
            {
                //Ignore
            }

            choices = new List<string>(path);
            for (var i = 0; i < choices.Count; i++)
            {
                choices[i] = Path.GetFileName(choices[i]);
            }

            return path;
        }

        private void HandleTemplateView()
        {
            sceneRadio = editorWindow.rootVisualElement.Q<RadioButton>("scene_radio");
            sceneRadio.RegisterCallback<ClickEvent>(CallbackScene);

            prefabRadio = editorWindow.rootVisualElement.Q<RadioButton>("prefab_radio");
            prefabRadio.RegisterCallback<ClickEvent>(CallbackPrefab);
        }

        private void CallbackScene(ClickEvent evt)
        {
            isScene = sceneRadio.value;
            SetUpRadio();
        }

        private void CallbackPrefab(ClickEvent evt)
        {
            isScene = !prefabRadio.value;
            SetUpRadio();
        }

        private void SetUpRadio()
        {
            sceneRadio.value = isScene;
            prefabRadio.value = !isScene;
            template.choices = isScene ? sceneTemplateChoices : prefabTemplateChoices;
            template.index = 0;
        }
        
        private void GenerateButton(ClickEvent evt)
        {
            
        }
    }
}