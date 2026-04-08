using GameFlow.Internal;
using UnityEngine;

namespace GameFlow.Component
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Game Flow/UI Camera")]
    public class FlowUICamera : MonoBehaviour
    {
        public static Camera Instance => GameFlowContext.Current?.UICamera;

        private void Awake()
        {
            var cam = GetComponent<Camera>();
            var context = GameFlowContext.Current;
            if (context != null)
                context.SetUICamera(cam);
            else
                InstanceManager.SetInstance(cam);
        }
    }
}
