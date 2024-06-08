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
        [SerializeField, HideInInspector] internal AssetReferenceElement reference;
        [SerializeField, HideInInspector] internal ElementReleaseMode releaseMode;
        [SerializeField, HideInInspector] internal bool canReActive = true;

        private void OnEnable()
        {
            elementType = GetType();
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
    }
}