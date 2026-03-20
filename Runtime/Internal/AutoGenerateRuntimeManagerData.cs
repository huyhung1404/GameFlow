using System;
using UnityEngine;

namespace GameFlow.Internal
{
    [Serializable]
    internal class AutoGenerateRuntimeManagerData
    {
        [SerializeField] internal AutoGenerateCameraData[] Cameras;

        [SerializeField] internal ShieldType ShieldType;
        [SerializeField] internal BaseLoadingTypeController[] Loadings;
    }

    [Serializable]
    internal class AutoGenerateCameraData
    {
        [SerializeField] internal bool IsMainUICamera;
        [SerializeField] internal Camera CameraPrefab;
    }

    internal abstract class PresetCamera
    {
        internal abstract void Preset(Camera camera);
    }

    internal class NormalCameraPreset : PresetCamera
    {
        internal override void Preset(Camera camera)
        {
            camera.clearFlags = CameraClearFlags.Depth;
            camera.backgroundColor = Color.black;
            camera.cullingMask = LayerMask.GetMask("UI");
            camera.orthographic = true;
            camera.orthographicSize = 5;
            camera.nearClipPlane = 5f;
            camera.farClipPlane = 105f;
            camera.depth = 50;
            camera.renderingPath = RenderingPath.UsePlayerSettings;
            camera.useOcclusionCulling = false;
            camera.allowHDR = false;
            camera.allowMSAA = false;
            camera.allowDynamicResolution = false;
        }
    }

    internal class BackgroundCameraPreset : PresetCamera
    {
        internal override void Preset(Camera camera)
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.cullingMask = 0;
            camera.orthographic = true;
            camera.orthographicSize = 5;
            camera.nearClipPlane = 5f;
            camera.farClipPlane = 105f;
            camera.depth = -99;
            camera.renderingPath = RenderingPath.UsePlayerSettings;
            camera.useOcclusionCulling = false;
            camera.allowHDR = false;
            camera.allowMSAA = false;
            camera.allowDynamicResolution = false;
        }
    }
}