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
        [SerializeField] internal AssetReferenceElement reference;
        [SerializeField] internal ElementReleaseMode releaseMode;
        [SerializeField] internal ElementActiveMode activeMode;

        private void OnEnable()
        {
            elementType = GetType();
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        internal string GetInfo()
        {
            return $"{elementType.Namespace}.{elementType.Name}";
        }

        internal string GetFullInfo()
        {
            return $@"<b><size=11>runtimeInstance:</size></b> {runtimeInstance?.name}
<b><size=11>includeInBuild:</size></b> {includeInBuild}
<b><size=11>canReActive:</size></b> {activeMode}
<b><size=11>reference is scene:</size></b> {reference.IsScene()}
<b><size=11>releaseMode:</size></b> {releaseMode}
";
        }
    }
}