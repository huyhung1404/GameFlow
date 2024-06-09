using UnityEngine;

namespace GameFlow.Component
{
    [RequireComponent(typeof(Camera))]
    public class FlowUICamera : MonoBehaviour
    {
        public static Camera instance { get; private set; }

        private void Awake()
        {
            instance = GetComponent<Camera>();
        }
    }
}