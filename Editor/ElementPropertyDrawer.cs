using GameFlow.Internal;
using UnityEditor;
using UnityEngine;

namespace GameFlow.Editor
{
    [CustomPropertyDrawer(typeof(ElementAttribute))]
    public class ElementPropertyDrawer : PropertyDrawer
    {
        private const int k_padding = 4;
        private const int k_innerBoxPadding = 4;
        private const float k_autoButtonWidth = 50f;
        private const float k_visualIndent = 15f; 

        private string GetPrefsKey(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var typeName = targetObject != null ? targetObject.GetType().Name : "UnknownType";
            return $"GameFlow_Element_Foldout_{typeName}_{property.propertyPath}";
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

            var contentRect = new Rect(position.x + k_padding, position.y + k_padding, position.width - k_padding * 2, position.height - k_padding * 2);
            
            var prefsKey = GetPrefsKey(property);
            var isExpanded = EditorPrefs.GetBool(prefsKey, property.isExpanded);

            var titleRect = new Rect(contentRect.x + 14, contentRect.y, 5, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();

            isExpanded = EditorGUI.Foldout(titleRect, isExpanded, GUIContent.none, true, EditorStyles.foldout);

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(prefsKey, isExpanded);
                property.isExpanded = isExpanded;
            }

            var propertyRect = new Rect(contentRect.x + 16, contentRect.y, contentRect.width - 16, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propertyRect, property, GUIContent.none, false);

            if (!isExpanded) return;

            var spacing = EditorGUIUtility.standardVerticalSpacing;
            var currentY = contentRect.y + EditorGUIUtility.singleLineHeight + spacing;
            
            var startX = contentRect.x + 16;
            var availableWidth = contentRect.width - 16;

            var serializedObject = property.serializedObject;

            var canvasProp = serializedObject.FindProperty("m_canvas");
            var autoGetProp = serializedObject.FindProperty("m_autoGetComponent");
            var offsetProp = serializedObject.FindProperty("m_offsetCanvasGroup");
            var safeViewProp = serializedObject.FindProperty("m_safeView");
            var safeIgnoreProp = serializedObject.FindProperty("m_safeAreaIgnore");

            if (canvasProp != null || offsetProp != null)
            {
                var offsetHeight = offsetProp != null ? EditorGUI.GetPropertyHeight(offsetProp, true) : 0f;
                var canvasHeight = canvasProp != null ? EditorGUIUtility.singleLineHeight : 0f;

                var box1Height = k_innerBoxPadding * 2f;
                if (canvasProp != null) box1Height += canvasHeight;
                if (offsetProp != null) box1Height += (canvasProp != null ? spacing : 0) + offsetHeight;

                var box1Rect = new Rect(startX, currentY, availableWidth, box1Height);
                GUI.Box(box1Rect, GUIContent.none, EditorStyles.helpBox);

                var innerX1 = box1Rect.x + k_innerBoxPadding;
                var innerY1 = box1Rect.y + k_innerBoxPadding;
                var innerW1 = box1Rect.width - k_innerBoxPadding * 2;

                if (canvasProp != null)
                {
                    var canvasRowRect = new Rect(innerX1, innerY1, innerW1, canvasHeight);
                    var isAuto = false;

                    if (autoGetProp != null)
                    {
                        var buttonRect = new Rect(canvasRowRect.xMax - k_autoButtonWidth, canvasRowRect.y, k_autoButtonWidth, canvasRowRect.height);
                        isAuto = autoGetProp.boolValue;
                        autoGetProp.boolValue = GUI.Toggle(buttonRect, isAuto, "Auto", EditorStyles.miniButton);
                        isAuto = autoGetProp.boolValue;
                    }

                    var autoBtnOffset = autoGetProp != null ? k_autoButtonWidth + 5f : 0f;
                    var canvasFullFieldRect = new Rect(canvasRowRect.x, canvasRowRect.y, canvasRowRect.width - autoBtnOffset, canvasRowRect.height);
                    
                    var labelWidth = EditorGUIUtility.labelWidth - 16f;
                    var labelRect = new Rect(canvasFullFieldRect.x, canvasFullFieldRect.y, labelWidth, canvasFullFieldRect.height);
                    var fieldRect = new Rect(canvasFullFieldRect.x + labelWidth, canvasFullFieldRect.y, canvasFullFieldRect.width - labelWidth, canvasFullFieldRect.height);

                    EditorGUI.LabelField(labelRect, "Canvas Root");

                    var preEnableState = GUI.enabled;
                    if (isAuto)
                    {
                        GUI.enabled = false;
                    }
                    
                    EditorGUI.PropertyField(fieldRect, canvasProp, GUIContent.none);
                    GUI.enabled = preEnableState;

                    innerY1 += canvasHeight;
                }

                if (offsetProp != null)
                {
                    if (canvasProp != null) innerY1 += spacing;
                    var offsetRect = new Rect(innerX1 + k_visualIndent, innerY1, innerW1 - k_visualIndent, offsetHeight);
                    EditorGUI.PropertyField(offsetRect, offsetProp, new GUIContent("↳ Offset Layer"));
                }

                currentY += box1Height + spacing;
            }

            var safeViewHeight = safeViewProp != null ? EditorGUI.GetPropertyHeight(safeViewProp, true) : 0f;
            var safeIgnoreHeight = safeIgnoreProp != null ? EditorGUI.GetPropertyHeight(safeIgnoreProp, true) : 0f;

            if (safeViewProp != null || safeIgnoreProp != null)
            {
                var box2Height = k_innerBoxPadding * 2f;
                if (safeViewProp != null) box2Height += safeViewHeight;
                if (safeIgnoreProp != null) box2Height += (safeViewProp != null ? spacing : 0) + safeIgnoreHeight;

                var box2Rect = new Rect(startX, currentY, availableWidth, box2Height);
                GUI.Box(box2Rect, GUIContent.none, EditorStyles.helpBox);

                var innerX2 = box2Rect.x + k_innerBoxPadding;
                var innerY2 = box2Rect.y + k_innerBoxPadding;
                var innerW2 = box2Rect.width - k_innerBoxPadding * 2;

                if (safeViewProp != null)
                {
                    var safeViewRect = new Rect(innerX2, innerY2, innerW2, safeViewHeight);
                    EditorGUI.PropertyField(safeViewRect, safeViewProp, new GUIContent("Safe View"));
                    innerY2 += safeViewHeight;
                }

                if (safeIgnoreProp != null)
                {
                    if (safeViewProp != null) innerY2 += spacing;
                    var safeIgnoreRect = new Rect(innerX2 + k_visualIndent, innerY2, innerW2 - k_visualIndent, safeIgnoreHeight);
                    EditorGUI.PropertyField(safeIgnoreRect, safeIgnoreProp, new GUIContent("↳ Ignore Edges"));
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var baseHeight = EditorGUIUtility.singleLineHeight + k_padding * 2;
            
            var prefsKey = GetPrefsKey(property);
            var isExpanded = EditorPrefs.GetBool(prefsKey, property.isExpanded);

            if (!isExpanded) return baseHeight;

            var spacing = EditorGUIUtility.standardVerticalSpacing;
            var height = baseHeight + spacing;

            var serializedObject = property.serializedObject;

            var canvasProp = serializedObject.FindProperty("m_canvas");
            var offsetProp = serializedObject.FindProperty("m_offsetCanvasGroup");
            var safeViewProp = serializedObject.FindProperty("m_safeView");
            var safeIgnoreProp = serializedObject.FindProperty("m_safeAreaIgnore");

            if (canvasProp != null || offsetProp != null)
            {
                var box1Height = k_innerBoxPadding * 2f;
                if (canvasProp != null) box1Height += EditorGUIUtility.singleLineHeight;
                if (offsetProp != null) box1Height += (canvasProp != null ? spacing : 0) + EditorGUI.GetPropertyHeight(offsetProp, true);
                
                height += box1Height + spacing; 
            }

            if (safeViewProp != null || safeIgnoreProp != null)
            {
                var box2Height = k_innerBoxPadding * 2f;
                if (safeViewProp != null) box2Height += EditorGUI.GetPropertyHeight(safeViewProp, true);
                if (safeIgnoreProp != null) box2Height += (safeViewProp != null ? spacing : 0) + EditorGUI.GetPropertyHeight(safeIgnoreProp, true);
                
                height += box2Height;
            }
            else
            {
                if (canvasProp != null || offsetProp != null)
                {
                    height -= spacing;
                }
            }

            return height;
        }
    }
}