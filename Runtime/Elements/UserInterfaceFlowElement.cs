using System;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    public class UserInterfaceFlowElement : GameFlowElement
    {
        internal int currentSortingOrder;
        [SerializeField] internal bool fullScene;
    }
}