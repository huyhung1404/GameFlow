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
        private const string kGroupName = "GameFlow";
        private const string kExcludeGroupName = "GameFlow_ExcludeFromBuild";

        internal static AssetReference AddAddressableGroup(string assetPath, bool includeInBuild)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            return AddAddressableGroupGUID(guid, includeInBuild);
        }

        internal static AssetReferenceElement AddAddressableGroup(string assetPath, bool includeInBuild, bool isScene)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            return AddAddressableGroupGUID(guid, includeInBuild, isScene);
        }

        internal static AssetReference AddAddressableGroupGUID(string guid, bool includeInBuild)
        {
            return !SetGroup(guid, includeInBuild) ? null : new AssetReference(guid);
        }

        private static AssetReferenceElement AddAddressableGroupGUID(string guid, bool includeInBuild, bool isScene)
        {
            return !SetGroup(guid, includeInBuild) ? null : new AssetReferenceElement(guid, isScene);
        }

        private static bool SetGroup(string guid, bool includeInBuild)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var groupName = includeInBuild ? kGroupName : kExcludeGroupName;
            if (!settings) return false;
            var group = settings.FindGroup(groupName);
            if (!group)
            {
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
                var bundledAssetGroupSchema = group.GetSchema<BundledAssetGroupSchema>();
                bundledAssetGroupSchema.IncludeInBuild = includeInBuild;
                bundledAssetGroupSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
            }

            var e = settings.CreateOrMoveEntry(guid, group, false, false);
            var entriesAdded = new List<AddressableAssetEntry> { e };
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
            return true;
        }
    }
}