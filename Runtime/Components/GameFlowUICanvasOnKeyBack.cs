using UnityEngine;
using UnityEngine.Events;

namespace GameFlow.Component
{
    [AddComponentMenu("Game Flow/UI Canvas Key Back")]
    public class GameFlowUICanvasOnKeyBack : GameFlowUICanvas
    {
        [SerializeField] protected UnityEvent onKeyBack;
        [SerializeField] protected bool useDefaultKeyBack;

        public override void OnKeyBack()
        {
            if (useDefaultKeyBack) base.OnKeyBack();
            onKeyBack?.Invoke();
        }
    }
}