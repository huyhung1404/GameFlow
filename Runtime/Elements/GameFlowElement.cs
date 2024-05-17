using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameFlow
{
    [Serializable]
    public class GameFlowElement
    {
        [SerializeField] internal bool includeInBuild = true;
        [SerializeField] internal AssetReference reference;
        [SerializeField] internal ElementReleaseMode releaseMode;
    }
}