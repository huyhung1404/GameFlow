using System;

namespace GameFlow
{
    public class DisplayLoading : BaseLoadingTypeController
    {
        protected override void OnShow()
        {
            isShow = true;
        }

        protected override void OnHide()
        {
            isShow = false;
        }

        public override BaseLoadingTypeController OnCompleted(Action onCompleted)
        {
            onCompleted?.Invoke();
            return this;
        }
    }
}