using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameFlow
{
    [Serializable]
    public class AssetReferenceElement : AssetReferenceT<Object>
    {
        [SerializeField] internal bool isScene;

        public AssetReferenceElement(string guid) : base(guid)
        {
            isScene = false;
        }

        public AssetReferenceElement(string guid, bool isScene) : base(guid)
        {
            this.isScene = isScene;
        }

        public override bool ValidateAsset(Object obj)
        {
#if UNITY_EDITOR
            return obj is GameObject || obj is SceneAsset;
#else
            return true;
#endif
        }

        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            var type = AssetDatabase.GetMainAssetTypeAtPath(path);
            return type == typeof(GameObject) || type == typeof(SceneAsset);
#else
            return false;
#endif
        }

#if UNITY_EDITOR
        public override bool SetEditorAsset(Object value)
        {
            if (!base.SetEditorAsset(value)) return false;
            isScene = value is SceneAsset;
            return true;
        }

        public override bool SetEditorSubObject(Object value)
        {
            if (!base.SetEditorSubObject(value)) return false;
            isScene = value is SceneAsset;
            return true;
        }
#endif
    }
}