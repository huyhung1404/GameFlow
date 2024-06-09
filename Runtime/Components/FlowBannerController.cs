using GameFlow.Internal;
using UnityEngine;

namespace GameFlow.Component
{
    public delegate void OnBannerUpdate(float height);

    public static class FlowBannerController
    {
        public static event OnBannerUpdate OnBannerUpdate
        {
            add => GameFlowRuntimeController.onBannerUpdate += value; 
            remove => GameFlowRuntimeController.onBannerUpdate -= value;
        }
        public static int CurrentBannerHeight { get; private set; }
        private static bool isShowBanner;
        private static int bannerHeight;

        public static void UpdateBannerStatus(bool isShow)
        {
            isShowBanner = isShow;
            UpdateBanner();
        }

        public static void UpdateBannerHeight(int height)
        {
            bannerHeight = height;
            UpdateBanner();
        }

        private static void UpdateBanner()
        {
            var height = isShowBanner ? bannerHeight : 0;
            if (CurrentBannerHeight == height) return;
            CurrentBannerHeight = height;
            GameFlowRuntimeController.updateBanner = true;
        }

        public static float Dp2Px(float dp) => dp * (Screen.dpi / 160);
        public static float Px2Dp(float px) => px * (160 / Screen.dpi);
    }
}