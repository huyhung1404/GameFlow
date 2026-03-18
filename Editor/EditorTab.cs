using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
    public partial class EditorTab : VisualElement
    {
        [UxmlAttribute("target-id")]
        public string TargetId { get => _targetId; set => _targetId = value; }

        [UxmlAttribute("icon-name")]
        public string IconName { get => _iconName; set { _iconName = value; SetIcon(value); } }

        [UxmlAttribute("text")]
        public string Text { get => _label.text; set => _label.text = value; }

        private string _targetId = "";
        private string _iconName = "";
#else
    public class EditorTab : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<EditorTab, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription m_targetId = new UxmlStringAttributeDescription { name = "target-id", defaultValue = "" };
            private UxmlStringAttributeDescription m_iconName = new UxmlStringAttributeDescription { name = "icon-name", defaultValue = "" };
            private UxmlStringAttributeDescription m_text = new UxmlStringAttributeDescription { name = "text", defaultValue = "New Tab" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var tab = (EditorTab)ve;
                tab.TargetId = m_targetId.GetValueFromBag(bag, cc);
                tab.IconName = m_iconName.GetValueFromBag(bag, cc);
                tab.Text = m_text.GetValueFromBag(bag, cc);
            }
        }

        public string TargetId { get; set; } = "";
        public string IconName { get => _iconName; set { _iconName = value; SetIcon(value); } }
        public string Text { get => _label.text; set => _label.text = value; }
        
        private string _iconName = "";
#endif

        private Image _iconImage;
        private Label _label;

        public EditorTab()
        {
            AddToClassList("gameflow-tab");
            
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.Center;
            style.alignItems = Align.Center;

            _iconImage = new Image { scaleMode = ScaleMode.ScaleToFit };
            _iconImage.AddToClassList("gameflow-tab__icon");
            _iconImage.style.display = DisplayStyle.None; 

            _label = new Label("New Tab");
            _label.AddToClassList("gameflow-tab__label");

            Add(_iconImage);
            Add(_label);
        }

        private void SetIcon(string iconName)
        {
            if (string.IsNullOrEmpty(iconName))
            {
                _iconImage.style.display = DisplayStyle.None;
                return;
            }

            var iconContent = EditorGUIUtility.IconContent(iconName);
            if (iconContent?.image is Texture2D iconTexture)
            {
                _iconImage.image = iconTexture;
                _iconImage.style.display = DisplayStyle.Flex;
            }
        }
    }
}