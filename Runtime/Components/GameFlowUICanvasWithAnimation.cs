namespace GameFlow.Component
{
    public abstract class GameFlowUICanvasWithAnimation : GameFlowUICanvas
    {
        private ICommandReleaseHandle releaseHandle;

        protected override void OnEnable()
        {
            base.OnEnable();
            delegates.OnHide += OnHide;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            delegates.OnHide -= OnHide;
        }

        public override void OnActive()
        {
            base.OnActive();
            OnShow();
        }

        private void OnHide(ICommandReleaseHandle handle)
        {
            releaseHandle = handle;
            OnHide();
        }

        protected abstract void OnShow();
        protected abstract void OnHide();

        protected void OnShowCompleted()
        {
            FlowObservable.UIEvent(element.elementType).RaiseOnShowCompleted();
        }

        protected void OnHideCompleted()
        {
            if (releaseHandle == null) return;
            releaseHandle.Next();
            releaseHandle = null;
        }
    }
}