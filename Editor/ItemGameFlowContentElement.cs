using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public class ItemGameFlowContentElement : VisualElement
    {
        private const string kUxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/ItemGameFlowContentElement.uxml";
        private SerializedObject serializedObject;
        private SerializedProperty includeInBuild;
        private SerializedProperty instanceID;
        private SerializedProperty reference;
        private bool active;
        private readonly EnumField releaseModeElement;
        private readonly Toggle fullSceneElement;

        public ItemGameFlowContentElement()
        {
            var root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(kUxmlPath).CloneTree();
            root.Q<IMGUIContainer>("title_gui").onGUIHandler = DrawTitleGUI;
            root.Q<Button>("remove_button").RegisterCallback<ClickEvent>(OnClickRemove);
            releaseModeElement = root.Q<EnumField>("release_mode");
            fullSceneElement = root.Q<Toggle>("full_scene");
            Add(root);
        }

        private void OnClickRemove(ClickEvent evt)
        {
        }

        private void DrawTitleGUI()
        {
            if (!active) return;
            var guiWidth = EditorGUIUtility.currentViewWidth;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            var lastValue = includeInBuild.boolValue;
            includeInBuild.boolValue = EditorGUI.Toggle(new Rect(0, 0, 20, 20), GUIContent.none, includeInBuild.boolValue);
            if (includeInBuild.boolValue != lastValue)
            {
                var referenceValue = reference.GetValue<AssetReference>();
                if (referenceValue == null)
                {
                    includeInBuild.boolValue = lastValue;
                }
                else
                {
                    AddressableUtility.AddAddressableGroupGUID(referenceValue.AssetGUID, !lastValue);
                }
            }

            var idWidth = Mathf.Max(30, guiWidth / 4);
            instanceID.stringValue = EditorGUI.TextField(new Rect(22, 1, idWidth, 18), GUIContent.none, instanceID.stringValue);
            EditorGUI.PropertyField(new Rect(30 + idWidth, 1, Mathf.Max(45, guiWidth - idWidth - 125), 18), reference, GUIContent.none);
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
        }

        public void UpdateGraphic(bool isUserInterface, Type type, SerializedProperty serializedProperty)
        {
            serializedObject = serializedProperty.serializedObject;
            includeInBuild = serializedProperty.FindPropertyRelative("includeInBuild");
            instanceID = serializedProperty.FindPropertyRelative("instanceID");
            reference = serializedProperty.FindPropertyRelative("reference");
            releaseModeElement.BindProperty(serializedProperty.FindPropertyRelative("releaseMode"));
            if (isUserInterface)
            {
                fullSceneElement.BindProperty(serializedProperty.FindPropertyRelative("fullScene"));
                fullSceneElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            }
            else
            {
                fullSceneElement.Unbind();
                fullSceneElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }

            active = true;
        }

        public new class UxmlFactory : UxmlFactory<ItemGameFlowContentElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }
    }
}