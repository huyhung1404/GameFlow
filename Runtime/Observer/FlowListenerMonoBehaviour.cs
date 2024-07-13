using UnityEngine;

namespace GameFlow
{
    public class FlowListenerMonoBehaviour : MonoBehaviour
    {
        public virtual void OnActive()
        {
        }

        public virtual void OnActiveWithData(object data)
        {
        }

        public virtual void OnRelease(bool isReleaseImmediately)
        {
        }

        public virtual void OnKeyBack()
        {
        }

        public virtual void OnReFocus()
        {
        }
    }

    public class FlowListenerMonoBehaviour<T> : FlowListenerMonoBehaviour
    {
        public override void OnActiveWithData(object data)
        {
            OnActiveWithData((T)data);
        }

        public virtual void OnActiveWithData(T data)
        {
        }
    }
}