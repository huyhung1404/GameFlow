using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
#endif
    public partial class GameFlowVisualElement : VisualElement
    {
        private const string k_uxmlPath = "Packages/com.huyhung1404.gameflow/Editor/UXML/GameFlowVisualElement.uxml";
        private readonly Foldout _container;
        private readonly List<ItemGameFlowContentElement> _elements;

        public GameFlowVisualElement()
        {
            var root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_uxmlPath).CloneTree();
            _container = root.Q<Foldout>("container");
            _elements = new List<ItemGameFlowContentElement>();
            Add(root);
        }

        public void UpdateGraphic(bool isUserInterface, Type type, ElementProperty elementProperty, Action<int> removeAt)
        {
            _container.text = $"{type.Name}.cs";
            _container.BindToViewDataKey(_container.text);
            
            var index = 0;
            for (; index < elementProperty.Properties.Count; index++)
            {
                if (index >= _elements.Count)
                {
                    var visual = new ItemGameFlowContentElement();
                    _container.Add(visual);
                    _elements.Add(visual);
                }

                var visualElement = _elements[index];
                visualElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                visualElement.UpdateGraphic(elementProperty.Properties[index], removeAt);
            }

            for (; index < _elements.Count; index++)
            {
                _elements[index].HideGraphic();
                _elements[index].style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
        }

#if !UNITY_6000_0_OR_NEWER
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
#endif
    }
}