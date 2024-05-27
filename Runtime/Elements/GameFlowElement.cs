using System;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    public class GameFlowElement
    {
        [SerializeField] internal string instanceID;
        [SerializeField] internal bool includeInBuild = true;
        [SerializeField] internal AssetReferenceElement reference;
        [SerializeField] internal ElementReleaseMode releaseMode;
    }
}