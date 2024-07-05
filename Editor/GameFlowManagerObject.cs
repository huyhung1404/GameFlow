﻿using System.IO;
using GameFlow.Internal;
using UnityEditor;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace GameFlow.Editor
{
    public static class GameFlowManagerObject
    {
        private static GameFlowManager _instance;

        internal static GameFlowManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = AssetDatabase.LoadAssetAtPath<GameFlowManager>(PackagePath.ManagerPath());
                return _instance;
            }
        }

        internal static void CreateDefaultInstance()
        {
            var manager = ScriptableObject.CreateInstance<GameFlowManager>();
            Directory.CreateDirectory(PackagePath.ProjectFolderPath());
            AssetDatabase.CreateAsset(manager, PackagePath.ManagerPath());
            AddressableUtility.AddAddressableGroupController(PackagePath.ManagerPath());
            CreateScripts();
            CreateTemplates();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateScripts()
        {
            Directory.CreateDirectory(PackagePath.ScriptsGenerateFolderPath());
            CreateFile(ObjectTemplate.AssemblyDefinition);
        }

        private static void CreateTemplates()
        {
            Directory.CreateDirectory(PackagePath.ProjectTemplatesPath());
            CreateFile(ObjectTemplate.GameFlowElementPrefab);
            CreateFile(ObjectTemplate.GameFlowElementScene);
            CreateFile(ObjectTemplate.UserInterfaceGameFlowElementPrefab);
            CreateFile(ObjectTemplate.UserInterfaceGameFlowElementScene);
            CreateFile(ObjectTemplate.TemplateScripts);
        }

        private static void CreateFile(ObjectTemplate.Template template)
        {
            var projectPath = PackagePath.ProjectFolderPath() + "/" + template.folder + "/" + template.fileName;

            var projectDir = Path.GetDirectoryName(projectPath);
            if (!Directory.Exists(projectDir))
            {
                Directory.CreateDirectory(projectDir);
            }

            File.WriteAllText(projectPath, template.content);
        }
    }

    internal static class ObjectTemplate
    {
        internal class Template
        {
            public string folder;
            public string fileName;
            public string content;
        }

        internal static readonly Template AssemblyDefinition = new Template
        {
            fileName = "GameFlowElements.asmdef",
            folder = PackagePath.kScriptFolderName,
            content = TemplateAssemblyDefinition
        };

        internal static readonly Template GameFlowElementPrefab = new Template
        {
            fileName = "TemplateGameFlowElement.prefab",
            folder = "Templates",
            content = TemplateEmptyPrefab
        };

        internal static readonly Template GameFlowElementScene = new Template
        {
            fileName = "TemplateGameFlowElement.unity",
            folder = "Templates",
            content = TemplateEmptyScene
        };

        internal static readonly Template UserInterfaceGameFlowElementPrefab = new Template
        {
            fileName = "TemplateUserInterfaceFlowElement.prefab",
            folder = "Templates",
            content = TemplateEmptyPrefab
        };

        internal static readonly Template UserInterfaceGameFlowElementScene = new Template
        {
            fileName = "TemplateUserInterfaceFlowElement.unity",
            folder = "Templates",
            content = TemplateEmptyScene
        };

        internal static readonly Template TemplateScripts = new Template
        {
            fileName = "TemplateScripts.txt",
            folder = "Templates",
            content = TemplateScript
        };

        internal const string TemplateAssemblyDefinition =
@"{
    ""name"": ""GameFlowElements"",
    ""rootNamespace"": ""GameFlow"",
    ""references"": [
        ""com.huyhung1404.gameflow""
    ],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": true,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}";

        internal const string TemplateEmptyPrefab =
@"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1 &6673496399411173317
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 5117071871698072443}
  m_Layer: 0
  m_Name: TemplateGameFlowElement
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &5117071871698072443
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 6673496399411173317}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_RootOrder: 0
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
";

        internal const string TemplateEmptyScene =
@"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestOccluder: 5
    smallestHole: 0.25
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
  m_OcclusionCullingData: {fileID: 0}
