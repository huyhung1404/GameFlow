using GameFlow.Internal;
using UnityEngine;

namespace GameFlow.Component
{
    public delegate void OnBannerUpdate(float height);

    public static class FlowBannerController
    {
        public static event OnBannerUpdate OnBannerUpdate
        {
            add => GameFlowRuntimeController.OnBannerUpdate += value; 
            remove => GameFlowRuntimeController.OnBannerUpdate -= value;
        }
        public static int CurrentBannerHeight { get; private set; }
        private static bool s_isShowBanner;
        private static int s_bannerHeight;

        public static void UpdateBannerStatus(bool isShow)
        {
            s_isShowBanner = isShow;
            UpdateBanner();
        }

        public static void UpdateBannerHeight(int height)
        {
            s_bannerHeight = height;
            UpdateBanner();
        }

        private static void UpdateBanner()
        {
            var height = s_isShowBanner ? s_bannerHeight : 0;
            if (CurrentBannerHeight == height) return;
            CurrentBannerHeight = height;
            GameFlowRuntimeController.s_UpdateBanner = true;
        }

        public static float Dp2Px(float dp) => dp * (Screen.dpi / 160);
        public static float Px2Dp(float px) => px * (160 / Screen.dpi);
    }
}