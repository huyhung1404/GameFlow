using GameFlow.Internal;
using UnityEngine;
using UnityEngine.Events;

namespace GameFlow.Component
{
    [AddComponentMenu("Game Flow/UI Canvas Key Back")]
    public class GameFlowUICanvasOnKeyBack : GameFlowUICanvas
    {
        [SerializeField, InternalDraw(DrawType.OnKeyBack)] protected UnityEvent onKeyBack;
        [SerializeField, HideInInspector] protected bool useDefaultKeyBack;

        public override void OnKeyBack()
        {
            if (useDefaultKeyBack) base.OnKeyBack();
            onKeyBack?.Invoke();
        }
    }
}