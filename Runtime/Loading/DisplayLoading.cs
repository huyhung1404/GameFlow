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
    }
}