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
            const string kPrefsKey = "com.huyhung1404.gameflow_showTestScripts";
            elementsInProject = GetDerivedTypes(this.isUserInterface, EditorPrefs.GetBool(kPrefsKey, false));
            elementChoices = elementsInProject.Select(type => type.Name).ToList();
            manager = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
        }

        private static List<Type> GetDerivedTypes(bool isUserInterface, bool enableTestsScripts)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var derivedTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                derivedTypes.AddRange(assembly.GetTypes()
                    .Where(t => IsElementChild(t) && IsNotAbstract(t) && IsTestScripts(t) && IsSubClass(t)));
            }

            return derivedTypes;

            bool IsElementChild(Type t)
            {
                return t != typeof(UserInterfaceFlowElement) && t != typeof(GameFlowElement);
            }

            bool IsNotAbstract(Type t)
            {
                return t.IsClass && !t.IsAbstract;
            }

            bool IsTestScripts(Type t)
            {
                if (enableTestsScripts) return true;
                return !t.Name.Contains("TestScript___");
            }

            bool IsSubClass(Type t)
            {
                return isUserInterface
                    ? t.IsSubclassOf(typeof(UserInterfaceFlowElement))
                    : t.IsSubclassOf(typeof(GameFlowElement)) && !t.IsSubclassOf(typeof(UserInterfaceFlowElement));
            }
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
                // if (!string.IsNullOrEmpty(idField.value) && manager.elementCollection.GetElement(currentType, idField.value) == null)
                // {
                //     addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                //     buttonSizePopup = 20;
                // }
                // else
                // {
                //     addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                //     buttonSizePopup = 0;
                // }
            }
            else
            {
                instanceIdView.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                generateSizePopup = 0;
                addButton.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                buttonSizePopup = 20;
            }
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
            manager.elementCollection.GenerateElement(instance);
            EditorUtility.SetDirty(manager);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            resetView?.Invoke();
        }
    }
}