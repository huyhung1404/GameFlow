using UnityEngine;
using UnityEngine.UI;

namespace GameFlow.Component
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class GameFlowUICanvas : MonoBehaviour
    {
        [SerializeField] protected UserInterfaceFlowElement element;

        private void OnEnable()
        {
            FlowSubject.UIEvent(element.elementType).OnSetUpCanvas += OnSetUpCanvas;
        }

        private void OnSetUpCanvas(int sortingOrder)
        {
        }


        private void OnDisable()
        {
            FlowSubject.UIEvent(element.elementType).OnSetUpCanvas -= OnSetUpCanvas;
        }
    }
}