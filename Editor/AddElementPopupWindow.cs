using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GameFlow.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public class AddElementPopupWindow : PopupWindowContent
    {
        private const string kUxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/AddElementPopupWindow.uxml";
        private const string kNamePattern = "^[a-zA-Z0-9_]+$";

        private readonly Action resetView;
        private readonly bool isUserInterface;
        private readonly Regex nameRegex;
        private readonly List<Type> elementsInProject;
        private readonly List<string> elementChoices;
        private readonly GameFlowManager manager;
        private DropdownField scripts;
        private TextField idField;
        private Button addButton;
        private VisualElement instanceIdView;
        private float generateSizePopup;
        private float idSizePopup;
        private float buttonSizePopup;

        public AddElementPopupWindow(bool isUserInterface, Action reset)
        {
            this.isUserInterface = isUserInterface;
            resetView = reset;
            nameRegex = new Regex(kNamePattern);
            elementsInProject = GetDerivedTypes(this.isUserInterface);
            elementChoices = elementsInProject.Select(type => type.Name).ToList();
            manager = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
        }

        private static List<Type> GetDerivedTypes(bool isUserInterface)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var derivedTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                derivedTypes.AddRange(assembly.GetTypes()
                    .Where(t => t != typeof(UserInterfaceFlowElement)
                                && t != typeof(GameFlowElement)
                                && t.IsClass
                                && !t.IsAbstract
                                && (isUserInterface
                                    ? t.IsSubclassOf(typeof(UserInterfaceFlowElement))
                                    : t.IsSubclassOf(typeof(GameFlowElement)) && !t.IsSubclassOf(typeof(UserInterfaceFlowElement)))));
            }

            return derivedTypes;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(400, 65 + generateSizePopup + buttonSizePopup + idSizePopup);
        }

        public override void OnGUI(Rect rect)
        {
        }

        public override void OnOpen()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(kUxmlPath);
            visualTreeAsset.CloneTree(editorWindow.rootVisualElement);
            editorWindow.rootVisualElement.Q<Label>("title").text = $"Add {(isUserInterface ? "User Interface" : "Game")} Flow Element";
            if (elementsInProject.Count == 0) return;
            editorWindow.rootVisualElement.Q<Label>("none").style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            scripts = editorWindow.rootVisualElement.Q<DropdownField>("scripts");
            scripts.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            scripts.choices = elementChoices;
            scripts.index = 0;
            scripts.RegisterValueChangedCallback(ScriptChange);
            instanceIdView = editorWindow.rootVisualElement.Q<VisualElement>("instance_id_view");
            idField = editorWindow.rootVisualElement.Q<TextField>("instance_id");
            idField.RegisterCallback<ChangeEvent<string>>(IDChange);
            addButton = editorWindow.rootVisualElement.Q<Button>("add_button");
            addButton.RegisterCallback<ClickEvent>(AddButton);
            SetUpView();
        }

        private void ScriptChange(ChangeEvent<string> evt)
        {
            SetUpView();
        }

        private void SetUpView()
        {
            var currentType = elementsInProject[scripts.index];
            if (manager.elementCollection.GetElement(currentType) != null)
            {
                instanceIdView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                generateSizePopup = 45;
                if (!string.IsNullOrEmpty(idField.value) && manager.elementCollection.GetElement(currentType, idField.value) == null)
                {
                    addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    buttonSizePopup = 20;
                }
                else
                {
                    addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    buttonSizePopup = 0;
                }
            }
            else
            {
                idField.value = null;
                instanceIdView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                generateSizePopup = 0;
                addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                buttonSizePopup = 20;
            }
        }

        private void IDChange(ChangeEvent<string> evt)
        {
            if (string.IsNullOrEmpty(evt.newValue) || nameRegex.IsMatch(evt.newValue))
            {
                SetUpView();
                return;
            }

            idField.value = evt.previousValue;
        }

        private void AddButton(ClickEvent evt)
        {
            GenerateScripts();
            editorWindow.Close();
        }

        private void GenerateScripts()
        {
            var instance = (GameFlowElement)Activator.CreateInstance(elementsInProject[scripts.index]);
            instance.includeInBuild = true;
            instance.releaseMode = ElementReleaseMode.RELEASE_ON_CLOSE;
            if (!string.IsNullOrEmpty(idField.value)) instance.instanceID = idField.value;
            manager.elementCollection.GenerateElement(instance);
            EditorUtility.SetDirty(manager);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            resetView?.Invoke();
        }
    }
}