using System;
using UnityEngine;

namespace GameFlow.Internal
{
    [Serializable]
    public enum ShieldType
    {
        UIImage
    }

    internal abstract class LoadingShield : MonoBehaviour
    {
        public bool IsShieldEnabled { get; protected set; }
        public abstract void SetUp();
        public abstract void OpenShield();
        public abstract void CloseShield();
    }
}