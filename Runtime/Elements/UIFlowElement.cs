using System;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    public class UIFlowElement : GameFlowElement
    {
        internal int currentSortingOrder;
        [SerializeField] internal bool fullScene;
    }
}