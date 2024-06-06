using System;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    public class GameFlowElement
    {
        internal GameObject runtimeInstance;
        private Type _type;
        internal Type elementType => _type ??= GetType();
        [SerializeField] internal string instanceID;
        [SerializeField] internal bool includeInBuild = true;
        [SerializeField] internal AssetReferenceElement reference;
        [SerializeField] internal ElementReleaseMode releaseMode;
        [SerializeField] internal bool canReActive = true;
    }
}