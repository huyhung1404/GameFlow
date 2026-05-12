using GameFlow.Component;

namespace GameFlow
{
    public abstract class FlowListenerMonoBehaviour : ElementMonoBehaviours, IFlowListener
    {
        public virtual void OnActive()
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
}