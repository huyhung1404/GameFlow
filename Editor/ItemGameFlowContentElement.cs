using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public class ItemGameFlowContentElement : VisualElement
    {
        public static bool IsDrawGUI;
        private const string kUxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/ItemGameFlowContentElement.uxml";
        private SerializedObject serializedObject;
        private SerializedProperty serializedProperty;
        private SerializedProperty includeInBuild;
        private SerializedProperty reference;
        private readonly EnumField releaseModeElement;
        private readonly Toggle fullSceneElement;
        private readonly Toggle canReActive;
        private Action<int> removeAtIndex;
        private bool showDialog;
        private bool isActive;
        private readonly Foldout container;

        public ItemGameFlowContentElement()
        {
            var root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(kUxmlPath).CloneTree();
            container = root.Q<Foldout>("container");
            root.Q<IMGUIContainer>("title_gui").onGUIHandler = DrawTitleGUI;
            root.Q<Button>("remove_button").RegisterCallback<ClickEvent>(OnClickRemove);
            releaseModeElement = root.Q<EnumField>("release_mode");
            fullSceneElement = root.Q<Toggle>("full_scene");
            canReActive = root.Q<Toggle>("canReActive");
            Add(root);
        }

        private void OnClickRemove(ClickEvent evt)
        {
            if (serializedProperty.ExtractArrayIndex() == null)
            {
                Debug.LogError("Not find array index");
                return;
            }

            showDialog = true;
        }

        private void DrawTitleGUI()
        {
            if (!IsDrawGUI || !isActive) return;
            var guiWidth = EditorGUIUtility.currentViewWidth;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            var lastValue = includeInBuild.boolValue;
            includeInBuild.boolValue = EditorGUI.Toggle(new Rect(0, 0, 20, 20), GUIContent.none, includeInBuild.boolValue);
            if (includeInBuild.boolValue != lastValue)
            {
                var referenceValue = reference.GetAssetReferenceValue();
                if (referenceValue == null)
                {
                    includeInBuild.boolValue = lastValue;
                }
                else
                {
                    AddressableUtility.AddAddressableGroupGUID(referenceValue.AssetGUID, !lastValue, false);
                }
            }

            var idWidth = Mathf.Max(30, guiWidth / 4);
            // instanceID.stringValue = EditorGUI.TextField(new Rect(22, 1, idWidth, 18), GUIContent.none, instanceID.stringValue);
            EditorGUI.PropertyField(new Rect(30 + idWidth, 1, Mathf.Max(45, guiWidth - idWidth - 135), 18), reference, GUIContent.none);
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
            if (!showDialog) return;
            ShowConfirmationDialog();
            showDialog = false;
        }

        private void ShowConfirmationDialog()
        {
            var index = serializedProperty.ExtractArrayIndex();
            if (index == null) return;
            var confirm = EditorUtility.DisplayDialog(
                "Remove element",
                "Are you sure you want to remove element?",
                "Yes",
                "No"
            );

            if (!confirm) return;
            IsDrawGUI = false;
            isActive = false;
            removeAtIndex?.Invoke(index.Value);
        }

        public void UpdateGraphic(bool isUserInterface, SerializedProperty serialized, Action<int> removeAt)
        {
            serializedProperty = serialized;
            removeAtIndex = removeAt;
            serializedObject = new SerializedObject(serializedProperty.objectReferenceValue);
            container.BindToViewDataKey(serializedProperty.propertyPath, false);
            includeInBuild = serializedObject.FindProperty(nameof(GameFlowElement.includeInBuild));
            reference = serializedObject.FindProperty(nameof(GameFlowElement.reference));
            releaseModeElement.BindProperty(serializedObject.FindProperty(nameof(GameFlowElement.releaseMode)));
            canReActive.BindProperty(serializedObject.FindProperty(nameof(GameFlowElement.canReActive)));
            if (isUserInterface)
            {
                fullSceneElement.BindProperty(serializedObject.FindProperty(nameof(UserInterfaceFlowElement.fullScene)));
                fullSceneElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            }
            else
            {
                fullSceneElement.Unbind();
                fullSceneElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }

            isActive = true;
        }

        public void HideGraphic()
        {
            isActive = false;
        }

        public new class UxmlFactory : UxmlFactory<ItemGameFlowContentElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription { get { yield break; } }
        }
    }
}