using UnityEngine;
using UnityEngine.Events;

namespace GameFlow.Component
{
    [AddComponentMenu("Game Flow/UI Canvas Callback")]
    public class GameFlowUICanvasCallbackEvent : GameFlowUICanvasOnKeyBack
    {
        [SerializeField] protected UnityEvent<int> onCanvasUpdate;

        protected override void SetUpCanvas()
        {
            base.SetUpCanvas();
            onCanvasUpdate?.Invoke(element.currentSortingOrder);
        }
    }
}