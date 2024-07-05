using GameFlow.Component;
using UnityEditor;
using UnityEngine;

namespace GameFlow.Editor
{
    [CustomEditor(typeof(GameFlowUICanvas), true)]
    public class GameFlowUICanvasEditor : UnityEditor.Editor
    {
        protected string[] propertyToExclude;
        protected SerializedProperty elementProperty;
        protected SerializedProperty safeViewProperty;
        protected SerializedProperty safeAreaIgnoreProperty;
        protected SerializedProperty autoGetComponentProperty;
        protected SerializedProperty canvasProperty;

        protected virtual void OnEnable()
        {
            elementProperty = serializedObject.FindProperty("element");
            safeViewProperty = serializedObject.FindProperty("safeView");
            safeAreaIgnoreProperty = serializedObject.FindProperty("safeAreaIgnore");
            autoGetComponentProperty = serializedObject.FindProperty("autoGetComponent");
            canvasProperty = serializedObject.FindProperty("canvas");
            propertyToExclude = new string[]
            {
                "m_Script",
                "element",
                "safeView",
                "safeAreaIgnore",
                "autoGetComponent",
                "canvas"
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            DrawSafeViewContent();
            DrawPropertiesExcluding(serializedObject, propertyToExclude);
            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawSafeViewContent()
        {
            EditorGUILayout.PropertyField(elementProperty, GUIContent.none);
            EditorGUILayout.Space(1);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Root Canvas", GUILayout.Width(80));
            EditorGUILayout.PropertyField(autoGetComponentProperty, GUIContent.none, GUILayout.Width(20));
            GUI.enabled = !autoGetComponentProperty.boolValue;
            EditorGUILayout.PropertyField(canvasProperty, GUIContent.none);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(1);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Safe View", GUILayout.Width(80));
            EditorGUILayout.PropertyField(safeViewProperty, GUIContent.none);
            EditorGUILayout.PropertyField(safeAreaIgnoreProperty, GUIContent.none, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(1);
        }
    }

    [CustomEditor(typeof(GameFlowUICanvasOnKeyBack), true)]
    public class GameFlowUICanvasOnKeyBackEditor : GameFlowUICanvasEditor
    {
        private SerializedProperty onKeyBackProperty;
        private SerializedProperty useDefaultProperty;
        private GUIContent useDefaultContent;
        private GUIContent onKeyBackContent;

        protected override void OnEnable()
        {
            elementProperty = serializedObject.FindProperty("element");
            safeViewProperty = serializedObject.FindProperty("safeView");
            safeAreaIgnoreProperty = serializedObject.FindProperty("safeAreaIgnore");
            onKeyBackProperty = serializedObject.FindProperty("onKeyBack");
            useDefaultProperty = serializedObject.FindProperty("useDefaultKeyBack");
            useDefaultContent = EditorGUIUtility.TrTextContent("Use Default");
            onKeyBackContent = new GUIContent("On Key Back");
            propertyToExclude = new string[]
            {
                "m_Script",
                "element",
                "safeView",
                "safeAreaIgnore",
                "autoGetComponent",
                "canvas",
                "onKeyBack",
                "useDefaultKeyBack"
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            DrawSafeViewContent();
            EditorGUILayout.PropertyField(onKeyBackProperty, onKeyBackContent);
            var callbackRect = GUILayoutUtility.GetLastRect();
            var size = new Vector2(18, 18);
            var pos = new Rect(callbackRect.xMax - size.x, callbackRect.y + 1, size.x, size.y);
            useDefaultProperty.boolValue = GUI.Toggle(pos, useDefaultProperty.boolValue, GUIContent.none);
            var size2 = new Vector2(80, 18);
            var pos2 = new Rect(callbackRect.xMax - size2.x - 10, callbackRect.y + 1, size2.x, size2.y);
            GUI.Label(pos2, useDefaultContent);
            EditorGUILayout.Space(1);
            DrawPropertiesExcluding(serializedObject, propertyToExclude);
            serializedObject.ApplyModifiedProperties();
        }
    }
}