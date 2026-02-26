using System;
using UnityEngine;

namespace GameFlow.Component
{
    [Flags]
    [Serializable]
    public enum SafeAreaIgnore
    {
        None = 0,
        Top = 1 << 0,
        Bottom = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        Height = Top | Bottom,
        Width = Left | Right,
        All = Height | Width
    }

    public static class UnitySafeArea
    {
        public static void ApplySafeArea(this RectTransform panel, Rect safeArea, SafeAreaIgnore ignore)
        {
            if (ignore == SafeAreaIgnore.All)
            {
                panel.anchorMin = Vector2.zero;
                panel.anchorMax = Vector2.one;
                return;
            }

            var screenWidth = Screen.width;
            var screenHeight = Screen.height;

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= screenWidth;
            anchorMin.y /= screenHeight;
            anchorMax.x /= screenWidth;
            anchorMax.y /= screenHeight;

            if ((ignore & SafeAreaIgnore.Top) != 0) anchorMax.y = 1f;
            if ((ignore & SafeAreaIgnore.Bottom) != 0) anchorMin.y = 0f;
            if ((ignore & SafeAreaIgnore.Left) != 0) anchorMin.x = 0f;
            if ((ignore & SafeAreaIgnore.Right) != 0) anchorMax.x = 1f;

            panel.anchorMin = anchorMin;
            panel.anchorMax = anchorMax;
        }

        public static RectOffset GetOffset()
        {
            var safeArea = Screen.safeArea;
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;

            var leftOffset = safeArea.x;
            var bottomOffset = safeArea.y;
            var rightOffset = screenWidth - (safeArea.x + safeArea.width);
            var topOffset = screenHeight - (safeArea.y + safeArea.height);

            return new RectOffset((int)leftOffset, (int)rightOffset, (int)topOffset, (int)bottomOffset);
        }
    }
}