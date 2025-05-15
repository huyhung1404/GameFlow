using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
    public class GameFlowVisualElement : VisualElement
    {
        private const string kUxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/GameFlowVisualElement.uxml";
        private readonly Foldout container;
        private readonly List<ItemGameFlowContentElement> elements;

        public GameFlowVisualElement()
        {
            var root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(kUxmlPath).CloneTree();
            container = root.Q<Foldout>("container");
            elements = new List<ItemGameFlowContentElement>();
            Add(root);
        }

        public void UpdateGraphic(bool isUserInterface, Type type, ElementProperty elementProperty, Action<int> removeAt)
        {
            container.text = $"{type.Name}.cs";
            container.BindToViewDataKey(container.text);
            var index = 0;
            for (; index < elementProperty.properties.Count; index++)
            {
                if (index >= elements.Count)
                {
                    var visual = new ItemGameFlowContentElement();
                    container.Add(visual);
                    elements.Add(visual);
                }

                var visualElement = elements[index];
                visualElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                visualElement.UpdateGraphic(elementProperty.properties[index], removeAt);
            }

            for (; index < elements.Count; index++)
            {
                elements[index].HideGraphic();
                elements[index].style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
        }

        public new class UxmlFactory : UxmlFactory<GameFlowVisualElement, UxmlTraits>
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