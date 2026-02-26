using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameFlow.Component
{
    [AddComponentMenu("Game Flow/UI Canvas Callback")]
    public class GameFlowUICanvasCallbackEvent : GameFlowUICanvasOnKeyBack
    {
        [SerializeField, FormerlySerializedAs("onCanvasUpdate")] protected UnityEvent<int> m_onCanvasUpdate;

        protected override void SetUpCanvas()
        {
            base.SetUpCanvas();
            m_onCanvasUpdate?.Invoke(m_element.CurrentSortingOrder);
        }
    }
}