using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
#endif
    public partial class ItemGameFlowContentElement : VisualElement
    {
        internal static bool s_IsDrawGUI;
        private const string k_uxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/ItemGameFlowContentElement.uxml";
        private SerializedObject _serializedObject;
        private SerializedProperty _serializedProperty;
        private SerializedProperty _includeInBuild;
        private SerializedProperty _reference;
        private readonly EnumField _releaseModeElement;
        private readonly EnumField _activeModeElement;
        private Action<int> _removeAtIndex;
        private bool _showDialog;
        private bool _isActive;
        private readonly Foldout _container;

        public ItemGameFlowContentElement()
        {
            var root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_uxmlPath).CloneTree();
            _container = root.Q<Foldout>("container");
            root.Q<IMGUIContainer>("title_gui").onGUIHandler = DrawTitleGUI;
            root.Q<Button>("remove_button").RegisterCallback<ClickEvent>(OnClickRemove);
            _releaseModeElement = root.Q<EnumField>("release_mode");
            _activeModeElement = root.Q<EnumField>("active_mode");
            Add(root);
        }

        private void OnClickRemove(ClickEvent evt)
        {
            if (_serializedProperty.ExtractArrayIndex() == null)
            {
                Debug.LogError("Not find array index");
                return;
            }

            _showDialog = true;
        }

        private void DrawTitleGUI()
        {
            if (!s_IsDrawGUI || !_isActive) return;
            var guiWidth = EditorGUIUtility.currentViewWidth;
            _serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            var lastValue = _includeInBuild.boolValue;
            _includeInBuild.boolValue = EditorGUI.Toggle(new Rect(0, 0, 20, 20), GUIContent.none, _includeInBuild.boolValue);
            if (_includeInBuild.boolValue != lastValue)
            {
                var referenceValue = _reference.GetAssetReferenceValue();
                if (referenceValue == null)
                {
                    _includeInBuild.boolValue = lastValue;
                }
                else
                {
                    AddressableUtility.AddAddressableGroupGUID(referenceValue.AssetGUID, !lastValue, false);
                }
            }

            EditorGUI.PropertyField(new Rect(20, 1, Mathf.Max(45, guiWidth - 175), 18), _reference, GUIContent.none);
            if (GUI.Button(new Rect(guiWidth - 150, 1, 45, 18), new GUIContent("Ping")))
            {
                EditorGUIUtility.PingObject(_serializedObject.targetObject);
            }

            if (EditorGUI.EndChangeCheck()) _serializedObject.ApplyModifiedProperties();
            if (!_showDialog) return;
            ShowConfirmationDialog();
            _showDialog = false;
        }

        private void ShowConfirmationDialog()
        {
            var index = _serializedProperty.ExtractArrayIndex();
            if (index == null) return;
            var confirm = EditorUtility.DisplayDialog(
                "Remove element",
                "Are you sure you want to remove element?",
                "Yes",
                "No"
            );

            if (!confirm) return;
            s_IsDrawGUI = false;
            _isActive = false;
            _removeAtIndex?.Invoke(index.Value);
        }

        public void UpdateGraphic(SerializedProperty serialized, Action<int> removeAt)
        {
            _serializedProperty = serialized;
            _removeAtIndex = removeAt;
            _serializedObject = new SerializedObject(_serializedProperty.objectReferenceValue);
            _container.BindToViewDataKey(_serializedProperty.propertyPath, false);
            _includeInBuild = _serializedObject.FindProperty(nameof(GameFlowElement.includeInBuild));
            _reference = _serializedObject.FindProperty(nameof(GameFlowElement.reference));
            _releaseModeElement.BindProperty(_serializedObject.FindProperty(nameof(GameFlowElement.releaseMode)));
            _activeModeElement.BindProperty(_serializedObject.FindProperty(nameof(GameFlowElement.activeMode)));
            _isActive = true;
        }

        public void HideGraphic()
        {
            _isActive = false;
        }
#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<ItemGameFlowContentElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription { get { yield break; } }
        }
#endif
    }
}