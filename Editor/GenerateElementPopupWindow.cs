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
        private const string k_uxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/GenerateElementPopupWindow.uxml";
        private const string k_namePattern = "^[a-zA-Z0-9_]+$";

        private readonly bool _isUserInterface;
        private readonly Generate _generateAction;
        private readonly Regex _nameRegex;
        private readonly List<Type> _elementsInProject;
        private readonly List<string> _prefabTemplatePath;
        private readonly List<string> _prefabTemplateChoices;
        private readonly List<string> _sceneTemplatePath;
        private readonly List<string> _sceneTemplateChoices;

        private TextField _textField;
        private bool _existsElement;
        private VisualElement _elementTypeView;
        private VisualElement _instanceIdView;
        private bool _isScene;
        private RadioButton _sceneRadio;
        private RadioButton _prefabRadio;
        private DropdownField _template;
        private float _generateSizePopup;
        private float _idSizePopup;
        private float _logSizePopup;
        private bool _fileNameIsExists;

        public GenerateElementPopupWindow(bool isUserInterface, Generate generate)
        {
            this._isUserInterface = isUserInterface;
            _generateAction = generate;
            _nameRegex = new Regex(k_namePattern);
            _elementsInProject = GetDerivedTypes(typeof(GameFlowElement));
            _isScene = true;
            _prefabTemplatePath = SearchTemplate(isUserInterface ? "*UserInterfaceFlow" : "*GameFlow", ".prefab",
                out _prefabTemplateChoices);
            _sceneTemplatePath = SearchTemplate(isUserInterface ? "*UserInterfaceFlow" : "*GameFlow", ".unity",
                out _sceneTemplateChoices);
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(400, 60 + _generateSizePopup + _logSizePopup + _idSizePopup);
        }

        public override void OnGUI(Rect rect)
        {
        }

        public override void OnOpen()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_uxmlPath);
            visualTreeAsset.CloneTree(editorWindow.rootVisualElement);
            DrawTitle();
            editorWindow.rootVisualElement.Q<IMGUIContainer>("log").onGUIHandler += OnLogGUI;
            _textField = editorWindow.rootVisualElement.Q<TextField>("element_name");
            _textField.RegisterCallback<ChangeEvent<string>>(NameChange);
            _elementTypeView = editorWindow.rootVisualElement.Q<VisualElement>("template_view");
            _instanceIdView = editorWindow.rootVisualElement.Q<VisualElement>("instance_id_view");
            _idSizePopup = _generateSizePopup = 0;
            _elementTypeView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            _instanceIdView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            _template = editorWindow.rootVisualElement.Q<DropdownField>("template");
            _template.choices = _isScene ? _sceneTemplateChoices : _prefabTemplateChoices;
            _template.index = 0;
            editorWindow.rootVisualElement.Q<Button>("generate_button").RegisterCallback<ClickEvent>(GenerateButton);
            HandleTemplateView();
            _fileNameIsExists = false;
        }

        private void DrawTitle()
        {
            editorWindow.rootVisualElement.Q<Label>("title").text = $"Generate {(_isUserInterface ? "User Interface" : "Game")} Flow Element";
        }

        private void OnLogGUI()
        {
            _logSizePopup = 0;
            if (_fileNameIsExists)
            {
                _logSizePopup += 40;
                EditorGUILayout.HelpBox("Element name is exists.", MessageType.Error);
            }

            if ((_isScene ? _sceneTemplateChoices : _prefabTemplateChoices).Count == 0)
            {
                _logSizePopup += 40;
                EditorGUILayout.HelpBox("Element template is not exists.", MessageType.Error);
            }
        }

        private void NameChange(ChangeEvent<string> evt)
        {
            string newValue;
            string previousValue;
            if (evt == null)
            {
                newValue = _textField.value;
                previousValue = _textField.value;
            }
            else
            {
                newValue = evt.newValue;
                previousValue = evt.previousValue;
            }

            if (string.IsNullOrEmpty(newValue))
            {
                DisableAllView();
                return;
            }

            if (_nameRegex.IsMatch(newValue))
            {
                var scriptsName = string.Format(GameFlowManagerEditorWindow.k_scriptsElementNameFormat, newValue);
                var type = _elementsInProject.Find(type => type.Name == scriptsName);
                _existsElement = type != null;
                if (_existsElement)
                {
                    if ((_isUserInterface && type?.BaseType == typeof(UIFlowElement))
                        || (!_isUserInterface && type?.BaseType == typeof(GameFlowElement)))
                    {
                        _instanceIdView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                        _idSizePopup = 30;
                        _elementTypeView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                        _generateSizePopup = 0;
                        _fileNameIsExists = false;
                        return;
                    }

                    DisableAllView();
                    _fileNameIsExists = true;
                    return;
                }

                _fileNameIsExists = false;
                _elementTypeView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                _instanceIdView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                _generateSizePopup = 60;
                _idSizePopup = 0;
                return;
            }

            DisableAllView();
            _textField.value = previousValue;
        }

        private void DisableAllView()
        {
            _existsElement = false;
            _fileNameIsExists = false;
            _generateSizePopup = _idSizePopup = 0;
            _elementTypeView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            _instanceIdView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
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
                path.AddRange(Directory.GetFiles(_isUserInterface
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
            _sceneRadio = editorWindow.rootVisualElement.Q<RadioButton>("scene_radio");
            _sceneRadio.RegisterCallback<ClickEvent>(CallbackScene);

            _prefabRadio = editorWindow.rootVisualElement.Q<RadioButton>("prefab_radio");
            _prefabRadio.RegisterCallback<ClickEvent>(CallbackPrefab);
        }

        private void CallbackScene(ClickEvent evt)
        {
            _isScene = _sceneRadio.value;
            SetUpRadio();
        }

        private void CallbackPrefab(ClickEvent evt)
        {
            _isScene = !_prefabRadio.value;
            SetUpRadio();
        }

        private void SetUpRadio()
        {
            _sceneRadio.value = _isScene;
            _prefabRadio.value = !_isScene;
            _template.choices = _isScene ? _sceneTemplateChoices : _prefabTemplateChoices;
            _template.index = 0;
        }

        private void GenerateButton(ClickEvent evt)
        {
            var templateList = _isScene ? _sceneTemplatePath : _prefabTemplatePath;
            if (templateList.Count == 0) return;
            var templatePath = templateList[_template.index];
            editorWindow.Close();
            _generateAction.Invoke(_isUserInterface, _isScene, templatePath, _textField.value);
        }
    }
}