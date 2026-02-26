namespace GameFlow.Component
{
    public abstract class GameFlowUICanvasWithAnimation : GameFlowUICanvas
    {
        private ICommandReleaseHandle _releaseHandle;

        protected override void OnEnable()
        {
            base.OnEnable();
            _delegates.OnHide += OnHide;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _delegates.OnHide -= OnHide;
        }

        public override void OnActive()
        {
            base.OnActive();
            OnShow();
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
            FlowObservable.UIEvent(m_element.ElementType).RaiseOnShowCompleted();
        }

        protected void OnHideCompleted()
        {
            if (_releaseHandle == null) return;
            _releaseHandle.Next();
            _releaseHandle = null;
        }
    }
}