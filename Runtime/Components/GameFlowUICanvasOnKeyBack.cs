using GameFlow.Internal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameFlow.Component
{
    [AddComponentMenu("Game Flow/UI Canvas Key Back")]
    public class GameFlowUICanvasOnKeyBack : GameFlowUICanvas
    {
        [SerializeField, InternalDraw(DrawType.OnKeyBack), FormerlySerializedAs("onKeyBack")] protected UnityEvent m_onKeyBack;
        [SerializeField, HideInInspector, FormerlySerializedAs("useDefaultKeyBack")] protected bool m_useDefaultKeyBack;

        public override void OnKeyBack()
        {
            if (m_useDefaultKeyBack) base.OnKeyBack();
            m_onKeyBack?.Invoke();
        }
    }
}