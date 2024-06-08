using System;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    public class UserInterfaceFlowElement : GameFlowElement
    {
        [SerializeField, HideInInspector] internal bool fullScene;
    }
}