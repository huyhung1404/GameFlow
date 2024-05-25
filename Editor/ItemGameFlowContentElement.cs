using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public class ItemGameFlowContentElement : VisualElement
    {
        private const string kUxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/ItemGameFlowContentElement.uxml";

        public ItemGameFlowContentElement()
        {
            var root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(kUxmlPath).CloneTree();
            root.Q<IMGUIContainer>("title_gui").onGUIHandler = DrawTitleGUI;
            Add(root);
        }

        private void DrawTitleGUI()
        {
            var guiWidth = EditorGUIUtility.currentViewWidth;
            EditorGUI.Toggle(new Rect(0, 0, 20, 20), GUIContent.none, false);
            EditorGUI.TextField(new Rect(22, 1, Mathf.Max(60,guiWidth / 4), 18), GUIContent.none, "Test");
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