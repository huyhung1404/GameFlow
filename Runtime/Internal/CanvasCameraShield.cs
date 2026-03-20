using System.Collections;
using GameFlow.Component;
using UnityEngine;
using UnityEngine.UI;

namespace GameFlow.Internal
{
    internal class CanvasCameraShield : BaseCanvasShield
    {
        protected override void SetUpCanvas()
        {
            var manager = InstanceManager.Manager;
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.sortingOrder = manager.LoadingShieldSortingOrder;
            canvas.vertexColorAlwaysGammaSpace = manager.VertexColorAlwaysGammaSpace;
            StartCoroutine(IELoadCamera(canvas));

            var canvasScale = canvas.GetComponent<CanvasScaler>();
            canvasScale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScale.referenceResolution = manager.ReferenceResolution;
            canvasScale.matchWidthOrHeight = (float)Screen.width / Screen.height < canvasScale.referenceResolution.x / canvasScale.referenceResolution.y ? 0 : 1;
        }

        private static IEnumerator IELoadCamera(Canvas canvas)
        {
            while (FlowUICamera.Instance == null) yield return null;
            canvas.worldCamera = FlowUICamera.Instance;
        }
    }
}