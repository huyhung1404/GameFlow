using GameFlow.Internal;
using UnityEngine;

namespace GameFlow.Component
{
    public abstract class GameFlowUIAnimation : ElementMonoBehaviours
    {
        [SerializeField, Element] protected UIFlowElement m_element;
        protected UIElementCallbackEvent _delegates;
        private ICommandReleaseHandle _releaseHandle;

        protected virtual void Awake()
        {
            m_element.EnsureCallbackEvent();
            _delegates = (UIElementCallbackEvent)m_element.CallbackEvent;
        }

        protected virtual void OnEnable()
        {
            _delegates.OnActive += OnShow;
            _delegates.OnHide += OnHide;
        }

        protected virtual void OnDisable()
        {
            _delegates.OnActive -= OnShow;
            _delegates.OnHide -= OnHide;
        }

        private void OnHide(ICommandReleaseHandle handle)
        {
            _releaseHandle = handle;
            OnHide();
        }

        protected abstract void OnShow();
        protected abstract void OnHide();

        protected void OnShowCompleted()
        {
            _delegates.RaiseOnShowCompleted();
        }

        protected void OnHideCompleted()
        {
            if (_releaseHandle == null) return;
            _releaseHandle.Next();
            _releaseHandle = null;
        }

        internal override void SetElement(GameFlowElement value, System.Type type)
        {
            if (m_element != null && type != m_element.GetType()) return;
            if (value is not UIFlowElement uiElement) return;

            var wasSubscribed = _delegates != null && isActiveAndEnabled;
            if (wasSubscribed)
            {
                _delegates.OnActive -= OnShow;
                _delegates.OnHide -= OnHide;
            }

            m_element = uiElement;
            m_element.EnsureCallbackEvent();
            _delegates = (UIElementCallbackEvent)m_element.CallbackEvent;

            if (wasSubscribed)
            {
                _delegates.OnActive += OnShow;
                _delegates.OnHide += OnHide;
            }
        }
    }
}
