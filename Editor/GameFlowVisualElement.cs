using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public class GameFlowVisualElement : VisualElement
    {
        private const string kUxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/GameFlowVisualElement.uxml";

        public GameFlowVisualElement()
        {
            var root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(kUxmlPath).CloneTree();
            Add(root);
        }

        public new class UxmlFactory : UxmlFactory<GameFlowVisualElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription { get { yield break; } }
        }
    }
}