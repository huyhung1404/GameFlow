using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFlow.Internal
{
    internal class GameFlowContext
    {
        internal static GameFlowContext Current { get; private set; }
        internal GameFlowManager Manager { get; }
        internal GameFlowRuntimeController RuntimeController { get; private set; }
        internal ElementsRuntimeManager ElementsRuntime { get; }
        internal UIElementsRuntimeManager UIElementsRuntime { get; }
        internal Dictionary<Type, ElementCallbackEvent> CallbackEvents { get; }
        internal LoadingController Loading { get; private set; }
        internal Camera UICamera { get; private set; }

        internal GameFlowContext(GameFlowManager manager)
        {
            Manager = manager;
            ElementsRuntime = new ElementsRuntimeManager();
            UIElementsRuntime = new UIElementsRuntimeManager(this);
            CallbackEvents = new Dictionary<Type, ElementCallbackEvent>(16);
        }

        internal void SetRuntimeController(GameFlowRuntimeController runtimeController)
        {
            RuntimeController = runtimeController;
        }

        internal void SetLoadingController(LoadingController loadingController)
        {
            Loading = loadingController;
        }

        internal void SetUICamera(Camera camera)
        {
            UICamera = camera;
        }

        internal static void SetCurrent(GameFlowContext context)
        {
            Current = context;
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            Current = null;
        }
#endif
    }
}