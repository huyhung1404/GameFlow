using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GameFlow.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public delegate void Generate(bool isUserInterface, bool isScene, string templatePath, string elementName);

    public class GenerateElementPopupWindow : PopupWindowContent
    {
        private const string kUxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/GenerateElementPopupWindow.uxml";
        private const string kNamePattern = "^[a-zA-Z0-9]+$";

        private readonly bool isUserInterface;
        private readonly Generate generateAction;
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

        public GenerateElementPopupWindow(bool isUserInterface, Generate generate)
        {
            this.isUserInterface = isUserInterface;
            generateAction = generate;
            nameRegex = new Regex(kNamePattern);
            elementsInProject = GetDerivedTypes(typeof(GameFlowElement));
            isScene = true;
            prefabTemplatePath = SearchTemplate(isUserInterface ? "*UserInterfaceFlow" : "*GameFlow", ".prefab",
                out prefabTemplateChoices);
            sceneTemplatePath = SearchTemplate(isUserInterface ? "*UserInterfaceFlow" : "*GameFlow", ".unity",
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
            if (errorName)
            {
                EditorGUILayout.HelpBox("Element name is exits", MessageType.Error);
                return;
            }

            if ((isScene ? sceneTemplateChoices : prefabTemplateChoices).Count == 0) EditorGUILayout.HelpBox("Element template is not exits", MessageType.Error);
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

        private List<string> SearchTemplate(string templatePattern, string extension, out List<string> choices)
        {
            var path = new List<string>();
            try
            {
                path.AddRange(Directory.GetFiles(PackagePath.ProjectTemplatesPath(PackagePath.PathType.FullPath), templatePattern + "*" + extension, SearchOption.AllDirectories));
            }
            catch (Exception)
            {
                //Ignore
            }

            try
            {
                path.AddRange(Directory.GetFiles(isUserInterface 
                    ? PackagePath.AssetsUserInterfaceElementsFolderPath(PackagePath.PathType.FullPath) 
                    : PackagePath.AssetsElementsFolderPath(PackagePath.PathType.FullPath), "*" + extension, SearchOption.AllDirectories));
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
            var templateList = isScene ? sceneTemplatePath : prefabTemplatePath;
            if (templateList.Count == 0) return;
            var templatePath = templateList[template.index];
            editorWindow.Close();
            generateAction.Invoke(isUserInterface, isScene, templatePath, textField.value);
        }
    }
}