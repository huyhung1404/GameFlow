using UnityEngine;
using UnityEngine.UI;

namespace GameFlow.Internal
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(CanvasScaler))]
    internal class UIImageShield : LoadingShield
    {
        private Image _transparent;

        public override void SetUp()
        {
            _transparent = GetComponent<Image>();
            _transparent.raycastTarget = true;
            _transparent.enabled = IsShieldEnabled;
        }

        public override void OpenShield()
        {
            if (IsShieldEnabled) return;
            _transparent.enabled = true;
            IsShieldEnabled = true;
        }

        public override void CloseShield()
        {
            if (!IsShieldEnabled) return;
            _transparent.enabled = false;
            IsShieldEnabled = false;
        }
    }
}