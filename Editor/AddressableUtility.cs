using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameFlow.Editor
{
    internal class AddressableGroupLockProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (!AddressableUtility.IsSystemLocked) return AssetDeleteResult.DidNotDelete;

            if (!assetPath.Contains("AddressableAssetsData/AssetGroups/")) return AssetDeleteResult.DidNotDelete;

            if (assetPath.Contains("GameFlow.asset") || 
                assetPath.Contains("GameFlow_ExcludeFromBuild.asset") || 
                assetPath.Contains("GameFlowManager.asset"))
            {
                Debug.LogError("[GameFlow System] Access Denied! This Addressable Group is hard-locked by code. Deletion is blocked!");
                return AssetDeleteResult.FailedDelete; 
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }

    [InitializeOnLoad]
    internal static class AddressableUtility
    {
        private const string k_groupName = "GameFlow";
        private const string k_excludeGroupName = "GameFlow_ExcludeFromBuild";
        private const string k_controllerName = "GameFlowManager";

        private static bool s_isInternalModifying;

        private static bool s_isSystemLocked
        {
            get => !Configs.instance.AddressableFolderUnlock;
            set
            {
                Configs.instance.AddressableFolderUnlock = !value;
                Configs.instance.SaveData();
            }
        }

        private static Dictionary<string, HashSet<string>> s_lockedGroupCache = new Dictionary<string, HashSet<string>>();

        public static bool IsSystemLocked => s_isSystemLocked;

        static AddressableUtility()
        {
            EditorApplication.delayCall += InitializeSystem;
        }
        
        public static void UnlockSystem()
        {
            if (!s_isSystemLocked) return;

            var isConfirmed = EditorUtility.DisplayDialog(
                "Security Warning: System Unlock",
                "You are about to UNLOCK the Addressable auto-healing and protection system.\n\nAre you sure you know what you're doing?",
                "Yes, I know what I'm doing",
                "Cancel"
            );

            if (!isConfirmed) return;

            s_isSystemLocked = false;
            
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (!settings) return;

            foreach (var group in settings.groups)
            {
                if (group == null || !IsLockedGroupName(group.Name)) continue;
                
                group.WithReadOnlyState(false);
                foreach (var entry in group.entries)
                {
                    if (entry != null) entry.WithReadOnlyState(false);
                }
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
            Debug.LogWarning("[GameFlow System] Addressables System is UNLOCKED. Manual modifications are now allowed.");
        }
        
        public static void LockSystem()
        {
            if (s_isSystemLocked) return;

            s_isSystemLocked = true;
            
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (!settings) return;

            ReapplyLocksInMemory(settings);
            UpdateSnapshotCache(settings);
            
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
            Debug.Log("[GameFlow System] Addressables System is LOCKED. Security protocols re-activated.");
        }

        private static void InitializeSystem()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (!settings) return;

            settings.OnModification -= OnAddressableModification;
            settings.OnModification += OnAddressableModification;

            if (s_isSystemLocked)
            {
                ReapplyLocksInMemory(settings);
                UpdateSnapshotCache(settings);
            }
        }

        private static void ReapplyLocksInMemory(AddressableAssetSettings settings)
        {
            foreach (var group in settings.groups)
            {
                if (group == null || !IsLockedGroupName(group.Name)) continue;

                group.WithReadOnlyState(true);
                foreach (var entry in group.entries)
                {
                    if (entry != null) entry.WithReadOnlyState(true);
                }
            }
        }

        private static void UpdateSnapshotCache(AddressableAssetSettings settings)
        {
            s_lockedGroupCache.Clear();
            foreach (var group in settings.groups)
            {
                if (group != null && IsLockedGroupName(group.Name))
                {
                    var guids = new HashSet<string>();
                    foreach (var entry in group.entries)
                    {
                        if (entry != null) guids.Add(entry.guid);
                    }
                    s_lockedGroupCache[group.Name] = guids;
                }
            }
        }

        private static void WithInternalModify(Action action)
        {
            s_isInternalModifying = true;
            try 
            { 
                action?.Invoke(); 
                
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings && s_isSystemLocked) UpdateSnapshotCache(settings);
            }
            finally { s_isInternalModifying = false; }
        }

        private static bool IsLockedGroupName(string name)
        {
            return name == k_groupName || name == k_excludeGroupName || name == k_controllerName;
        }

        private static void OnAddressableModification(AddressableAssetSettings settings, AddressableAssetSettings.ModificationEvent ev, object eventData)
        {
            if (s_isInternalModifying || !s_isSystemLocked) return;

            if (ev == AddressableAssetSettings.ModificationEvent.EntryMoved ||
                ev == AddressableAssetSettings.ModificationEvent.EntryAdded ||
                ev == AddressableAssetSettings.ModificationEvent.EntryRemoved)
            {
                EditorApplication.delayCall += () => CheckAndHeal(settings);
            }
        }

        private static void CheckAndHeal(AddressableAssetSettings settings)
        {
            if (!settings || !s_isSystemLocked) return;

            var isHealed = false;
            WithInternalModify(() =>
            {
                foreach (var kvp in s_lockedGroupCache)
                {
                    var groupName = kvp.Key;
                    var cachedGuids = kvp.Value;
                    
                    var group = settings.FindGroup(groupName);
                    if (!group) continue;

                    var currentGuids = new HashSet<string>();
                    foreach (var entry in group.entries)
                    {
                        if (entry != null) currentGuids.Add(entry.guid);
                    }

                    foreach (var guid in cachedGuids)
                    {
                        if (!currentGuids.Contains(guid))
                        {
                            var recoveredEntry = settings.CreateOrMoveEntry(guid, group, false, false);
                            recoveredEntry.WithReadOnlyState(true);
                            isHealed = true;
                            Debug.LogWarning($"[GameFlow System] Entry Deletion Prevented! Auto-healed entry '{guid}' back to '{groupName}'. 🛡️");
                        }
                    }

                    var entriesToRemove = new List<AddressableAssetEntry>();
                    foreach (var entry in group.entries)
                    {
                        if (entry != null && !cachedGuids.Contains(entry.guid))
                        {
                            entriesToRemove.Add(entry);
                        }
                    }

                    if (entriesToRemove.Count > 0)
                    {
                        var defaultGroup = settings.DefaultGroup;
                        foreach (var entry in entriesToRemove)
                        {
                            settings.MoveEntry(entry, defaultGroup);
                        }
                        isHealed = true;
                    }
                }

                if (isHealed)
                {
                    ReapplyLocksInMemory(settings);
                    EditorUtility.SetDirty(settings);
                    AssetDatabase.SaveAssets();
                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
                }
            });
        }

        internal static void AddAddressableGroupController(string assetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            WithInternalModify(() => AddAddressableGroupGUID(guid, true, true));
        }

        internal static AssetReferenceElement AddAddressableGroup(string assetPath, bool includeInBuild, bool isScene)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            AssetReferenceElement result = null;
            WithInternalModify(() => result = AddAddressableGroupGUID(guid, includeInBuild, isScene, false));
            return result;
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
            if (!settings) return false;

            var groupName = isController ? k_controllerName : (includeInBuild ? k_groupName : k_excludeGroupName);
            var group = settings.FindGroup(groupName);

            if (!group)
            {
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
                group.GetSchema<BundledAssetGroupSchema>().WithSettings(includeInBuild, isController);
            }

            group.WithReadOnlyState(false);
            
            var entry = settings.CreateOrMoveEntry(guid, group, false, false);
            entry.WithReadOnlyState(false);

            var entriesAdded = new List<AddressableAssetEntry> { entry };
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
            
            EditorUtility.SetDirty(group);
            AssetDatabase.SaveAssets();

            EditorApplication.delayCall += () =>
            {
                if (!settings) return;
                
                if (s_isSystemLocked)
                {
                    ReapplyLocksInMemory(settings); 
                }
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
            };

            return true;
        }
    }

    internal static class AddressableGroupExtensions
    {
        public static void WithReadOnlyState(this AddressableAssetGroup group, bool isReadOnly)
        {
            if (!group) return;

            var serializedGroup = new SerializedObject(group);
            var readOnlyProp = serializedGroup.FindProperty("m_ReadOnly");
            
            if (readOnlyProp == null || readOnlyProp.boolValue == isReadOnly) return;

            readOnlyProp.boolValue = isReadOnly;
            serializedGroup.ApplyModifiedProperties();
        }

        public static void WithReadOnlyState(this AddressableAssetEntry entry, bool isReadOnly)
        {
            if (entry == null) return;

            var prop = typeof(AddressableAssetEntry).GetProperty("ReadOnly", BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
            {
                var currentValue = (bool)prop.GetValue(entry);
                if (currentValue == isReadOnly) return;

                prop.SetValue(entry, isReadOnly);
                return;
            }

            var field = typeof(AddressableAssetEntry).GetField("m_ReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) return;

            var currentFieldValue = (bool)field.GetValue(entry);
            if (currentFieldValue == isReadOnly) return;

            field.SetValue(entry, isReadOnly);
        }
    }

    internal static class AddressableSchemaExtensions
    {
        public static void WithSettings(this BundledAssetGroupSchema schema, bool includeInBuild, bool isController)
        {
            schema.IncludeInBuild = includeInBuild;
            schema.BundleMode = isController ? BundledAssetGroupSchema.BundlePackingMode.PackTogether : BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
        }
    }
}