--- !u!104 &2
RenderSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 9
  m_Fog: 0
  m_FogColor: {r: 0.5, g: 0.5, b: 0.5, a: 1}
  m_FogMode: 3
  m_FogDensity: 0.01
  m_LinearFogStart: 0
  m_LinearFogEnd: 300
  m_AmbientSkyColor: {r: 0.212, g: 0.227, b: 0.259, a: 1}
  m_AmbientEquatorColor: {r: 0.114, g: 0.125, b: 0.133, a: 1}
  m_AmbientGroundColor: {r: 0.047, g: 0.043, b: 0.035, a: 1}
  m_AmbientIntensity: 1
  m_AmbientMode: 0
  m_SubtractiveShadowColor: {r: 0.42, g: 0.478, b: 0.627, a: 1}
  m_SkyboxMaterial: {fileID: 10304, guid: 0000000000000000f000000000000000, type: 0}
  m_HaloStrength: 0.5
  m_FlareStrength: 1
  m_FlareFadeSpeed: 3
  m_HaloTexture: {fileID: 0}
  m_SpotCookie: {fileID: 10001, guid: 0000000000000000e000000000000000, type: 0}
  m_DefaultReflectionMode: 0
  m_DefaultReflectionResolution: 128
  m_ReflectionBounces: 1
  m_ReflectionIntensity: 1
  m_CustomReflection: {fileID: 0}
  m_Sun: {fileID: 0}
  m_IndirectSpecularColor: {r: 0.12731749, g: 0.13414757, b: 0.1210787, a: 1}
  m_UseRadianceAmbientProbe: 0
--- !u!157 &3
LightmapSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 12
  m_GIWorkflowMode: 1
  m_GISettings:
    serializedVersion: 2
    m_BounceScale: 1
    m_IndirectOutputScale: 1
    m_AlbedoBoost: 1
    m_EnvironmentLightingMode: 0
    m_EnableBakedLightmaps: 1
    m_EnableRealtimeLightmaps: 0
  m_LightmapEditorSettings:
    serializedVersion: 12
    m_Resolution: 2
    m_BakeResolution: 40
    m_AtlasSize: 1024
    m_AO: 0
    m_AOMaxDistance: 1
    m_CompAOExponent: 1
    m_CompAOExponentDirect: 0
    m_ExtractAmbientOcclusion: 0
    m_Padding: 2
    m_LightmapParameters: {fileID: 0}
    m_LightmapsBakeMode: 1
    m_TextureCompression: 1
    m_FinalGather: 0
    m_FinalGatherFiltering: 1
    m_FinalGatherRayCount: 256
    m_ReflectionCompression: 2
    m_MixedBakeMode: 2
    m_BakeBackend: 1
    m_PVRSampling: 1
    m_PVRDirectSampleCount: 32
    m_PVRSampleCount: 512
    m_PVRBounces: 2
    m_PVREnvironmentSampleCount: 256
    m_PVREnvironmentReferencePointCount: 2048
    m_PVRFilteringMode: 1
    m_PVRDenoiserTypeDirect: 1
    m_PVRDenoiserTypeIndirect: 1
    m_PVRDenoiserTypeAO: 1
    m_PVRFilterTypeDirect: 0
    m_PVRFilterTypeIndirect: 0
    m_PVRFilterTypeAO: 0
    m_PVREnvironmentMIS: 1
    m_PVRCulling: 1
    m_PVRFilteringGaussRadiusDirect: 1
    m_PVRFilteringGaussRadiusIndirect: 5
    m_PVRFilteringGaussRadiusAO: 2
    m_PVRFilteringAtrousPositionSigmaDirect: 0.5
    m_PVRFilteringAtrousPositionSigmaIndirect: 2
    m_PVRFilteringAtrousPositionSigmaAO: 1
    m_ExportTrainingData: 0
    m_TrainingDataDestination: TrainingData
    m_LightProbeSampleCountMultiplier: 4
  m_LightingDataAsset: {fileID: 0}
  m_LightingSettings: {fileID: 0}
--- !u!196 &4
NavMeshSettings:
  serializedVersion: 2
  m_ObjectHideFlags: 0
  m_BuildSettings:
    serializedVersion: 2
    agentTypeID: 0
    agentRadius: 0.5
    agentHeight: 2
    agentSlope: 45
    agentClimb: 0.4
    ledgeDropHeight: 0
    maxJumpAcrossDistance: 0
    minRegionArea: 2
    manualCellSize: 0
    cellSize: 0.16666667
    manualTileSize: 0
    tileSize: 256
    accuratePlacement: 0
    maxJobWorkers: 0
    preserveTilesOutsideBounds: 0
    debug:
      m_Flags: 0
  m_NavMeshData: {fileID: 0}
";

        internal const string TemplateScript =
@"namespace %NAMESPACE%
{
    public class %NAME% : %BASE_CLASS_NAME%
    {
    }
}";
    }
}