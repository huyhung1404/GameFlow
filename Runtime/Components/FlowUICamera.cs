using UnityEngine;

namespace GameFlow.Component
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Game Flow/UI Camera")]
    public class FlowUICamera : MonoBehaviour
    {
        public static Camera instance { get; private set; }

        private void Awake()
        {
            instance = GetComponent<Camera>();
        }

        public bool IsRectTransformVisible(RectTransform rectTransform, RectTransform canvas)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            for (var index = corners.Length - 1; index >= 0; index--)
            {
                var corner = corners[index];
                var viewportPoint = instance.WorldToViewportPoint(corner);
                if (viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1) continue;
                if (RectTransformUtility.RectangleContainsScreenPoint(canvas, corner)) return true;
            }

            return false;
        }
    }
}