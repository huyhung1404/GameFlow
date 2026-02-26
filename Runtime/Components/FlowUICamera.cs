using UnityEngine;

namespace GameFlow.Component
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Game Flow/UI Camera")]
    public class FlowUICamera : MonoBehaviour
    {
        public static Camera Instance { get; private set; }

        private void Awake()
        {
            Instance = GetComponent<Camera>();
        }
    }
}