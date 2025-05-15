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
        private readonly EnumField activeModeElement;
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
            activeModeElement = root.Q<EnumField>("active_mode");
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

            EditorGUI.PropertyField(new Rect(20, 1, Mathf.Max(45, guiWidth - 175), 18), reference, GUIContent.none);
            if (GUI.Button(new Rect(guiWidth - 150, 1, 45, 18), new GUIContent("Ping")))
            {
                EditorGUIUtility.PingObject(serializedObject.targetObject);
            }

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

        public void UpdateGraphic(SerializedProperty serialized, Action<int> removeAt)
        {
            serializedProperty = serialized;
            removeAtIndex = removeAt;
            serializedObject = new SerializedObject(serializedProperty.objectReferenceValue);
            container.BindToViewDataKey(serializedProperty.propertyPath, false);
            includeInBuild = serializedObject.FindProperty(nameof(GameFlowElement.includeInBuild));
            reference = serializedObject.FindProperty(nameof(GameFlowElement.reference));
            releaseModeElement.BindProperty(serializedObject.FindProperty(nameof(GameFlowElement.releaseMode)));
            activeModeElement.BindProperty(serializedObject.FindProperty(nameof(GameFlowElement.activeMode)));
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