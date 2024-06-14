using UnityEngine;

namespace GameFlow.Component
{
    [AddComponentMenu("Game Flow/UI Animation")]
    public class GameFlowUIAnimation : MonoBehaviour
    {
        [SerializeField] protected UserInterfaceFlowElement element;
        protected UIElementCallbackEvent delegates;

        private void Awake()
        {
            delegates = FlowSubject.UIEvent(element.GetType());
        }

        private void OnEnable()
        {
            delegates.OnActive += OnShow;
            delegates.OnHide += OnHide;
        }

        private void OnShow()
        {
        }

        private void OnHide(ICommandReleaseHandle handle)
        {
        }

        protected virtual void OnAnimationCompleted()
        {
            delegates.RaiseOnShowCompleted();
        }

        private void OnDisable()
        {
            delegates.OnActive -= OnShow;
            delegates.OnHide -= OnHide;
        }
    }
}