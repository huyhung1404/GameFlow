using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameFlow.Editor
{
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
    public partial class EditorIconButton : Button
    {
        [UxmlAttribute("icon-name")]
        public string IconName
        {
            get => _iconName;
            set { _iconName = value; SetIcon(value); }
        }

        [UxmlAttribute("scale-mode")]
        public ScaleMode ImageScaleMode
        {
            get => _scaleMode;
            set { _scaleMode = value; SetScaleMode(value); }
        }

        [UxmlAttribute("icon-margin-left")]
        public float IconMarginLeft
        {
            get => _iconMarginLeft;
            set { _iconMarginLeft = value; ApplyIconMargins(); }
        }

        [UxmlAttribute("icon-margin-top")]
        public float IconMarginTop
        {
            get => _iconMarginTop;
            set { _iconMarginTop = value; ApplyIconMargins(); }
        }

        [UxmlAttribute("icon-margin-right")]
        public float IconMarginRight
        {
            get => _iconMarginRight;
            set { _iconMarginRight = value; ApplyIconMargins(); }
        }

        [UxmlAttribute("icon-margin-bottom")]
        public float IconMarginBottom
        {
            get => _iconMarginBottom;
            set { _iconMarginBottom = value; ApplyIconMargins(); }
        }

        private string _iconName = "";
        private ScaleMode _scaleMode = ScaleMode.ScaleToFit;
        private float _iconMarginLeft;
        private float _iconMarginTop;
        private float _iconMarginRight = 4f;
        private float _iconMarginBottom;
#else
    public class EditorIconButton : Button
    {
        public new class UxmlFactory : UxmlFactory<EditorIconButton, UxmlTraits> { }

        public new class UxmlTraits : Button.UxmlTraits
        {
            private UxmlStringAttributeDescription m_iconName = new UxmlStringAttributeDescription { name = "icon-name", defaultValue = "" };
            private UxmlEnumAttributeDescription<ScaleMode> m_scaleMode = new UxmlEnumAttributeDescription<ScaleMode> { name = "scale-mode", defaultValue = ScaleMode.ScaleToFit };

            private UxmlFloatAttributeDescription m_iconMarginLeft = new UxmlFloatAttributeDescription { name = "icon-margin-left", defaultValue = 0f };
            private UxmlFloatAttributeDescription m_iconMarginTop = new UxmlFloatAttributeDescription { name = "icon-margin-top", defaultValue = 0f };
            private UxmlFloatAttributeDescription m_iconMarginRight = new UxmlFloatAttributeDescription { name = "icon-margin-right", defaultValue = 4f };
            private UxmlFloatAttributeDescription m_iconMarginBottom = new UxmlFloatAttributeDescription { name = "icon-margin-bottom", defaultValue = 0f };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var button = (EditorIconButton)ve;

                var iconName = m_iconName.GetValueFromBag(bag, cc);
                var scaleMode = m_scaleMode.GetValueFromBag(bag, cc);

                button.SetIcon(iconName);
                button.SetScaleMode(scaleMode);

                float ml = m_iconMarginLeft.GetValueFromBag(bag, cc);
                float mt = m_iconMarginTop.GetValueFromBag(bag, cc);
                float mr = m_iconMarginRight.GetValueFromBag(bag, cc);
                float mb = m_iconMarginBottom.GetValueFromBag(bag, cc);
                button.SetIconMargins(ml, mt, mr, mb);
            }
        }
#endif
        
        public override string text
        {
            get => _textLabel != null ? _textLabel.text : "";
            set
            {
                if (_textLabel == null) return;
                
                _textLabel.text = value;
                base.text = ""; 
            }
        }

        private Image _iconImage;
        private Label _textLabel;

        public EditorIconButton()
        {
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.Center;
            style.alignItems = Align.Center;

            _iconImage = new Image
            {
                scaleMode = ScaleMode.ScaleToFit
            };
            _iconImage.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            _iconImage.style.width = new StyleLength(StyleKeyword.Auto);
            _iconImage.style.flexShrink = 0f;
            _iconImage.style.marginRight = 4f;

            _textLabel = new Label();
            _textLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            _textLabel.style.flexShrink = 1f;

            Add(_iconImage);
            Add(_textLabel);
            
            base.text = "";
        }

        public void SetIcon(string iconName)
        {
            if (string.IsNullOrEmpty(iconName))
            {
                _iconImage.style.display = DisplayStyle.None;
                return;
            }

            var iconContent = EditorGUIUtility.IconContent(iconName);
            if (iconContent?.image is not Texture2D iconTexture)
            {
                _iconImage.style.display = DisplayStyle.None;
                return;
            }

            _iconImage.image = iconTexture;
            _iconImage.style.display = DisplayStyle.Flex;
        }

        public void SetScaleMode(ScaleMode mode)
        {
            if (_iconImage == null) return;
            _iconImage.scaleMode = mode;
        }

        private void ApplyIconMargins()
        {
            if (_iconImage == null) return;

#if UNITY_6000_0_OR_NEWER
            _iconImage.style.marginLeft = _iconMarginLeft;
            _iconImage.style.marginTop = _iconMarginTop;
            _iconImage.style.marginRight = _iconMarginRight;
            _iconImage.style.marginBottom = _iconMarginBottom;
#endif
        }

        public void SetIconMargins(float left, float top, float right, float bottom)
        {
            if (_iconImage == null) return;

#if UNITY_6000_0_OR_NEWER
            _iconMarginLeft = left;
            _iconMarginTop = top;
            _iconMarginRight = right;
            _iconMarginBottom = bottom;
#endif
            _iconImage.style.marginLeft = left;
            _iconImage.style.marginTop = top;
            _iconImage.style.marginRight = right;
            _iconImage.style.marginBottom = bottom;
        }

        public EditorIconButton WithEditorIcon(string iconName)
        {
            SetIcon(iconName);
            return this;
        }

        public EditorIconButton WithScaleMode(ScaleMode mode)
        {
            SetScaleMode(mode);
            return this;
        }

        public EditorIconButton WithText(string buttonText)
        {
            text = buttonText;
            return this;
        }

        public EditorIconButton WithIconMargins(float left, float top, float right, float bottom)
        {
            SetIconMargins(left, top, right, bottom);
            return this;
        }
    }
}