using GameFlow.Internal;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GameFlow.Editor
{
    [CustomPropertyDrawer(typeof(InternalDrawAttribute))]
    public class InternalPropertyDrawer : PropertyDrawer
    {
        private static UnityEventDrawer eventDrawer;
        private const int k_Space = 2;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (((InternalDrawAttribute)attribute).drawType)
            {
                default:
                case DrawType.Element:
                    DrawElement(new Rect(position.x, position.y, position.width, position.height - k_Space), property);
                    break;
                case DrawType.Canvas:
                    DrawCanvas(new Rect(position.x, position.y, position.width, position.height - k_Space), property);
                    break;
                case DrawType.SafeView:
                    DrawSafeView(new Rect(position.x, position.y, position.width, position.height - k_Space), property);
                    break;
                case DrawType.OnKeyBack:
                    DrawKeyBack(new Rect(position.x, position.y, position.width, position.height - k_Space), property, label);
                    break;
            }
        }

        private static void DrawElement(Rect position, SerializedProperty property)
        {
            EditorGUI.PropertyField(position, property, GUIContent.none);
        }

        private static void DrawCanvas(Rect position, SerializedProperty property)
        {
            var autoGetComponentProperty = property.serializedObject.FindProperty("autoGetComponent");
            var offsetCanvasGroup = property.serializedObject.FindProperty("offsetCanvasGroup");
            EditorGUI.LabelField(new Rect(position.x, position.y, 80, position.height), "Root Canvas");
            EditorGUI.PropertyField(new Rect(position.x + 80, position.y, 20, position.height), autoGetComponentProperty, GUIContent.none);
            GUI.enabled = !autoGetComponentProperty.boolValue;
            EditorGUI.PropertyField(new Rect(position.x + 100, position.y, position.width - 135, position.height), property, GUIContent.none);
            GUI.enabled = true;
            EditorGUI.PropertyField(new Rect(position.x + position.width - 30, position.y, 30, position.height), offsetCanvasGroup, GUIContent.none);
        }

        private static void DrawSafeView(Rect position, SerializedProperty property)
        {
            var ignore = property.serializedObject.FindProperty("safeAreaIgnore");
            EditorGUI.LabelField(new Rect(position.x, position.y, 80, position.height), "Safe View");
            EditorGUI.PropertyField(new Rect(position.x + 80, position.y, position.width - 160, position.height), property, GUIContent.none);
            EditorGUI.PropertyField(new Rect(position.x + (position.width - 80), position.y, 80, position.height), ignore, GUIContent.none);
        }

        private static void DrawKeyBack(Rect position, SerializedProperty property, GUIContent label)
        {
            eventDrawer ??= new UnityEventDrawer();
            eventDrawer.OnGUI(position, property, label);
            var useDefaultProperty = property.serializedObject.FindProperty("useDefaultKeyBack");
            EditorGUI.PropertyField(new Rect(position.x + position.width - 18, position.y + 1, 18, 18), useDefaultProperty, GUIContent.none);
            EditorGUI.LabelField(new Rect(position.x + position.width - 90, position.y + 1, 80, 18), "Use Default");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            switch (((InternalDrawAttribute)attribute).drawType)
            {
                default:
                case DrawType.Element:
                case DrawType.Canvas:
                case DrawType.SafeView:
                    return EditorGUIUtility.singleLineHeight + k_Space;
                case DrawType.OnKeyBack:
                    eventDrawer ??= new UnityEventDrawer();
                    return eventDrawer.GetPropertyHeight(property, label) + k_Space;
            }
        }
    }
}