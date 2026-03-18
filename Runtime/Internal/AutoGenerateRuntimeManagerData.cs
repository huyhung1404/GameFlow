using System;
using UnityEngine;

namespace GameFlow.Internal
{
    [Serializable]
    internal class AutoGenerateRuntimeManagerData
    {
        public AutoGenerateCameraData[] Cameras;
    }

    [Serializable]
    internal class AutoGenerateCameraData
    {
        public bool IsMainUICamera;
        public Camera CameraPrefab;
    }

    public abstract class PresetCamera
    {
        public abstract void Preset(Camera camera);
    }

    public class NormalUICameraPreset : PresetCamera
    {
        public override void Preset(Camera camera)
        {
        }
    }

    public class BackgroundCameraPreset : PresetCamera
    {
        public override void Preset(Camera camera)
        {
        }
    }
}