using System;
using UnityEngine;

namespace GameFlow.Internal
{
    [Serializable]
    public enum ShieldType
    {
        CanvasOverlay,
        CanvasCamera
    }

    internal abstract class LoadingShield : MonoBehaviour
    {
        public bool IsShieldEnabled { get; protected set; }
        internal abstract void SetUp();
        internal abstract void OpenShield();
        internal abstract void CloseShield();
    }
}