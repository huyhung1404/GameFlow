using UnityEngine;

namespace GameFlow.Internal
{
    public abstract class CloneElement
    {
        internal CloneElement(GameFlowElement baseElement)
        {
            //Tao element
            //SInh instance
            //Doi element
        }

        internal GameObject Instance()
        {
            return null;
        }

        internal GameObject RuntimeInstance()
        {
            return null;
        }

        internal abstract void ActiveElement();
    }
}