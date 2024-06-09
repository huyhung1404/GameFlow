using System;
using UnityEngine;

namespace GameFlow.Component
{
    [Serializable]
    public enum SafeAreaIgnore
    {
        NONE,
        HEIGHT,
        WIDTH,
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
        ALL
    }

    public static class UnitySafeArea
    {
        public static void ApplySafeArea(this RectTransform panel, Rect safeArea, SafeAreaIgnore ignore)
        {
            if (ignore == SafeAreaIgnore.ALL) return;
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            switch (ignore)
            {
                case SafeAreaIgnore.NONE:
                    break;
                case SafeAreaIgnore.HEIGHT:
                    anchorMin.y = 0;
                    anchorMax.y = 1;
                    break;
                case SafeAreaIgnore.WIDTH:
                    anchorMin.x = 0;
                    anchorMax.x = 1;
                    break;
                case SafeAreaIgnore.TOP:
                    anchorMax.y = 1;
                    break;
                case SafeAreaIgnore.BOTTOM:
                    anchorMin.y = 0;
                    break;
                case SafeAreaIgnore.LEFT:
                    anchorMin.x = 0;
                    break;
                case SafeAreaIgnore.RIGHT:
                    anchorMax.x = 1;
                    break;
            }

            panel.anchorMin = anchorMin;
            panel.anchorMax = anchorMax;
        }
    }
}