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

    internal class NormalUICameraPreset : PresetCamera
    {
        internal override void Preset(Camera camera)
        {
        }
    }

    internal class BackgroundCameraPreset : PresetCamera
    {
        internal override void Preset(Camera camera)
        {
        }
    }
}