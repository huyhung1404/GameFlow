using GameFlow.Component;
using UnityEditor;
using UnityEngine;

namespace GameFlow.Editor
{
    [CustomPropertyDrawer(typeof(KeyBackEvent))]
    public class KeyBackEventDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var useDefaultProperty = property.FindPropertyRelative("UseDefault");
            var onKeyBackProp = property.FindPropertyRelative("OnKeyBack");
            
            EditorGUI.PropertyField(position, onKeyBackProp, new GUIContent("On Key Back"), true);

            EditorGUI.PropertyField(new Rect(position.x + position.width - 18, position.y + 1, 18, 18), useDefaultProperty, GUIContent.none);
            EditorGUI.LabelField(new Rect(position.x + position.width - 90, position.y + 1, 80, 18), "Use Default");

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var onKeyBackProp = property.FindPropertyRelative("OnKeyBack");
            return onKeyBackProp != null
                ? EditorGUI.GetPropertyHeight(onKeyBackProp)
                : EditorGUIUtility.singleLineHeight;
        }
    }
}