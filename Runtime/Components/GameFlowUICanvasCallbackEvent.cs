using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameFlow.Component
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [AddComponentMenu("Game Flow/UI Canvas Callback")]
    public class GameFlowUICanvasCallbackEvent : GameFlowUICanvas
    {
        [SerializeField] protected UnityEvent<int> onCanvasUpdate;

        protected override void SetUpCanvas()
        {
            base.SetUpCanvas();
            onCanvasUpdate?.Invoke(element.currentSortingOrder);
        }
    }
}