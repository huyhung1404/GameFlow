using System;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    public class GameFlowElement
    {
        internal GameObject runtimeInstance;
        [SerializeField] internal string instanceID;
        [SerializeField] internal bool includeInBuild = true;
        [SerializeField] internal AssetReferenceElement reference;
        [SerializeField] internal ElementReleaseMode releaseMode;
        [SerializeField] internal bool canReActive;
    }
}