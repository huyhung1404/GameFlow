using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameFlow.Component
{
    [Serializable]
    public class UnityEventInt : UnityEvent<int>
    {
    }
    
    [AddComponentMenu("Game Flow/UI Canvas Callback")]
    public class GameFlowUICanvasCallbackEvent : GameFlowUICanvasOnKeyBack
    {
        [SerializeField, FormerlySerializedAs("onCanvasUpdate")] protected UnityEventInt m_onCanvasUpdate;

        protected override void SetUpCanvas()
        {
            base.SetUpCanvas();
            m_onCanvasUpdate?.Invoke(m_element.CurrentSortingOrder);
        }
    }
}