using System;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    public class GameFlowElement : ScriptableObject
    {
        internal GameObject runtimeInstance;
        internal Type elementType;
        [SerializeField, HideInInspector] internal bool includeInBuild = true;
        [SerializeField] internal bool canReActive = true;
        [SerializeField] internal AssetReferenceElement reference;
        [SerializeField] internal ElementReleaseMode releaseMode;

        private void OnEnable()
        {
            elementType = GetType();
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
    }
}