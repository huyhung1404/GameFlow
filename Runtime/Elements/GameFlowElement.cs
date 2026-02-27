using System;
using UnityEngine;

namespace GameFlow
{
    [Serializable]
    public class GameFlowElement : ScriptableObject
    {
        internal GameObject RuntimeInstance;
        internal Type ElementType;
        [SerializeField, HideInInspector] internal bool IncludeInBuild = true;
        [SerializeField] internal AssetReferenceElement Reference;
        [SerializeField] internal ElementReleaseMode ReleaseMode;
        [SerializeField] internal ElementActiveMode ActiveMode = ElementActiveMode.ReActive;

        private void OnEnable()
        {
            ElementType = GetType();
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        internal string GetInfo()
        {
            return $"{ElementType.Namespace}.{ElementType.Name}";
        }

        internal string GetFullInfo()
        {
            return $@"<b><size=11>runtimeInstance:</size></b> {RuntimeInstance?.name}
<b><size=11>includeInBuild:</size></b> {IncludeInBuild}
<b><size=11>canReActive:</size></b> {ActiveMode}
<b><size=11>reference is scene:</size></b> {Reference.IsScene()}
<b><size=11>releaseMode:</size></b> {ReleaseMode}
";
        }

        public void AddElement()
        {
            GameCommand.Add(ElementType).Build();
        }

        public void ReleaseElement()
        {
            GameCommand.Release(ElementType).Build();
        }

        public void LoadElement()
        {
            GameCommand.Load(ElementType).Build();
        }
    }
}