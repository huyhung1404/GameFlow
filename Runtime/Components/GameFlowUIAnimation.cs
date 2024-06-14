using UnityEngine;

namespace GameFlow.Component
{
    [AddComponentMenu("Game Flow/UI Animation")]
    public class GameFlowUIAnimation : MonoBehaviour
    {
        [SerializeField] protected UserInterfaceFlowElement element;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
        }
    }
}