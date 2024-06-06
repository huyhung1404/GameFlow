using System;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    public class GameFlowElement : ScriptableObject
    {
        internal GameObject runtimeInstance;
        internal Type elementType;
        [SerializeField] internal string instanceID;
        [SerializeField] internal bool includeInBuild = true;
        [SerializeField] internal AssetReferenceElement reference;
        [SerializeField] internal ElementReleaseMode releaseMode;
        [SerializeField] internal bool canReActive = true;

        private void OnEnable()
        {
            elementType = GetType();
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
    }
}