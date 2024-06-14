using GameFlow.Component;
using UnityEditor;
using UnityEngine;

namespace GameFlow.Editor
{
    [CustomEditor(typeof(GameFlowUICanvas), true)]
    public class GameFlowUICanvasEditor : UnityEditor.Editor
    {
        private static readonly string[] propertyToExclude = new string[]
        {
            "m_Script",
            "element",
            "safeView",
            "safeAreaIgnore"
        };

        private SerializedProperty elementProperty;
        private SerializedProperty safeViewProperty;
        private SerializedProperty safeAreaIgnoreProperty;

        private void OnEnable()
        {
            elementProperty = serializedObject.FindProperty("element");
            safeViewProperty = serializedObject.FindProperty("safeView");
            safeAreaIgnoreProperty = serializedObject.FindProperty("safeAreaIgnore");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(elementProperty, GUIContent.none);
            EditorGUILayout.Space(1);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Safe View", GUILayout.Width(65));
            EditorGUILayout.PropertyField(safeViewProperty, GUIContent.none);
            EditorGUILayout.PropertyField(safeAreaIgnoreProperty, GUIContent.none, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
            DrawPropertiesExcluding(serializedObject, propertyToExclude);
            serializedObject.ApplyModifiedProperties();
        }
    }
}