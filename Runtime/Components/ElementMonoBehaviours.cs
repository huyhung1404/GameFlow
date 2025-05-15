using System;
using UnityEngine;

namespace GameFlow.Component
{
    public abstract class ElementMonoBehaviours : MonoBehaviour
    {
        internal abstract void SetElement(GameFlowElement element, Type type);
    }
}