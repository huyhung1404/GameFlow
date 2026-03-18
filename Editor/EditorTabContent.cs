using UnityEngine.UIElements;

namespace GameFlow.Editor
{
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
    public partial class EditorTabContent : VisualElement
    {
        [UxmlAttribute("target-id")] public string TargetId { get => _targetId; set => _targetId = value; }
        private string _targetId = "";
#else
    public class EditorTabContent : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<EditorTabContent, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription m_targetId = new UxmlStringAttributeDescription { name = "target-id", defaultValue = "" };
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var content = (EditorTabContent)ve;
                content.TargetId = m_targetId.GetValueFromBag(bag, cc);
            }
        }

        public string TargetId { get; set; } = "";
#endif

        public EditorTabContent()
        {
            AddToClassList("gameflow-tab-content");
            style.flexGrow = 1;
        }

        public EditorTabContent WithTargetId(string id)
        {
            TargetId = id;
            return this;
        }
    }
}