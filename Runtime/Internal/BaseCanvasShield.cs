using UnityEngine;
using UnityEngine.UI;

namespace GameFlow.Internal
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(CanvasScaler))]
    internal abstract class BaseCanvasShield : LoadingShield
    {
        private Image _transparent;

        internal override void SetUp()
        {
            _transparent = GetComponent<Image>();
            _transparent.raycastTarget = true;
            _transparent.enabled = IsShieldEnabled;
            SetUpCanvas();
        }

        protected abstract void SetUpCanvas();

        internal override void OpenShield()
        {
            if (IsShieldEnabled) return;
            _transparent.enabled = true;
            IsShieldEnabled = true;
        }

        internal override void CloseShield()
        {
            if (!IsShieldEnabled) return;
            _transparent.enabled = false;
            IsShieldEnabled = false;
        }
    }
}