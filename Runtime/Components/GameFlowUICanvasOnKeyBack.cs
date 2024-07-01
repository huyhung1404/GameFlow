using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameFlow.Component
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [AddComponentMenu("Game Flow/UI Canvas Key Back")]
    public class GameFlowUICanvasOnKeyBack : GameFlowUICanvas
    {
        [SerializeField] protected UnityEvent onKeyBack;
        [SerializeField] protected bool useDefaultKeyBack;

        protected override void OnKeyBack()
        {
            if (useDefaultKeyBack) base.OnKeyBack();
            onKeyBack?.Invoke();
        }
    }
}