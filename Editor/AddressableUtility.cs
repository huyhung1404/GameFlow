using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine.AddressableAssets;

namespace GameFlow.Editor
{
    internal static class AddressableUtility
    {
        private const string k_groupName = "GameFlow";
        private const string k_excludeGroupName = "GameFlow_ExcludeFromBuild";
        private const string k_controllerName = "GameFlowManager";

        internal static void AddAddressableGroupController(string assetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            AddAddressableGroupGUID(guid, true, true);
        }

        internal static AssetReferenceElement AddAddressableGroup(string assetPath, bool includeInBuild, bool isScene)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            return AddAddressableGroupGUID(guid, includeInBuild, isScene, false);
        }

        internal static AssetReference AddAddressableGroupGUID(string guid, bool includeInBuild, bool isController)
        {
            return !SetGroup(guid, includeInBuild, isController) ? null : new AssetReference(guid);
        }

        private static AssetReferenceElement AddAddressableGroupGUID(string guid, bool includeInBuild, bool isScene, bool isController)
        {
            return !SetGroup(guid, includeInBuild, isController) ? null : new AssetReferenceElement(guid, isScene);
        }

        private static bool SetGroup(string guid, bool includeInBuild, bool isController)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var groupName = includeInBuild ? k_groupName : k_excludeGroupName;
            if (isController) groupName = k_controllerName;
            if (!settings) return false;
            var group = settings.FindGroup(groupName);
            if (!group)
            {
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
                var bundledAssetGroupSchema = group.GetSchema<BundledAssetGroupSchema>();
                bundledAssetGroupSchema.IncludeInBuild = includeInBuild;
                bundledAssetGroupSchema.BundleMode = isController ? BundledAssetGroupSchema.BundlePackingMode.PackTogether : BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
            }

            var e = settings.CreateOrMoveEntry(guid, group, false, false);
            var entriesAdded = new List<AddressableAssetEntry> { e };
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
            return true;
        }
    }
}