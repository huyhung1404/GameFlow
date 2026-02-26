using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameFlow.Component
{
    [Serializable]
    public class KeyBackEvent
    {
        public bool UseDefault;
        public UnityEvent OnKeyBack;
    }

    [AddComponentMenu("Game Flow/UI Canvas Key Back")]
    public class GameFlowUICanvasOnKeyBack : GameFlowUICanvas
    {
        [SerializeField] private KeyBackEvent m_keyBackEvent;

        public override void OnKeyBack()
        {
            if (m_keyBackEvent.UseDefault) base.OnKeyBack();
            m_keyBackEvent.OnKeyBack.Invoke();
        }
    }
}