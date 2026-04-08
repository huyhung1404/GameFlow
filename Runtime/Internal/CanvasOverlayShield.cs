using UnityEngine;
using UnityEngine.UI;

namespace GameFlow.Internal
{
    internal class CanvasOverlayShield : BaseCanvasShield
    {
        protected override void SetUpCanvas()
        {
            var manager = GameFlowContext.Current.Manager;
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = manager.LoadingShieldSortingOrder;
            canvas.vertexColorAlwaysGammaSpace = manager.VertexColorAlwaysGammaSpace;

            var canvasScale = canvas.GetComponent<CanvasScaler>();
            canvasScale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScale.referenceResolution = manager.ReferenceResolution;
            canvasScale.matchWidthOrHeight = (float)Screen.width / Screen.height < canvasScale.referenceResolution.x / canvasScale.referenceResolution.y ? 0 : 1;
        }
    }
}
